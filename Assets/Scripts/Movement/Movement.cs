using System;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{

    [Serializable]
    public enum CharacterStates
    {
        Idle = 0,
        Walk,
        Jump,
        DoubleJump,
        Fall,
        Land,
        Shoot
    }
    [SerializeField] private CharacterStates m_currentState;
    [SerializeField] private CharacterStates m_previousState;
    public void SetState(CharacterStates state)
    {
        m_previousState = m_currentState;
        m_currentState = state;
    }

    [Header("Main Components")]
    [SerializeField] private PlayerControls m_controls;
    [SerializeField] private SquashAndStretchController m_squashAndStretch;
    [SerializeField] private CameraZoomController m_camZoom;
    [SerializeField] private CameraShakeController m_camShake;
    private CharacterController m_controller;
    private PlayerWeapon m_weapon;
    private bool m_canMove = true;

    [Header("Movement Values")]
    [SerializeField] private float m_groundedSpeed = 5.0f;
    [SerializeField] private float m_airSpeed = 2.5f;
    [SerializeField] private float m_currentSpeed = 0.0f;
    [SerializeField] private float m_maxSpeed = 10.0f;
    [SerializeField] private float m_increaseSpeed = 5.0f;
    [SerializeField] private float m_decreaseSpeed = 5.0f;
    private float m_movementSpeed = 0.0f;
    private int m_previousDirection = -1;

    [Header("Jump Values")]
    [SerializeField] private float m_jumpHeight = 5.0f;
    [SerializeField] private float m_maxJumpHeight = 10.0f;
    [SerializeField] private float m_maxJumpTimer = 1.0f;
    [SerializeField] private float m_jumpHeightIncreaseSpeed = 5.0f;
    [SerializeField] private float m_jumpHeightDecreaseSpeed = 10.0f;
    [SerializeField] private float m_koyoteTime = 0.0f;
    [SerializeField] private float m_maxKoyoteTime = 0.5f;
    private float m_jumpTimer = 0.0f;

    [Header("Double Jump Values")]
    [SerializeField] private float m_doubleJumpTimer = 0.0f;
    [SerializeField] private float m_maxDoubleJumpTimer = 0.0f;
    [SerializeField] private float m_doubleJumpHeightIncreaseSpeed = 10.0f;
    private bool m_hasDoubleJumped = false;

    [Header("Fall Values")]
    [SerializeField] private float m_fallDelay = 0.0f;
    [SerializeField] private float m_maxFallDelay = 0.1f;
    [SerializeField] private float m_fallTimer = 0.0f;
    [SerializeField] private float m_maxFallTimer = 1.0f;

    [Header("Land Values")]
    [SerializeField] private float m_landTimer = 0.0f;
    [SerializeField] private float m_maxLandTime = 0.5f;

    [Header("Raycasts")]
    [SerializeField] private Transform m_leftPosition;
    [SerializeField] private Transform m_middlePosition;
    [SerializeField] private Transform m_rightPosition;
    [SerializeField] private LayerMask m_groundLayer;
    private const float GroundRayLength = 0.6f;

    [Header("Runtime Values")]
    private Vector2 m_moveInput = Vector2.zero;
    private float m_xMovement = 0.0f;
    private float m_yMovement = 0.0f;
    private Vector3 m_playerPos = Vector3.zero;

    public Vector2 MoveInput => m_moveInput;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer m_fallTrail;
    [SerializeField] private TrailRenderer m_changeDirectionTrail;
    [SerializeField] private float m_timeToStartTrail = 0.25f;

    [Header("UI")]
    [SerializeField] private PlayerUI m_playerUIPrefab;
    private PlayerUI m_playerUI;
    public PlayerUI PlayerUI => m_playerUI;

    private void Awake()
    {
        // Initial state
        SetState(CharacterStates.Idle);

        // Movement
        m_controls = new PlayerControls();
        m_canMove = true;
        m_controller = GetComponent<CharacterController>();

        // Weapon
        m_weapon = GetComponent<PlayerWeapon>();
        m_weapon.Init();

        // UI
        m_playerUI = Instantiate(m_playerUIPrefab);
        m_playerUI.SetAmmoText(m_weapon.CurrentAmmo);
    }

    private void OnEnable()
    {
        m_controls.Enable();

        // Walking
        m_controls.Player.Move.performed += ctx => m_moveInput.x = ctx.ReadValue<float>();
        m_controls.Player.Move.canceled += ctx => m_moveInput.x = 0;

        // Jumping
        m_controls.Player.Jump.performed += ctx => PressedJump();

        // Shooting
        m_controls.Player.Shoot.performed += ctx => m_weapon.isShooting = true;
        m_controls.Player.Shoot.canceled += ctx => m_weapon.isShooting = false;
    }

    private void OnDisable()
    {
        m_controls.Disable();
    }

    private void StateUpdate()
    {
        switch(m_currentState)
        {
            case CharacterStates.Idle:
                IdleUpdate();
                break;

            case CharacterStates.Walk:
                WalkUpdate();
                break;

            case CharacterStates.Jump:
                JumpUpdate();
                break;

            case CharacterStates.DoubleJump:
                DoubleJumpUpdate();
                break;

            case CharacterStates.Fall:
                FallUpdate();
                break;

            case CharacterStates.Land:
                LandUpdate();
                break;

            case CharacterStates.Shoot:
                ShootUpdate();
                break;
        }
    }

    private void IdleUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Idle);
            m_camZoom.SetZoonType(CameraZoomController.CameraZoomState.Idle);
            m_canMove = true;
            m_previousState = m_currentState;
        }

        m_currentSpeed = (m_currentSpeed > 0) ? m_currentSpeed - Time.deltaTime : 0f;

        if (m_weapon.isShooting && m_weapon.CurrentAmmo > 0)
        {
            SetState(CharacterStates.Shoot);
        }
        else if (m_moveInput.x != 0 && IsGrounded())
        {
            SetState(CharacterStates.Walk);
        }
        else if (!IsGrounded())
        {
            SetState(CharacterStates.Fall);
        }

        ResetJumpStats();
    }

    private void WalkUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Walk);
            m_camZoom.SetZoonType(CameraZoomController.CameraZoomState.Walk);
            m_previousState = m_currentState;
        }

        if (m_weapon.isShooting)
        {
            SetState(CharacterStates.Shoot);
        }
        else if (m_moveInput.x == 0 && IsGrounded())
        {
            SetState(CharacterStates.Idle);
        }
        else if (!IsGrounded())
        {
            SetState(CharacterStates.Fall);
        }

        m_movementSpeed = m_groundedSpeed;

        ResetJumpStats();
    }

    private void JumpUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Jump);
            m_camZoom.SetZoonType(CameraZoomController.CameraZoomState.Jump);
            m_previousState = m_currentState;
        }

        if (m_weapon.isShooting)
        {
            SetState(CharacterStates.Shoot);
            return;
        }

        // Has player landed?
        if (m_jumpTimer > 0.5f)
        {
            if (Landed())
            {
                return;
            }
        }

        HitRoof();

        // Detect when to move to falling state
        m_jumpTimer += Time.deltaTime;
        if(m_jumpTimer > m_maxJumpTimer)
        {
            m_fallDelay += Time.deltaTime;
            if (m_fallDelay > m_maxFallDelay)
            {
                SetState(CharacterStates.Fall);
            }
            return;
        }

        // Jumping logic
        m_moveInput.y = 1;
        m_jumpHeight = m_jumpHeightIncreaseSpeed;

        // Setting horizontal movement when in the air
        m_movementSpeed = m_airSpeed;
    }

    private void DoubleJumpUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.DoubleJump);
            m_camZoom.SetZoonType(CameraZoomController.CameraZoomState.DoubleJump);
            m_hasDoubleJumped = true;
            m_doubleJumpTimer = 0f;
            m_fallTimer = 0f;
            m_previousState = m_currentState;
        }

        if (m_weapon.isShooting)
        {
            SetState(CharacterStates.Shoot);
            return;
        }

        // Has player landed?
        if (Landed())
        {
            return;
        }

        // Detect when to move to falling state
        m_doubleJumpTimer += Time.deltaTime;
        if (m_doubleJumpTimer > m_maxDoubleJumpTimer)
        {
            SetState(CharacterStates.Fall);
            return;
        }

        // Jumping logic
        m_moveInput.y = 1;
        m_jumpHeight = m_doubleJumpHeightIncreaseSpeed;

        // Setting horizontal movement when in the air
        m_movementSpeed = m_airSpeed;
    }

    private void FallUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Fall);
            m_fallTimer = 0.0f;
            m_previousState = m_currentState;
        }

        m_fallTimer += Time.deltaTime;

        if(m_fallTimer > m_timeToStartTrail)
        {
            m_fallTrail.time = 0.1f;
        }

        if (Landed())
        {
            return;
        }

        // Falling logic
        m_moveInput.y = -1;
        m_jumpHeight = m_jumpHeightDecreaseSpeed;

        // Setting horizontal movement when in the air
        m_movementSpeed = m_airSpeed;
    }

    private void LandUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Land);
            m_camShake.StartShake(CameraShakeController.CameraShakeType.ShiftDownAtTarget, transform.position);
            m_landTimer = 0.0f;
            m_previousState = m_currentState;
        }

        m_landTimer += Time.deltaTime;

        if (m_landTimer > m_maxLandTime)
        {
            SetState(CharacterStates.Idle);
            return;
        }

        // Land logic
        m_canMove = false;
        m_currentSpeed = 0f;
    }
    private void ShootUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Shoot);
            m_previousState = m_currentState;
        }

        PlayerWeapon weapon = m_weapon;

        if(weapon == null)
        {
            return;
        }

        if (!m_weapon.isShooting)
        {
            if (IsGrounded())
            {
                SetState(CharacterStates.Idle);
            }
            else
            {
                SetState(CharacterStates.Fall);
            }
        }

        weapon.Shoot();

        if(!IsGrounded())
        {
            Knockback(weapon);
        }

        if (!IsGrounded())
        {
            m_xMovement = 0f;
        }

        m_yMovement = 0f;
    }

    private void Knockback(PlayerWeapon weapon)
    {
        Vector3 dir = weapon.Direction;
        m_controller.Move(-dir * Time.deltaTime * weapon.KnockbackPower);
    }

    private bool Landed()
    {
        if (IsGrounded() && m_fallTimer > m_maxFallTimer)
        {
            SetState(CharacterStates.Land);
            return true;
        }
        else if (IsGrounded())
        {
            SetState(CharacterStates.Idle);
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Application.targetFrameRate = 30;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Application.targetFrameRate = 60;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Application.targetFrameRate = 250;
        }

        StateUpdate();

        if (!IsGrounded())
        {
            m_koyoteTime += Time.deltaTime;
        }
        else
        {
            m_koyoteTime = 0.0f;
        }

        if(m_currentState != CharacterStates.Fall && m_fallTrail.time > 0f)
        {
            m_fallTrail.time -= Time.deltaTime * .5f;
        }

        ControllerUpdate();
    }

    private void ControllerUpdate()
    {
        if (!m_canMove)
        {
            return;
        }

        if (m_moveInput.x == 0)
        {
            m_currentSpeed = Mathf.Max(0, m_currentSpeed - Time.deltaTime * m_decreaseSpeed);
        }
        else if (m_currentSpeed < m_maxSpeed)
        {
            m_currentSpeed += Time.deltaTime * m_increaseSpeed;
        }

        if(m_changeDirectionTrail.time > 0f)
        {
            m_changeDirectionTrail.time -= Time.deltaTime;
        }

        // Check if changed direction and decrease acceleration if so
        if ((int)m_moveInput.x != m_previousDirection)
        {
            m_currentSpeed = Mathf.Max(0, m_currentSpeed / 2);
            m_changeDirectionTrail.time = (IsGrounded()) ? 0.5f : m_changeDirectionTrail.time;
        }

        m_previousDirection = (int)m_moveInput.x;

        if (!m_weapon.isShooting || IsGrounded())
        {
            // Control horizontal movement
            m_xMovement = m_moveInput.x * m_currentSpeed * m_movementSpeed * Time.deltaTime;
        }

        if (!m_weapon.isShooting)
        {
            // Control vertical movement
            m_yMovement = m_moveInput.y * Time.deltaTime * m_jumpHeight;
        }

        // Moving the player
        m_playerPos = new Vector2(m_xMovement, m_yMovement);
        m_controller.Move(m_playerPos);
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(m_leftPosition.position, Vector2.down * GroundRayLength, Color.red);
        Debug.DrawRay(m_middlePosition.position, Vector2.down * GroundRayLength, Color.red);
        Debug.DrawRay(m_rightPosition.position, Vector2.down * GroundRayLength, Color.red);

        bool hit = false;

        if (Physics.Raycast(m_leftPosition.position, Vector3.down, GroundRayLength, m_groundLayer))
        {
            hit = true;
        }

        if (Physics.Raycast(m_middlePosition.position, Vector3.down, GroundRayLength, m_groundLayer))
        {
            hit = true;
        }

        if (Physics.Raycast(m_rightPosition.position, Vector3.down, GroundRayLength, m_groundLayer))
        {
            hit = true;
        }   

        return hit;
    }

    private bool HitRoof()
    {
        Debug.DrawRay(m_middlePosition.position, Vector2.up * GroundRayLength, Color.red);

        bool hit = false;
        RaycastHit raycastHit;
        if (Physics.Raycast(m_middlePosition.position, Vector3.up, out raycastHit, GroundRayLength, m_groundLayer))
        {
            hit = true;

            // Get this object's collider
            Collider thisCollider = GetComponent<Collider>();

            // Get the collider that was hit
            Collider hitCollider = raycastHit.collider;

            // Make them ignore each other
            if (thisCollider != null && hitCollider != null)
            {
                Physics.IgnoreCollision(thisCollider, hitCollider);

                // Start coroutine to re-enable collisions safely
                StartCoroutine(ReenableCollisionAfterSeparation(thisCollider, hitCollider));
            }
        }
        else
        {
            hit = false;
        }

        return hit;
    }

    private IEnumerator ReenableCollisionAfterSeparation(Collider a, Collider b)
    {
        // Wait a minimum delay based on frame time (helps with lower framerates)
        float timer = 0f;
        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Wait until colliders have stopped interacting
        while (a.bounds.Intersects(b.bounds))
        {
            yield return null;
        }

        // Re-enable collision
        Physics.IgnoreCollision(a, b, false);
    }

    private void PressedJump()
    {
        if (m_weapon.isShooting)
        {
            return;
        }

        if (!IsGrounded() && m_koyoteTime < m_maxKoyoteTime)
        {
            SetState(CharacterStates.Jump);
            return;
        }
        else if (!IsGrounded() && !m_hasDoubleJumped)
        {
            SetState(CharacterStates.DoubleJump);
            return;
        }
        else if(!IsGrounded())
        {
            return;
        }

        // Reset the koyote time
        m_koyoteTime = 0.0f;

        if(m_currentState != CharacterStates.Walk && m_currentState != CharacterStates.Idle)
        {
            return;
        }

        if(m_currentState == CharacterStates.Jump)
        {
            return;
        }

        ResetJumpStats();
        SetState(CharacterStates.Jump);
    }

    private void ResetJumpStats()
    {
        m_yMovement = 0.0f;
        m_moveInput.y = 0.0f;
        m_jumpTimer = 0.0f;
        m_fallDelay = 0.0f;
        m_hasDoubleJumped = false;
    }

}
