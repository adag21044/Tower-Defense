using System;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    private float currentHealth;
    
    
    private void OnEnable() { currentHealth = maxHealth; }  // reset on spawn from pool
    public void SetMaxHealth(int value) { maxHealth = value; currentHealth = maxHealth; }

    [SerializeField] private GameObject hitEffectPrefab;

    public static event Action OnEnemyDeath; // global event for score, currency, etc.

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        PoolManager.Instance.Prewarm(hitEffectPrefab, 20);
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
        PoolManager.Instance.Despawn(gameObject);

        if (hitEffectPrefab != null)
        {
            var effectObj = PoolManager.Instance.Spawn(
                hitEffectPrefab, 
                transform.position, 
                Quaternion.identity
            );
            var ps = effectObj.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        OnEnemyDeath?.Invoke(); // her durumda tetiklensin
    }
}