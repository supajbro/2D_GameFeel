using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
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

    [Header("Set Path")]
    [SerializeField] private bool m_moveRight = true;
    [SerializeField] private Transform m_rightPath;
    [SerializeField] private Transform m_leftPath;
    private const float MinDistToTarget = 0.1f;

    private void Update()
    {
        MoveUpdate();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == Tags.PLAYER_TAG)
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
        if (!m_canMove)
        {
            return;
        }
        player.OnDamage.Invoke(Damage);
    }

    public void MoveUpdate()
    {
        if (!m_canMove)
        {
            return;
        }

        Transform targetPath = (m_moveRight) ? m_rightPath : m_leftPath;

        Vector2 targetPosition = new Vector2(targetPath.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_moveSpeed);

        if (Vector2.Distance(transform.position, targetPosition) < MinDistToTarget)
        {
            m_moveRight = !m_moveRight;
        }
    }

}
