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

    [System.Obsolete]
    private void ShowWaypointsandGizmos()
    {
        // Find all EnemyMover instances in the scene
        var movers = FindObjectsOfType<EnemyMover>();
        foreach (var mover in movers)
        {
            Debug.Log($"[DebugGame] EnemyMover {mover.name} has {mover.transform.childCount} waypoints");
        }

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

    [System.Obsolete]
    private void NukeAllEnemies()
    {
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            // return to pool instead of Destroy
            PoolManager.Instance.Despawn(enemy.gameObject);
        }

        Debug.Log($"[DebugGame] Nuked {enemies.Length} enemies.");
    }

    [System.Obsolete]
    private void ShowSpawnPoints()
    {
        var spawners = FindObjectsOfType<WaveSpawner>();
        foreach (var spawner in spawners)
        {
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
