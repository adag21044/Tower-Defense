using UnityEngine;

public class DebugGame : MonoBehaviour
{
    [System.Obsolete]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ShowWaypointsandGizmos();
        if (Input.GetKeyDown(KeyCode.F2)) NukeAllEnemies();
        if (Input.GetKeyDown(KeyCode.F3)) ShowSpawnPoints();
    }

    // Waypoint ve path çizimlerini görmek
    [System.Obsolete]
    private void ShowWaypointsandGizmos()
    {
        // Sahnedeki tüm EnemyMover’ları bul
        var movers = FindObjectsOfType<EnemyMover>();
        foreach (var mover in movers)
        {
            // EnemyMover zaten OnDrawGizmos ile çiziyor
            // Ama burada debug log atabilirsin
            Debug.Log($"[DebugGame] EnemyMover {mover.name} has {mover.transform.childCount} waypoints");
        }

        // Ayrıca sahnedeki Path objelerini bulup çizdirebilirsin
        var paths = FindObjectsOfType<EnemyPath>();
        foreach (var path in paths)
        {
            var pts = path.Points;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                Debug.DrawLine(pts[i].position, pts[i + 1].position, Color.magenta, 2f);
            }
        }
    }

    // Tüm düşmanları anında yok etmek
    [System.Obsolete]
    private void NukeAllEnemies()
    {
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            // Havuz sistemine geri gönder
            PoolManager.Instance.Despawn(enemy.gameObject);
        }

        Debug.Log($"[DebugGame] Nuked {enemies.Length} enemies.");
    }

    // Spawn noktalarını göstermek
    [System.Obsolete]
    private void ShowSpawnPoints()
    {
        var spawners = FindObjectsOfType<WaveSpawner>();
        foreach (var spawner in spawners)
        {
            // Spawner’daki wave’lerdeki spawn transformlarını çek
            var wavesField = spawner.GetType()
                .GetField("waves", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (wavesField != null)
            {
                var waves = wavesField.GetValue(spawner) as System.Array;
                if (waves != null)
                {
                    foreach (var wave in waves)
                    {
                        var enemyField = wave.GetType().GetField("enemy");
                        if (enemyField != null)
                        {
                            var enemyPrefab = enemyField.GetValue(wave) as Transform;
                            if (enemyPrefab != null)
                            {
                                Debug.DrawRay(enemyPrefab.position, Vector3.up * 2f, Color.yellow, 2f);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("[Debug] Spawn points drawn in Scene view.");
    }
}
