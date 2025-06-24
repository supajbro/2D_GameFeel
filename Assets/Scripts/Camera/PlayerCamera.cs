using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private Player m_player;
    public Player Player => m_player;

    [SerializeField] private Vector2 m_screenThreshold = new Vector2(0.75f, 0.75f);
    private CameraShakeController m_camShake;

    [Header("Speed Values")]
    [SerializeField] private float m_xSpeed = 5.0f;
    [SerializeField] private float m_ySpeed = 2.5f;

    private Vector3 m_pos = Vector3.zero;
    private float m_zPos = -10f;

    private Camera m_cam;

    private void Start()
    {
        m_player = FindObjectOfType<Player>();
        m_cam = GetComponent<Camera>();
        m_camShake = GetComponent<CameraShakeController>();
    }

    private void LateUpdate()
    {
        PositionUpdate();
    }

    private void PositionUpdate()
    {
        if(m_player == null)
        {
            return;
        }

        if(m_cam == null)
        {
            return;
        }

        if (!m_player.CanMove)
        {
            return;
        }

        Vector3 viewPos = m_cam.WorldToViewportPoint(m_player.transform.position);
        Vector3 camMove = Vector3.zero;

        // Check horizontal movement
        if (viewPos.x > m_screenThreshold.x)
        {
            camMove.x = viewPos.x - m_screenThreshold.x;
        }
        else if (viewPos.x < 1f - m_screenThreshold.x)
        {
            camMove.x = viewPos.x - (1f - m_screenThreshold.x);
        }

        // Check vertical movement
        if (viewPos.y > m_screenThreshold.y)
        {
            camMove.y = viewPos.y - m_screenThreshold.y;
        }
        else if (viewPos.y < 1f - m_screenThreshold.y)
        {
            camMove.y = viewPos.y - (1f - m_screenThreshold.y);
        }

        // Move directly to player pos
        if (m_player.MoveInput == Vector2.zero)
        {
            m_pos = new Vector3(m_player.transform.position.x, m_player.transform.position.y, m_zPos);
            transform.position = Vector3.MoveTowards(transform.position, m_pos, m_xSpeed * Time.deltaTime);
        }
        // Lerp smoothly to player
        else if (camMove != Vector3.zero)
        {
            Vector3 worldMove = m_cam.ViewportToWorldPoint(camMove) - m_cam.ViewportToWorldPoint(Vector3.zero);
            Vector3 targetPos = transform.position + new Vector3(worldMove.x, worldMove.y, 0f);

            // Use separate speeds for x and y
            float xSpeed = m_xSpeed;
            float ySpeed = m_ySpeed;

            Vector3 lerpedPos = new Vector3(
                Mathf.Lerp(transform.position.x, targetPos.x, xSpeed * Time.deltaTime),
                Mathf.Lerp(transform.position.y, m_player.transform.position.y, ySpeed * Time.deltaTime),
                transform.position.z
            );

            transform.position = lerpedPos;

            //float x = Mathf.Lerp(transform.position.x, m_pos.x, m_speed * Time.deltaTime);
            //float y = m_pos.y;

            //transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, m_zPos), m_speed * Time.deltaTime);
            //transform.position = new Vector3(x, y, m_zPos);
        }
    }

}
