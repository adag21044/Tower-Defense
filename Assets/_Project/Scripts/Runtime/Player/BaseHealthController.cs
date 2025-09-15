using UnityEngine;

public class BaseHealthController : MonoBehaviour
{
    [SerializeField] private int health = 200; // base daha dayanıklı olabilir
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private HealthBarUI baseHealthBarUI;

    private void Start()
    {
        baseHealthBarUI.UpdateHealthBar(health, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        baseHealthBarUI.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Debug.Log("Base destroyed!");
            // Oyun bitiş senaryosu burada
        }
    }

    private void OnEnable()
    {
        EnemyMover.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        EnemyMover.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void HandleEnemyReachedEnd()
    {
        TakeDamage(10);
        Debug.Log("Base took damage because an enemy reached the end.");
    }
}
