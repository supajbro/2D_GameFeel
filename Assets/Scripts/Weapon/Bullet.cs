using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private SpriteRenderer m_sprite;

    [Header("Values")]
    [SerializeField] private bool m_isActive = false;
    [SerializeField] private float m_speed = 10.0f;

    [Header("Lifespan")]
    [SerializeField] private float m_lifeSpan = 0.0f;
    [SerializeField] private float m_maxLifeSpan = 1.0f;

    private Vector3 m_dir;

    public bool IsActive => m_isActive;

    private void Awake()
    {
        m_sprite.enabled = false;
    }

    public void Init(Transform shootPos, Vector3 dir)
    {
        m_isActive = true;
        m_sprite.enabled = true;
        m_lifeSpan = 0.0f;
        transform.position = shootPos.position;
        m_dir = dir;
    }

    private void Update()
    {
        if(m_isActive == false)
        {
            return;
        }

        Velocity();
        LifeSpan();
    }

    private void Velocity()
    {
        transform.Translate(m_dir * Time.deltaTime * m_speed);
    }

    private void LifeSpan()
    {
        m_lifeSpan = (m_lifeSpan < m_maxLifeSpan) ? m_lifeSpan + Time.deltaTime : m_maxLifeSpan;

        if (m_lifeSpan >= m_maxLifeSpan)
        {
            m_isActive = false;
            m_sprite.enabled = false;
        }
    }

}
