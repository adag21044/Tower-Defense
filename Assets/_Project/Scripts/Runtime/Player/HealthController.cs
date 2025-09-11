using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [Header("UI Reference")]
    [SerializeField] private HealthBarUI healthBarUI;

    public event Action OnDeath;
    public event Action<int> OnHealthChanged;


    private void Start()
    {
        // Initialize UI at start
        healthBarUI.UpdateHealthBar(health, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Update UI
        healthBarUI.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        OnDeath?.Invoke();
    }
    
    public void ResetHealth()
    {
        health = maxHealth;
        healthBarUI.UpdateHealthBar(health, maxHealth);
        OnHealthChanged?.Invoke(health);
    }
}
