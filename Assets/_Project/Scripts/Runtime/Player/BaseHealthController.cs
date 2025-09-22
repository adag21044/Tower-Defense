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

    private void HandleEnemyReachedEnd()
    {
        TakeDamage(50);
        Debug.Log("Base took damage because an enemy reached the end.");
        ToastManager.Instance.ShowToast("Base took 50 damage!");
    }

    // for testin purposes
    [ContextMenu("Set Health Infinite")]
    public void SetHealthInfinite()
    {
        health = int.MaxValue;
        baseHealthBarUI.UpdateHealthBar(health, maxHealth);
    }
}
