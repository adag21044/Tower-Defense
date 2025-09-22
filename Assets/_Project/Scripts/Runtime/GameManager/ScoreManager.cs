using UnityEngine;
using TMPro; // eğer UI’da da göstermek istiyorsan

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText; // optional UI label

    private void OnEnable()
    {
        // Subscribe to the event
        EnemyHealthController.OnEnemyDeath += IncreaseScore;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        EnemyHealthController.OnEnemyDeath -= IncreaseScore;
    }

    private void IncreaseScore()
    {
        score += 1;
        Debug.Log("Score: " + score);
        ToastManager.Instance.ShowToast("Enemy defeated! Score +1");

        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void ResetScore()
    {
        score = 0;
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public int GetScore()
    {
        return score;
    }
}
