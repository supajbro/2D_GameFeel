using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [Header("Ammo")]
    [SerializeField] private int m_currentAmmo = 0;
    [SerializeField] private int m_maxAmmo = 30;

    [Header("Reload")]
    [SerializeField] private bool m_reloading = false;
    [SerializeField] private float m_reloadTime = 0f;
    [SerializeField] private float m_maxReloadTime = 1f;

}
