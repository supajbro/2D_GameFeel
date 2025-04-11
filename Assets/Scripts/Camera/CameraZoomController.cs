using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static SquashAndStretchController;

public class CameraZoomController : MonoBehaviour
{

    public enum CameraZoomState
    {
        Idle = 0,
        Walk,
        Jump,
        DoubleJump,
        Fall,
    }
    private CameraZoomState m_state;
    private Camera m_cam;

    private bool m_zoom = false;

    [SerializeField] private AnimationCurve m_curve;
    private float m_elapsedTime = 0.0f;
    private float m_initSize = 0f;

    [SerializeField] private Vector2 m_idleZoom = new Vector2(6.0f, 1.0f);
    [SerializeField] private Vector2 m_walkZoom = new Vector2(7.0f, 1.0f);
    [SerializeField] private Vector2 m_jumpZoom = new Vector2(8.0f, 1.0f);
    [SerializeField] private Vector2 m_doubleJjumpZoom = new Vector2(8.0f, 1.0f);

    private void Start()
    {
        m_cam = GetComponent<Camera>();
    }

    public void SetZoonType(CameraZoomState state)
    {
        m_state = state;
        m_zoom = true;
        m_elapsedTime = 0.0f;
        m_initSize = m_cam.orthographicSize;
    }

    private void Update()
    {
        if(m_zoom == false)
        {
            return;
        }

        if(m_cam == null)
        {
            return;
        }

        switch (m_state)
        {
            case CameraZoomState.Idle:
                Zoom(m_idleZoom.x, m_idleZoom.y);
                break;

            case CameraZoomState.Walk:
                Zoom(m_walkZoom.x, m_walkZoom.y);
                break;

            case CameraZoomState.Jump:
                Zoom(m_jumpZoom.x, m_jumpZoom.y);
                break;

            case CameraZoomState.DoubleJump:
                Zoom(m_doubleJjumpZoom.x, m_doubleJjumpZoom.y);
                break;
        }
    }

    private void Zoom(float size, float duration)
    {
        if(m_cam.orthographicSize == size)
        {
            return;
        }

        m_elapsedTime += Time.deltaTime;
        float t = m_elapsedTime / duration;

        if(m_elapsedTime < duration)
        {
            float scaleFactor = m_curve.Evaluate(t);
            float orthoSize = m_cam.orthographicSize;
            orthoSize = Mathf.Lerp(m_initSize, size, scaleFactor);
            m_cam.orthographicSize = orthoSize;
        }
        else
        {
            m_zoom = false;
        }
    }

}
