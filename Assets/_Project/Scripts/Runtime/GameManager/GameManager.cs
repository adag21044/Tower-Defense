using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BaseHealthController baseHealthController;
    [SerializeField] private GameObject retryPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        playerController.healthController.OnDeath += RetryPanel;
        baseHealthController.OnBaseDestroyed += RetryPanel;
    }

    private void OnDisable()
    {
        playerController.healthController.OnDeath -= RetryPanel;
        baseHealthController.OnBaseDestroyed -= RetryPanel;
    }

    public void Retry()
    {
        // Implement retry logic, e.g., reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void RetryPanel()
    {
        // Show retry panel logic here
        Debug.Log("Show Retry Panel");

        retryPanel.SetActive(true);
    }
}