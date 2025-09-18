using UnityEngine;
using System;

public class BaseHealthController : MonoBehaviour
{
    [SerializeField] private int health = 200; // base daha dayanıklı olabilir
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private HealthBarUI baseHealthBarUI;
    public event Action OnBaseDestroyed;

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
            ToastManager.Instance.ShowToast("Base destroyed! Game Over.");
            OnBaseDestroyed?.Invoke();
        }
    }

    private void OnEnable()
    {
        EnemyMover.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        OnBaseDestroyed += LogBaseDestroyed;
    }

    private void OnDisable()
    {
        EnemyMover.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        OnBaseDestroyed -= LogBaseDestroyed;
    }

    private void LogBaseDestroyed()
    {
        Debug.Log("Base has been destroyed! Game Over.");
        ToastManager.Instance.ShowToast("Base destroyed! Game Over.");
    }

    private void HandleEnemyReachedEnd(EnemyMover mover)
    {
        var enemy = mover.GetComponent<EnemyController>();
        if (enemy != null && enemy.enemyData != null)
        {
            TakeDamage(enemy.enemyData.damage);
            Debug.Log($"Base took {enemy.enemyData.damage} damage because {enemy.name} reached the end.");
            ToastManager.Instance.ShowToast($"Base took {enemy.enemyData.damage} damage!");
        }
    }
}
