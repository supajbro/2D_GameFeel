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
                Animate(new Vector2(1.0f, 1.0f), Duration, m_idleCurve);
                break;

            case SquashAndStretchType.Walk:
                Animate(new Vector2(1.0f, 1.0f), Duration, m_walkCurve);
                break;

            case SquashAndStretchType.Jump:
                Animate(new Vector2(0.75f,0.75f), Duration, m_jumpCurve);
                break;

            case SquashAndStretchType.Fall:
                Animate(new Vector2(0.5f, 1.0f), 0.5f, m_fallCurve);
                break;

            case SquashAndStretchType.Land:
                Animate(new Vector2(0.25f, 0.25f), Duration, m_landCurve);
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
