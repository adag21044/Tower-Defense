using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    public bool isPaused = false;

    

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void InitalPauseGame()
    {
        Time.timeScale = 0f; // oyun durur
        isPaused = true;
        pausePanel.SetActive(false); // sadece menü görünsün, pause menüsü değil
    }
    
    public void PauseGame()
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
