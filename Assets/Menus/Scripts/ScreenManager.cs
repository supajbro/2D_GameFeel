using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;
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

    [System.Serializable]
    public enum ScreenType
    {
        MenuScreen = 0,
        GameScreen,
        PauseScreen,
        DeathScreen,
        ALLSCREENS
    }

    [Header("Screens")]
    [SerializeField] private DeathScreen m_deathScreenPrefab;
    private DeathScreen m_deathScreen;
    public DeathScreen DeathScreen
    {
        get => m_deathScreen;
        set => m_deathScreen = value;
    }

    public void SpawnGameScreens()
    {
        m_deathScreen = Instantiate(m_deathScreenPrefab, transform);
    }

    public void Open(ScreenType screen)
    {
        switch(screen)
        {
            case ScreenType.MenuScreen:
                break;

            case ScreenType.GameScreen:
                break;

            case ScreenType.PauseScreen:
                break;

            case ScreenType.DeathScreen:
                m_deathScreen?.Open();
                break;
        }
    }

    public void Close(ScreenType screen)
    {
        switch (screen)
        {
            case ScreenType.MenuScreen:
                break;

            case ScreenType.GameScreen:
                break;

            case ScreenType.PauseScreen:
                break;

            case ScreenType.DeathScreen:
                m_deathScreen?.Close();
                break;
        }
    }
}
