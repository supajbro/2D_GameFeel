using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    private Movement m_player;

    [Header("Is Shooting")]
    public bool isShooting = false;
    [SerializeField] private float m_nonShootingTime = 0f;
    [SerializeField] private float m_maxNonShootingTime = 1.0f;

    [Header("Shoot Position")]
    [SerializeField] private Transform m_shootPos;
    [SerializeField] private float m_knockbackPower = 50.0f;
    private Vector3 m_direction = Vector3.zero;

    [Header("Ammo")]
    [SerializeField] private int m_currentAmmo = 0;
    [SerializeField] private int m_maxAmmo = 30;

    [Header("Firerate")]
    [SerializeField] private float m_fireRate = 0.0f;
    [SerializeField] private float m_maxFireRate = 1.0f;

    [Header("Reload")]
    [SerializeField] private bool m_reloading = false;
    [SerializeField] private float m_reloadTime = 0f;
    [SerializeField] private float m_maxReloadTime = 1f;

    public float KnockbackPower => m_knockbackPower;
    public Vector3 Direction => m_direction;
    public int CurrentAmmo => m_currentAmmo;

    public void Init()
    {
        m_player = GetComponent<Movement>();
        m_currentAmmo = m_maxAmmo;
    }

    private void Update()
    {
        m_fireRate = (m_fireRate > 0) ? m_fireRate - Time.deltaTime : 0f;

        AutoReload();

        Reload();
    }

    public void Shoot()
    {
        if(BulletPooler.Instance == null)
        {
            return;
        }

        if(m_fireRate > 0)
        {
            return;
        }

        if(m_currentAmmo <= 0)
        {
            m_reloading = true;

            if (isShooting)
            {
                isShooting = false;
            }

            return;
        }

        m_fireRate = m_maxFireRate;
        m_currentAmmo--;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = m_shootPos.position.z;
        Vector3 direction = (mouseWorldPos - m_shootPos.position).normalized;
        m_direction = direction;

        BulletPooler.Instance.GrabBullet(m_shootPos, direction);

        m_player.PlayerUI.SetAmmoText(m_currentAmmo);
    }

    private void Reload()
    {
        if (!m_reloading)
        {
            return;
        }

        if (isShooting)
        {
            return;
        }

        m_reloadTime = (m_reloadTime < m_maxReloadTime) ? m_reloadTime + Time.deltaTime : m_maxReloadTime;

        if(m_reloadTime >= m_maxReloadTime)
        {
            m_reloading = false;
            m_reloadTime = 0.0f;
            m_currentAmmo = m_maxAmmo;
        }

        m_player.PlayerUI.SetAmmoText(m_currentAmmo);
    }

    private void AutoReload()
    {
        if (isShooting)
        {
            m_nonShootingTime = 0f;
            return;
        }

        if(m_currentAmmo == m_maxAmmo)
        {
            return;
        }

        m_nonShootingTime = (m_nonShootingTime < m_maxNonShootingTime) ? m_nonShootingTime + Time.deltaTime : m_maxNonShootingTime;
        if (m_nonShootingTime >= m_maxNonShootingTime)
        {
            m_reloading = true;
            m_nonShootingTime = 0f;
        }
    }

}
