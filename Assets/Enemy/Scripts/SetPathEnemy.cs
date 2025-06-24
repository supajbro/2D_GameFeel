using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPathEnemy : Enemy, IEnemy, IHealth
{
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
        if (!CanMove)
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
        if (!CanMove)
        {
            return;
        }

        Transform targetPath = (m_moveRight) ? m_rightPath : m_leftPath;

        Vector2 targetPosition = new Vector2(targetPath.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed);

        if (Vector2.Distance(transform.position, targetPosition) < MinDistToTarget)
        {
            m_moveRight = !m_moveRight;
        }
    }
}
