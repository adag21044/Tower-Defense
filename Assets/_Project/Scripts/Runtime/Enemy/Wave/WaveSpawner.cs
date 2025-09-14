using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Wave[] waves;
    [SerializeField] private float countdown = 2f;

    private int currentWaveIndex = 0;
    private bool isReadyToCountdown;

    private void Start()
    {
        isReadyToCountdown = true;

        
    }

    private void Update()
    {   
        if (!isReadyToCountdown)
            countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            isReadyToCountdown = false;
            countdown = waves[currentWaveIndex].timeToNextWave;
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        var wave = waves[currentWaveIndex];

        // Safety checks
        if (wave.path == null || wave.path.Points == null || wave.path.Points.Length < 2)
        {
            Debug.LogError("[WaveSpawner] Path is missing or has less than 2 points.");
            yield break;
        }

        foreach (var enemyPrefab in wave.enemies)
        {
            // Instantiate as world object (not child)
            GameObject go = Instantiate(enemyPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);

            // Inject path
            var mover = go.GetComponent<EnemyMover>();
            if (mover == null)
            {
                Debug.LogError("[WaveSpawner] Enemy prefab has no EnemyMover component.");
            }
            else
            {
                mover.SetPath(wave.path.Points);
            }

            yield return new WaitForSeconds(wave.timeToNextEnemy);
        }

        // Prepare next wave
        countdown = wave.timeToNextWave;
        currentWaveIndex = Mathf.Min(currentWaveIndex + 1, waves.Length - 1);
    }
}


[System.Serializable]
public class Wave
{
    public EnemyController[] enemies;  // Prefab references from Project (NOT scene)
    public WaypointPath path;                  // Scene object (Path root with children)
    public float timeToNextEnemy = 0.5f;
    public float timeToNextWave = 5f;
}


