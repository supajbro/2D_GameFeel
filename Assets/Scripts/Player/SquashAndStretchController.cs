using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretchController : MonoBehaviour
{

    public enum SquashAndStretchType
    {
        Idle = 0,
        Walk,
        Jump,
        DoubleJump,
        Fall,
        Land
    }

    private SquashAndStretchType m_type;
    private bool m_animate = false;
    private float m_elapsedTime = 0.0f;
    private const float Duration = 0.15f;

    private Vector2 m_initScale = Vector2.one;
    private Vector2 m_currentScale = Vector2.one;

    [Header("Curves")]
    [SerializeField] private AnimationCurve m_idleCurve;
    [SerializeField] private AnimationCurve m_walkCurve;
    [SerializeField] private AnimationCurve m_jumpCurve;
    [SerializeField] private AnimationCurve m_fallCurve;
    [SerializeField] private AnimationCurve m_landCurve;

    [Header("Scales")]
    [SerializeField] private Vector2 m_idleScale = new Vector2(1.0f, 1.0f);
    [SerializeField] private Vector2 m_walkScale = new Vector2(1.0f, 1.0f);
    [SerializeField] private Vector2 m_jumpScale = new Vector2(0.75f, 0.75f);
    [SerializeField] private Vector2 m_doubleJumpScale = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 m_fallScale = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 m_landScale = new Vector2(0.25f, 0.25f);

    private void Start()
    {
        m_initScale = transform.localScale;
    }

    public void SetSquashType(SquashAndStretchType type)
    {
        m_type = type;
        m_animate = true;
        m_elapsedTime = 0.0f;
        m_currentScale = transform.localScale;
    }

    private void Update()
    {
        switch (m_type)
        {
            case SquashAndStretchType.Idle:
                Animate(m_idleScale, Duration, m_idleCurve);
                break;

            case SquashAndStretchType.Walk:
                Animate(m_walkScale, Duration, m_walkCurve);
                break;

            case SquashAndStretchType.Jump:
                Animate(m_jumpScale, Duration, m_jumpCurve);
                break;

            case SquashAndStretchType.DoubleJump:
                Animate(m_doubleJumpScale, Duration, m_fallCurve);
                break;

            case SquashAndStretchType.Fall:
                Animate(m_fallScale, Duration, m_fallCurve);
                break;

            case SquashAndStretchType.Land:
                Animate(m_landScale, Duration, m_landCurve);
                break;
        }
    }

    private void Animate(Vector2 newScale, float duration, AnimationCurve curve)
    {
        if (!m_animate)
        {
            return;
        }

        m_elapsedTime += Time.deltaTime;
        float t = m_elapsedTime / duration;

        if (m_elapsedTime < duration)
        {
            float scaleFactor = curve.Evaluate(t);
            transform.localScale = Vector2.LerpUnclamped(m_currentScale, newScale, scaleFactor);
        }
        else
        {
            m_animate = false;
        }
    }

}
