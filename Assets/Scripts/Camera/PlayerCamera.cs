using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private Movement m_player;
    [SerializeField] private Vector2 m_screenThreshold = new Vector2(0.75f, 0.75f);
    [SerializeField] private float m_speed = 5.0f;

    private Vector3 m_pos = Vector3.zero;
    private float m_zPos = -10f;

    private Camera m_cam;

    private void Start()
    {
        m_cam = GetComponent<Camera>();
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

        if (m_player.MoveInput == Vector2.zero)
        {
            m_pos = new Vector3(m_player.transform.position.x, m_player.transform.position.y, m_zPos);
            transform.position = Vector3.MoveTowards(transform.position, m_pos, m_speed * Time.deltaTime);
        }
        else if (camMove != Vector3.zero)
        {
            Vector3 worldMove = m_cam.ViewportToWorldPoint(camMove) - m_cam.ViewportToWorldPoint(Vector3.zero);
            m_pos = transform.position + new Vector3(worldMove.x, worldMove.y, 0f);
            transform.position = Vector3.Lerp(transform.position, m_pos, m_speed * Time.deltaTime);
        }
    }

}
