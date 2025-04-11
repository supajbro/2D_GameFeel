using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [Header("Shoot Position")]
    [SerializeField] private Transform m_shootPos;

    [Header("Ammo")]
    [SerializeField] private int m_currentAmmo = 0;
    [SerializeField] private int m_maxAmmo = 30;

    [Header("Reload")]
    [SerializeField] private bool m_reloading = false;
    [SerializeField] private float m_reloadTime = 0f;
    [SerializeField] private float m_maxReloadTime = 1f;

    public void Shoot()
    {
        if(BulletPooler.Instance == null)
        {
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = m_shootPos.position.z;
        Vector3 direction = (mouseWorldPos - m_shootPos.position).normalized;

        BulletPooler.Instance.GrabBullet(m_shootPos, direction);
    }

}
