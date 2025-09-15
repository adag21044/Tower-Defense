using UnityEngine;
using System.Linq;

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
        var enemyPath = FindObjectOfType<EnemyPath>();
        if (enemyPath != null)
            cachedWaypoints = enemyPath.Points;
        else
            Debug.LogError("Path not found!");


    }

    private void Start()
    {
        if (waves == null) return;

        var uniques = waves
            .Where(w => w != null && w.enemyTypes != null)
            .SelectMany(w => w.enemyTypes)
            .Where(d => d != null && d.prefab != null)
            .Select(d => d.prefab)
            .Distinct();

        foreach (var p in uniques)
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.Prewarm(p, 10);
            else
                Debug.LogError("PoolManager.Instance is null in Start!");
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
        controller.Apply(data); // sadece EnemyData veriyoruz
    }


        currentWave.noOfEnemies--;
        nextSpawnTime = Time.time + currentWave.spawnInterval;
        if (currentWave.noOfEnemies <= 0) canSpawn = false;
    }
}
