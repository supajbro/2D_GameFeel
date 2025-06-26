using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [SerializeField] private Indicator m_indicatorPrefab;
    [SerializeField] private List<Indicator> m_indicators;
    private LevelRules m_lvlRules;

    private void Awake()
    {
        m_lvlRules = FindObjectOfType<LevelRules>();
        for (int i = 0; i < m_lvlRules.Enemies.Count; i++)
        {
            var clone = Instantiate(m_indicatorPrefab, transform);
            clone.Init(m_lvlRules.Enemies[i]);
            m_indicators.Add(clone);
        }
    }
}
