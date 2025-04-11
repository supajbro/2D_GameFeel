using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{

    public enum CameraShakeType
    {
        ShiftDownAtTarget = 0
    }
    private CameraShakeType m_type;
    private Camera m_cam;
    private bool m_shake = false;

    public bool IsShaking => m_shake;

    [SerializeField] private AnimationCurve m_shiftDownCurve;
    private float m_elapsedTime = 0.0f;

    private Quaternion m_initRot = Quaternion.identity;
    private Vector2 m_impactDirection = Vector2.zero;

    [SerializeField] private float m_duration = 0.5f;
    [SerializeField] private float m_rotIntensity = 10.0f;
    [SerializeField] private float m_shakeIntensity = 5.0f;

    private void Start()
    {
        m_cam = GetComponent<Camera>();
    }

    public void StartShake(CameraShakeType type, Vector3 worldImpactPosition)
    {
        m_type = type;
        m_shake = true;
        m_elapsedTime = 0.0f;
        m_initRot = transform.rotation;

        Vector3 screenPoint = m_cam.WorldToViewportPoint(worldImpactPosition);

        // Calculate direction from center (0.5, 0.5) to impact point
        Vector2 dirFromCenter = new Vector2(screenPoint.x - 0.5f, screenPoint.y - 0.5f).normalized;

        m_impactDirection = dirFromCenter;
    }

    private void Update()
    {
        if (m_shake == false)
        {
            return;
        }

        switch (m_type)
        {
            case CameraShakeType.ShiftDownAtTarget:
                Shake(m_duration);
                break;
        }
    }

    private void Shake(float duration)
    {
        m_elapsedTime += Time.deltaTime;
        float t = m_elapsedTime / duration;

        if (m_elapsedTime < duration)
        {
            float scaleFactor = m_shiftDownCurve.Evaluate(t);

            // Rotation around Z axis (roll) based on impact X direction
            float zRot = m_impactDirection.x * m_rotIntensity;

            transform.rotation = Quaternion.Lerp(m_initRot,
                Quaternion.Euler(0, 0, zRot),
                scaleFactor);
        }
        else
        {
            m_shake = false;
        }
    }

}
