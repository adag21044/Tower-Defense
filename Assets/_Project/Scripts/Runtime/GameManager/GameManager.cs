using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BaseHealthController baseHealthController;
    [SerializeField] private GameObject retryPanel;

    [System.Obsolete]
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Sahne yüklenince referansları yeniden bul
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [System.Obsolete]
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerController = FindObjectOfType<PlayerController>();
        baseHealthController = FindObjectOfType<BaseHealthController>();

        if (playerController != null)
            playerController.healthController.OnDeath += RetryPanel;

        if (baseHealthController != null)
            baseHealthController.OnBaseDestroyed += RetryPanel;
    }

    [System.Obsolete]
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
}
