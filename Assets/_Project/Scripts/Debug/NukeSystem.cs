using UnityEngine;

public class NukeSystem : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ActivateNuke();
        }
    }

    private void ActivateNuke()
    {
        Debug.Log("Nuke activated! All enemies destroyed.");
        ToastManager.Instance.ShowToast("Nuke activated! All enemies destroyed.");

        foreach (var enemy in waveSpawner.enemiesInScene)
        {
            if (enemy == null) continue;

            var health = enemy.GetComponent<EnemyHealthController>();
            if (health != null)
            {
                // force kill via health system (triggers OnEnemyDeath, VFX, pooling)
                health.TakeDamage(float.MaxValue);
            }
            else
            {
                // fallback: despawn directly if something missing
                PoolManager.Instance.Despawn(enemy);
            }
        }

        // Sayaç sıfırlamak istersen:
        EnemyCounter.Reset();
    }
}
