using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooler : MonoBehaviour
{

    public static BulletPooler Instance;

    [SerializeField] private Bullet m_bulletPrefab;
    [SerializeField] private int m_bulletPoolAmount = 10;

    [SerializeField] private List<Bullet> m_bullets;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < m_bulletPoolAmount; i++)
        {
            Bullet bullet = Instantiate(m_bulletPrefab, transform);
            m_bullets.Add(bullet);
        }
    }

    public void GrabBullet(Transform shootPos, Vector3 dir)
    {
        foreach (var bullet in m_bullets)
        {
            if (bullet.IsActive)
            {
                continue;
            }
            bullet.Init(shootPos, dir);
            break;
        }
    }

}
