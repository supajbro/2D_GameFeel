using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI m_currentAmmo;

    public void SetAmmoText(int amount)
    {
        m_currentAmmo.text = amount.ToString();
    }

}
