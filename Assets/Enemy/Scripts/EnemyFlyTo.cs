using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyTo : MonoBehaviour, IEnemy, IHealth
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

    [Header("Fly To")]
    [SerializeField] private Transform m_target;

    private void Start()
    {
        m_target = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        MoveUpdate();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!m_canMove)
        {
            return;
        }

        if (collision.gameObject.tag == Tags.PLAYER_TAG)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                AttackUpdate(player);
            }
        }
    }

    public void AttackUpdate(Player player)
    {
        player.OnDamage.Invoke(Damage);
    }

    public void MoveUpdate()
    {
        if (!m_canMove)
        {
            return;
        }

        Vector2 targetPosition = new Vector2(m_target.position.x, m_target.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_moveSpeed);
    }
}
