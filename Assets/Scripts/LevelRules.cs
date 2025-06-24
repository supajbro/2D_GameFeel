using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRules : MonoBehaviour
{
    [System.Serializable]
    public enum LevelType
    {
        KillAll = 0, // Need to kill all enemies in the level
        CollectAll, // Need to collect all the coins in that level
        KillAndCollectAll, // Need to kill all enemies and collect all the coins in that level
        EndOfLevelTypes
    }
    [SerializeField] private LevelType m_lvlType;

    [Header("Game Prefabs")]
    [SerializeField] private Player m_playerPrefab;

    private Player m_currentPlayer;
    public Player CurrentPlayer => m_currentPlayer;

    private void Awake()
    {
        Instantiate(m_playerPrefab.gameObject);
        m_currentPlayer = m_playerPrefab.GetComponent<Player>();
    }

}
