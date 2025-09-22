using System;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBarUI playerHealthBarUI;

    public event Action OnDeath;
    public event Action<int> OnHealthChanged;
    [SerializeField] private bool canTakeDamage = true;

    private void Start()
    {
        playerHealthBarUI.UpdateHealthBar(health, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        playerHealthBarUI.UpdateHealthBar(health, maxHealth);
        OnHealthChanged?.Invoke(health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        ToastManager.Instance.ShowToast("You have died!");
        Debug.Log("Player has died.");
        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        health = maxHealth;
        playerHealthBarUI.UpdateHealthBar(health, maxHealth);
        OnHealthChanged?.Invoke(health);
    }
    
    public void KillPlayer()
    {
        if (health > 0)
        {
            health = 0;
            playerHealthBarUI.UpdateHealthBar(health, maxHealth);
            OnHealthChanged?.Invoke(health);
            Die();
        }
    }
}
