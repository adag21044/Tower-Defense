using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    private float currentHealth;
    
    
    private void OnEnable() { currentHealth = maxHealth; }  // reset on spawn from pool
    public void SetMaxHealth(int value) { maxHealth = value; currentHealth = maxHealth; }


    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        
        if (PoolManager.Instance != null)
            PoolManager.Instance.Despawn(gameObject);
        else
            Destroy(gameObject);
    }
}