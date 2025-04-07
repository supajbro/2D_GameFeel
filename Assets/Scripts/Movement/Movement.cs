using System;
using UnityEngine;

public class Movement : MonoBehaviour
{

    [Serializable]
    public enum CharacterStates
    {
        Idle = 0,
        Walk,
        Jump,
        Fall,
        Shoot
    }
    [SerializeField] private CharacterStates m_currentState;
    [SerializeField] private CharacterStates m_previousState;
    private void SetState(CharacterStates state)
    {
        m_previousState = m_currentState;
        m_currentState = state;
    }

    [Header("Main Components")]
    [SerializeField] private PlayerControls m_controls;
    [SerializeField] private SquashAndStretchController m_squashAndStretch;
    private CharacterController m_controller;

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
    private float m_jumpTimer = 0.0f;
    private float m_currentJumpHeight = 0f;

    [Header("Raycasts")]
    [SerializeField] private Transform m_leftPosition;
    [SerializeField] private Transform m_middlePosition;
    [SerializeField] private Transform m_rightPosition;
    private const float GroundRayLength = 0.6f;

    [Header("Fall Values")]
    [SerializeField] private float m_fallDelay = 0.0f;
    [SerializeField] private float m_maxFallDelay = 0.1f;

    [Header("Hidden Values")]
    private Vector2 m_moveInput = Vector2.zero;
    private float m_xMovement = 0.0f;
    private float m_yMovement = 0.0f;
    private Vector3 m_playerPos = Vector3.zero;

    public Vector2 MoveInput => m_moveInput;

    private void Awake()
    {
        m_controls = new PlayerControls();
        m_controller = GetComponent<CharacterController>();
        SetState(CharacterStates.Idle);
    }

    private void OnEnable()
    {
        m_controls.Enable();

        // Walking
        m_controls.Player.Move.performed += ctx => m_moveInput.x = ctx.ReadValue<float>();
        m_controls.Player.Move.canceled += ctx => m_moveInput.x = 0;

        // Jumping
        m_controls.Player.Jump.performed += ctx => PressedJump();
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

            case CharacterStates.Fall:
                FallUpdate();
                break;
        }
    }

    private void IdleUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Idle);
            m_previousState = m_currentState;
        }

        if (m_moveInput.x != 0 && IsGrounded())
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
            m_previousState = m_currentState;
        }

        if (m_moveInput.x == 0 && IsGrounded())
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
            m_previousState = m_currentState;
        }

        // Has player landed?
        if (m_jumpTimer > 0.5f)
        {
            if (Landed())
            {
                return;
            }
        }

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
        m_currentJumpHeight = Time.deltaTime * m_jumpHeightIncreaseSpeed;
        m_jumpHeight = m_jumpHeightIncreaseSpeed;

        // Setting horizontal movement when in the air
        m_movementSpeed = m_airSpeed;
    }

    private void FallUpdate()
    {
        if (m_previousState != m_currentState)
        {
            m_squashAndStretch.SetSquashType(SquashAndStretchController.SquashAndStretchType.Fall);
            m_previousState = m_currentState;
        }

        if (Landed())
        {
            return;
        }

        // Falling logic
        m_moveInput.y = -1;
        m_currentJumpHeight = Time.deltaTime * m_jumpHeightDecreaseSpeed;
        m_jumpHeight = m_jumpHeightDecreaseSpeed;

        // Setting horizontal movement when in the air
        m_movementSpeed = m_airSpeed;
    }

    private bool Landed()
    {
        if (m_moveInput.x == 0 && IsGrounded())
        {
            SetState(CharacterStates.Idle);
            //m_yMovement = 0.0f;
            return true;
        }
        else if (IsGrounded())
        {
            SetState(CharacterStates.Walk);
            //m_yMovement = 0.0f;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Application.targetFrameRate = 60;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Application.targetFrameRate = 250;
        }

        IsGrounded();
        StateUpdate();

        if(m_moveInput.x == 0)
        {
            m_currentSpeed = Mathf.Max(0, m_currentSpeed - Time.deltaTime * m_decreaseSpeed);
        }
        else if(m_currentSpeed < m_maxSpeed)
        {
            m_currentSpeed += Time.deltaTime * m_increaseSpeed;
        }

        ControllerUpdate();
    }

    private void ControllerUpdate()
    {
        // Check if changed direction and decrease acceleration if so
        if ((int)m_moveInput.x != m_previousDirection)
        {
            m_currentSpeed = Mathf.Max(0, m_currentSpeed / 2);
        }

        m_previousDirection = (int)m_moveInput.x;

        // Control horizontal movement
        m_xMovement = m_moveInput.x * m_currentSpeed * m_movementSpeed * Time.deltaTime;

        // Control vertical movement
        m_yMovement = m_moveInput.y * Time.deltaTime * m_jumpHeight;

        // Moving the player
        m_playerPos = new Vector2(m_xMovement, m_yMovement);
        m_controller.Move(m_playerPos);
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(m_leftPosition.position, Vector2.down * GroundRayLength, Color.red);
        Debug.DrawRay(m_middlePosition.position, Vector2.down * GroundRayLength, Color.red);
        Debug.DrawRay(m_rightPosition.position, Vector2.down * GroundRayLength, Color.red);

        if (Physics.Raycast(m_leftPosition.position, Vector3.down, GroundRayLength))
        {
            return true;
        }

        if (Physics.Raycast(m_middlePosition.position, Vector3.down, GroundRayLength))
        {
            return true;
        }

        if (Physics.Raycast(m_rightPosition.position, Vector3.down, GroundRayLength))
        {
            return true;
        }

        return false;
    }

    private void PressedJump()
    {
        if (!IsGrounded())
        {
            return;
        }

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
        m_currentJumpHeight = 0.0f;
        m_yMovement = 0.0f;
        m_moveInput.y = 0.0f;
        m_jumpTimer = 0.0f;
        m_fallDelay = 0.0f;
    }

}
