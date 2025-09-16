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
    

    public static event Action OnStart;


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
                pauseManager.TogglePause();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetryPanel()
    {
        Debug.Log("Show Retry Panel");
        retryPanel.SetActive(true);
    }
    
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        pauseManager.ResumeGame(); 
        OnStart?.Invoke();
    }
}
