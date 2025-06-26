using System;
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

    [Header("Player")]
    [SerializeField] private Vector2 m_initPosition = Vector2.zero;
    private Player m_currentPlayer;
    public Player CurrentPlayer => m_currentPlayer;

    [Header("Enemies")]
    private List<Enemy> m_enemies = new();
    public List<Enemy> Enemies => m_enemies;
    public void AddEnemy(Enemy enemy)
    {
        m_enemies.Add(enemy);
    }

    private void Awake()
    {
        var player = Instantiate(m_playerPrefab.gameObject);
        m_currentPlayer = player.GetComponent<Player>();
        m_currentPlayer.Init(m_initPosition);
    }

    private void Start()
    {
        ScreenManager.Instance.SpawnGameScreens();
        InitButtons();
    }

    private void InitButtons()
    {
        ScreenManager.Instance.DeathScreen.RestartAction += RestartLvl;
        ScreenManager.Instance.DeathScreen.RestartBtn.onClick.AddListener(() =>
        {
            ScreenManager.Instance.DeathScreen.RestartAction?.Invoke();
        });
    }

    public void RestartLvl()
    {
        m_currentPlayer.ReInit(m_initPosition);

        foreach (var enemy in m_enemies)
        {
            enemy.ReInit();
        }

        ScreenManager.Instance.Close(ScreenManager.ScreenType.DeathScreen);
    }
}
