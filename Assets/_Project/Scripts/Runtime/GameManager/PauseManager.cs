using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // pause
        isPaused = true;
    }
    
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // resume
        isPaused = false;
    }
}
