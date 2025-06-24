using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour, IEnemy, IHealth
{
    [Header("Enemy Values")]
    [SerializeField] private EnemyState m_state;
    public EnemyState State
    {
        get => m_state;
        set => m_state = value;
    }

    [SerializeField] private bool m_canMove;
    public bool CanMove
    {
        get => m_canMove;
        set => m_canMove = value;
    }

    [SerializeField] private float m_moveSpeed;
    public float MoveSpeed
    {
        get => m_moveSpeed;
        set => m_moveSpeed = value;
    }

    [SerializeField] private float m_damage;
    public float Damage
    {
        get => m_damage;
        set => m_damage = value;
    }

    public virtual void AttackUpdate(Player player){}
    public virtual void MoveUpdate(){}

    [Header("Health")]
    [SerializeField] private float m_currentHealth;
    public float CurrentHealth
    {
        get => m_currentHealth;
        set => m_currentHealth = value;
    }

    [SerializeField] private float m_maxHealth;
    public float MaxHealth
    {
        get => m_maxHealth;
        set => m_maxHealth = value;
    }

    public void ChangeHealth(float health)
    {
        m_currentHealth -= health;
        m_currentHealth = Mathf.Max(0, m_currentHealth);

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        m_canMove = false;
        gameObject.SetActive(false);
    }
    public Action<float> OnDamage;

    private void Awake()
    {
        OnDamage += ChangeHealth;
    }
}
