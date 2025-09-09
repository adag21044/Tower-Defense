using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int health = 100;

    // Events
    public event Action OnDeath;
    public event Action<int> OnHealthChanged;

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged?.Invoke(health);

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
}
