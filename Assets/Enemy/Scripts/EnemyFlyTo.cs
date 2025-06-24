using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyTo : Enemy, IEnemy, IHealth
{

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

    public override void AttackUpdate(Player player)
    {
        player.OnDamage.Invoke(Damage);
    }

    public override void MoveUpdate()
    {
        if (!CanMove)
        {
            return;
        }

        Vector2 targetPosition = new Vector2(m_target.position.x, m_target.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed);
    }
}
