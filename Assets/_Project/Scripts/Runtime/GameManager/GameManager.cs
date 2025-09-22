using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Singleton")]
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BaseHealthController baseHealthController;
    [SerializeField] private GameObject retryPanel;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private WaveSpawner waveSpawner;
    public GameState CurrentState => currentState;


    public static event Action OnStart;
    public static event Action OnRetry;

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    [SerializeField] private GameState currentState = GameState.MainMenu;


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

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        pauseManager.InitalPauseGame();
    }


    private void Update()
    {
        // pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseManager != null)
            {
                pauseManager.TogglePause();
                currentState = pauseManager.isPaused ? GameState.Paused : GameState.Playing;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerController = FindAnyObjectByType<PlayerController>();
        baseHealthController = FindAnyObjectByType<BaseHealthController>();

        if (playerController != null)
            playerController.healthController.OnDeath += RetryPanel;

        if (baseHealthController != null)
            baseHealthController.OnBaseDestroyed += RetryPanel;
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Retry()
    {
        // reset enemy counter
        EnemyCounter.Reset();

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reset Game State
        ResetGame();
        OnRetry?.Invoke();
        Debug.Log("Retrying...");
        ToastManager.Instance.ShowToast("Game Restarted!");
    }


    private void ResetGame()
    {
        scoreManager.ResetScore();
        pauseManager.ResumeGame();
        if (retryPanel != null) retryPanel.SetActive(false);

        // Find references again
        playerController = FindAnyObjectByType<PlayerController>();
        baseHealthController = FindAnyObjectByType<BaseHealthController>();

        var waveSpawner = FindAnyObjectByType<WaveSpawner>();
        if (waveSpawner != null) EnemyCounter.Reset();
    }


    public void RetryPanel()
    {
        Debug.Log("Show Retry Panel");
        ToastManager.Instance.ShowToast("Game Over! Try Again?");
        retryPanel.SetActive(true);
        currentState = GameState.GameOver;

        if (waveSpawner != null)
        {
            waveSpawner.enabled = false; 
        }
    }


    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        pauseManager.ResumeGame();
        OnStart?.Invoke();
        currentState = GameState.Playing;
    }
}
