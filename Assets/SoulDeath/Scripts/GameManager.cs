using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsPaused => Instance != null && Instance.paused;
    public static bool IsGameOver => Instance != null && Instance.gameOver;

    public string CurrentObjective { get; private set; } = "Find a way out of the city.";
    public int BossesDefeated { get; private set; }

    private bool paused;
    private bool gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetPaused(false);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
        Time.timeScale = 1f;
    }

    public void SetObjective(string objective)
    {
        CurrentObjective = objective;
    }

    public void SetPaused(bool value)
    {
        paused = value;
        Time.timeScale = value ? 0f : 1f;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }

    public void SetGameOver()
    {
        gameOver = true;
        SetPaused(true);
    }

    public void RegisterBossDefeat()
    {
        BossesDefeated++;
        if (BossesDefeated == 1)
            SetObjective("One Card Guard remains.");
        else
            SetObjective("The arena gate is open. Find the exit portal.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        paused = false;
        gameOver = false;
        Time.timeScale = 1f;
        BossesDefeated = 0;
        CurrentObjective = SceneTransitionManager.GetObjective(scene.name);

        PortalController.ClearPrompt();
        bool isMenu = scene.name == SceneTransitionManager.MainMenu || scene.name == SceneTransitionManager.Ending;
        Cursor.lockState = isMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenu;
    }
}
