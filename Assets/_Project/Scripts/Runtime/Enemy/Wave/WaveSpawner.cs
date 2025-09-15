using System.Collections;
using UnityEngine;
using TMPro;

/// Drop-in wave system tailored to your project.
/// - Uses EnemyData + PathData for path-following enemies
/// - Spawns via PoolManager (no GC thrash)
/// - Early-call next wave with a key
/// - Boss every N waves (5/10/15…)
/// - Optional "wait for clear" before next wave
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveEntry
    {
        public EnemyData enemy;     // ScriptableObject
        [Min(1)] public int count = 5;
        [Min(0.1f)] public float rate = 1f; // enemies per second
    }

    [System.Serializable]
    public class WaveDefinition
    {
        public string name;
        public WaveEntry[] entries;
        public bool waitForClear = false;         // wait all enemies to die/reach end?
        [Min(0f)] public float extraDelayAfter = 0f; // extra delay before countdown
    }

    [Header("Waves")]
    [SerializeField] private WaveDefinition[] waves;
    [SerializeField] private bool loopLastWave = false; // optional endless mode

    [Header("Timing")]
    [SerializeField] private float timeBetweenWaves = 4f;
    [SerializeField] private KeyCode earlyCallKey = KeyCode.Space;

    [Header("Boss")]
    [SerializeField] private EnemyData bossEnemy;  // optional boss type
    [SerializeField] private int bossEveryN = 5;   // 5,10,15...

    [Header("Difficulty Scaling / waveIndex >= 1")]
    [Tooltip("Extra HP per wave as a fraction (0.15 => +15% per wave)")]
    [SerializeField] private float hpMultiplierPerWave = 0.15f;
    [Tooltip("Extra speed per wave as a fraction (0.05 => +5% per wave)")]
    [SerializeField] private float speedMultiplierPerWave = 0.05f;

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints; // optional: if empty, uses pathData first point
    [SerializeField] private LayerMask enemyLayerForTag; // optional, for sanity checks

    [Header("UI (optional)")]
    [SerializeField] private TextMeshProUGUI waveLabel;

    private int waveIndex = 0;
    private float countdown;
    private bool counting = true;
    private Coroutine activeRoutine;

    private void Awake()
    {
        EnemyCounter.Reset();
        countdown = timeBetweenWaves;
        UpdateWaveLabel();
    }

    private void Update()
    {
        // Early call: only when counting between waves
        if (counting && Input.GetKeyDown(earlyCallKey))
        {
            countdown = 0f; // trigger next wave immediately
        }

        if (!counting) return;

        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            counting = false;
            if (activeRoutine == null)
            {
                var def = GetWaveDefinitionForIndex(waveIndex);
                activeRoutine = StartCoroutine(SpawnWaveRoutine(def, waveIndex));
            }
        }
    }

    private WaveDefinition GetWaveDefinitionForIndex(int idx)
    {
        if (waves == null || waves.Length == 0) return new WaveDefinition { name = $"Wave {idx+1}", entries = new WaveEntry[0] };
        if (idx < waves.Length) return waves[idx];
        // Past last wave
        return loopLastWave ? waves[waves.Length - 1] : waves[waves.Length - 1];
    }

    private IEnumerator SpawnWaveRoutine(WaveDefinition def, int currentWaveNumber)
    {
        // Spawn each entry batch
        foreach (var entry in def.entries)
        {
            if (entry == null || entry.enemy == null) continue;

            for (int i = 0; i < entry.count; i++)
            {
                SpawnOne(entry.enemy, currentWaveNumber);
                yield return new WaitForSeconds(1f / Mathf.Max(0.01f, entry.rate));
            }
        }

        // Boss check (5/10/15…)
        if (bossEnemy != null && bossEveryN > 0 && ((currentWaveNumber + 1) % bossEveryN == 0))
        {
            SpawnOne(bossEnemy, currentWaveNumber, isBoss:true);
        }

        // Optionally wait until all enemies are cleared (death or reached end)
        if (def.waitForClear)
        {
            while (EnemyCounter.ActiveCount > 0)
                yield return null;
        }

        // Extra delay & return to counting
        if (def.extraDelayAfter > 0f)
            yield return new WaitForSeconds(def.extraDelayAfter);

        activeRoutine = null;

        // Prepare next wave
        waveIndex++;
        UpdateWaveLabel();

        // If not looping and past last wave, stop counting
        if (!loopLastWave && waveIndex >= waves.Length)
            yield break;

        countdown = timeBetweenWaves;
        counting = true;
    }

    private void SpawnOne(EnemyData data, int currentWaveNumber, bool isBoss = false)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("[WaveDirector] EnemyData or prefab missing.");
            return;
        }

        // Determine spawn position
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            var t = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = t.position;
            spawnRot = t.rotation;
        }
        else if (data.pathData != null && data.pathData.waypoints != null && data.pathData.waypoints.Length > 0)
        {
            spawnPos = data.pathData.waypoints[0]; // spawn at first path point
        }

        // Spawn via pool
        GameObject go = (PoolManager.Instance != null)
            ? PoolManager.Instance.Spawn(data.prefab, spawnPos, spawnRot, null)
            : Instantiate(data.prefab, spawnPos, spawnRot);

        // Ensure tag for targeting
        go.tag = "Enemy";

        // Track lifetime count
        if (!go.TryGetComponent<EnemyCounter>(out _))
            go.AddComponent<EnemyCounter>();

        // Apply base EnemyData
        if (go.TryGetComponent<EnemyController>(out var ctrl))
        {
            ctrl.Apply(data);
        }
        else
        {
            Debug.LogWarning("[WaveDirector] Spawned object has no EnemyController.");
        }

        // Difficulty scaling (per wave)
        float hpScale = 1f + Mathf.Max(0f, hpMultiplierPerWave) * currentWaveNumber;
        float spdScale = 1f + Mathf.Max(0f, speedMultiplierPerWave) * currentWaveNumber;

        // Boss bump
        if (isBoss)
        {
            hpScale *= 3f;
            spdScale *= 1.2f;
        }

        // Apply scaled stats directly on components
        if (go.TryGetComponent<EnemyHealthController>(out var eh))
        {
            int scaledMax = Mathf.RoundToInt(Mathf.Max(1f, data.maxHealth) * hpScale);
            eh.SetMaxHealth(scaledMax);
        }
        if (go.TryGetComponent<EnemyMover>(out var mover))
        {
            mover.MoveSpeed = Mathf.Max(0.01f, data.moveSpeed) * spdScale;
            // path injected already by EnemyController.Apply(data) -> EnemyMover.SetWaypoints
        }
    }

    private void UpdateWaveLabel()
    {
        if (!waveLabel) return;
        waveLabel.text = $"Wave {waveIndex + 1}";
    }
}

/// Tracks how many enemies are currently alive/active (works with pooling)
public class EnemyCounter : MonoBehaviour
{
    public static int ActiveCount { get; private set; }
    private void OnEnable() { ActiveCount++; }
    private void OnDisable() { if (ActiveCount > 0) ActiveCount--; }
    public static void Reset() { ActiveCount = 0; }
}
