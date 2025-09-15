using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int noOfEnemies;
        public EnemyData[] enemyTypes;   // ScriptableObject bazlı düşman tipleri
        public float spawnInterval;
    }

    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform spawnPoint; // İlk waypoint başlangıç noktası
    private Wave currentWave;
    private int currentWaveIndex = 0;
    private float nextSpawnTime;
    private bool canSpawn = true;
    [SerializeField] private Transform[] pathWaypoints;
    private Transform[] cachedWaypoints;

    private void Awake()
    {
        // Sahnede Path objesini bul ve waypointleri cachele
        var path = FindObjectOfType<EnemyPath>();
        if (path != null)
        {
            cachedWaypoints = path.Points;
        }
        else
        {
            Debug.LogError("Path object not found in scene!");
        }
    }

    private void Update()
    {
        if (currentWaveIndex >= waves.Length) return;

        currentWave = waves[currentWaveIndex];
        HandleSpawning();

        // Aktif düşman sayısını kontrol et
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !canSpawn)
        {
            currentWaveIndex++;
            canSpawn = true;

            if (currentWaveIndex >= waves.Length)
            {
                Debug.Log("All waves finished!");
            }
            else
            {
                Debug.Log($"Wave {currentWaveIndex} started: {waves[currentWaveIndex].waveName}");
            }
        }
    }

    private void HandleSpawning()
    {
        if (!canSpawn || Time.time < nextSpawnTime) return;

        EnemyData data = currentWave.enemyTypes[Random.Range(0, currentWave.enemyTypes.Length)];

        GameObject enemy = PoolManager.Instance.Spawn(data.prefab, spawnPoint.position, Quaternion.identity);

        var controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.SetData(data);

            // Waypoints atama
            var mover = enemy.GetComponent<EnemyMover>();
            if (mover != null)
            {
                mover.SetWaypoints(pathWaypoints);
            }
        }

        currentWave.noOfEnemies--;
        nextSpawnTime = Time.time + currentWave.spawnInterval;

        if (currentWave.noOfEnemies <= 0)
        {
            canSpawn = false;
        }
    }
}
