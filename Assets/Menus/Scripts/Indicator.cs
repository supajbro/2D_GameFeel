using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private Enemy m_enemy;
    private Camera m_cam;
    private CanvasGroup m_canvasGroup;

    private RectTransform m_rect;
    [SerializeField] private float m_edgeBuffer = 50f;

    private void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_rect = GetComponent<RectTransform>();
        m_cam = FindObjectOfType<LevelRules>().CurrentPlayer.Cam;
    }

    public void Init(Enemy enemy)
    {
        m_enemy = enemy;
    }

    private void Update()
    {
        if (m_enemy == null)
        {
            return;
        }

        Vector3 screenPos = m_cam.WorldToScreenPoint(m_enemy.transform.position);

        bool isOffScreen = screenPos.z < 0 ||
                           screenPos.x < 0 || screenPos.x > Screen.width ||
                           screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen && m_enemy.CanMove)
        {
            screenPos.z = Mathf.Max(screenPos.z, 0.1f); // Prevent negative z

            // Clamp to screen with buffer
            screenPos.x = Mathf.Clamp(screenPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
            screenPos.y = Mathf.Clamp(screenPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);

            m_canvasGroup.alpha = 1f;
            m_rect.position = screenPos;

            // Rotate the indicator to point toward the enemy
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            m_rect.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            m_canvasGroup.alpha = 0f;
        }
    }
}
