using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Set (ScriptableObject)")]
    [SerializeField] private WaveSet waveSet;          // Use the external SO types
    [SerializeField] private bool loopLastWave = false;

    [Header("Timing")]
    [SerializeField] private float timeBetweenWaves = 4f;
    [SerializeField] private KeyCode earlyCallKey = KeyCode.Space;

    [Header("Boss")]
    [SerializeField] private EnemyData bossEnemy;      // Optional boss type (SO)
    [SerializeField] private int bossEveryN = 5;       // 5,10,15...

    [Header("Scaling per wave (>=1)")]
    [Tooltip("Extra HP per wave as a fraction (0.15 => +15% per wave)")]
    [SerializeField] private float hpMultiplierPerWave = 0.15f;
    [Tooltip("Extra speed per wave as a fraction (0.05 => +5% per wave)")]
    [SerializeField] private float speedMultiplierPerWave = 0.05f;

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;  // If empty, uses first PathData point

    [Header("UI (optional)")]
    [SerializeField] private TextMeshProUGUI waveLabel;
    [SerializeField] private TextMeshProUGUI statusLabel; // countdown / enemies left

    private int waveIndex;
    private float countdown;
    public bool counting = true;

    private Coroutine activeRoutine;

    private void Awake()
    {

        waveIndex = 0;
        countdown = timeBetweenWaves;
        UpdateWaveLabel();
        UpdateStatusLabel(force: true);
    }

    private void Update()
    {
        // Eğer oyun bitti ise tamamen çık
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
            return;

        // Normal flow
        if (counting && Input.GetKeyDown(earlyCallKey))
            CallNextWaveEarly();

        if (!counting)
        {
            UpdateStatusLabel(); 
            return;
        }

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

        UpdateStatusLabel();
    }


    private void OnEnable()
    {
        GameManager.OnRetry += HandleRetry;
    }

    private void OnDisable()
    {
        GameManager.OnRetry -= HandleRetry;
    }

    private void HandleRetry()
    {
        // Tüm coroutine’leri durdur
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        // Spawner tamamen dursun
        counting = false;
        enabled = false; // WaveSpawner update loop’u artık çalışmaz

        Debug.Log("[WaveSpawner] Stopped due to Retry.");
    }


    private WaveDefinition GetWaveDefinitionForIndex(int idx)
    {
        if (waveSet == null || waveSet.waves == null || waveSet.waves.Length == 0)
            return null;

        if (idx < waveSet.waves.Length) return waveSet.waves[idx];
        return loopLastWave ? waveSet.waves[waveSet.waves.Length - 1] : null;
    }

    private IEnumerator SpawnWaveRoutine(WaveDefinition def, int currentWaveNumber)
    {
        if (def == null) yield break;

        if (waveLabel) waveLabel.text = $"{def.waveName} ({currentWaveNumber + 1})";

        if (def.entries != null)
        {
            foreach (var entry in def.entries)
            {
                if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
                    yield break; // oyun bittiyse coroutine iptal

                int spawnCount = Mathf.Max(0, entry.count);
                float spawnRate = Mathf.Max(0.01f, entry.rate);

                for (int i = 0; i < spawnCount; i++)
                {
                    SpawnOne(entry.enemy, currentWaveNumber, isBoss: false);
                    yield return new WaitForSeconds(1f / spawnRate);
                }
            }
        }

        // Boss check (5/10/15…)
        if (bossEnemy != null && bossEveryN > 0 && ((currentWaveNumber + 1) % bossEveryN == 0))
            SpawnOne(bossEnemy, currentWaveNumber, isBoss: true);

        // Optional: wait for clear
        if (def.waitForClear)
        {
            while (EnemyCounter.ActiveCount > 0)
                yield return null;
        }

        // Extra delay after wave
        if (def.extraDelayAfter > 0f)
            yield return new WaitForSeconds(def.extraDelayAfter);

        // Prepare next
        activeRoutine = null;
        waveIndex++;

        if (!loopLastWave && waveSet != null && waveIndex >= waveSet.waves.Length)
        {
            if (waveLabel) waveLabel.text = "All Waves Completed";
            if (statusLabel) statusLabel.text = "Enemies: 0";
            yield break;
        }

        countdown = timeBetweenWaves;
        counting = true;
        UpdateWaveLabel();
        UpdateStatusLabel(force: true);
    }

    private void SpawnOne(EnemyData data, int currentWaveNumber, bool isBoss)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("[WaveSpawner] EnemyData or prefab missing.");
            return;
        }

        // Decide spawn transform
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
            // Spawn at first path point from PathData (SO)
            spawnPos = data.pathData.waypoints[0];
        }

        // Spawn via pool (fallback to Instantiate)
        GameObject go = (PoolManager.Instance != null)
            ? PoolManager.Instance.Spawn(data.prefab, spawnPos, spawnRot, null)
            : Instantiate(data.prefab, spawnPos, spawnRot);

        // Ensure tag for targeting systems
        go.tag = "Enemy";

        // Track active enemies even with pooling
        if (!go.TryGetComponent<EnemyCounter>(out _))
            go.AddComponent<EnemyCounter>();

        // Apply base stats/path through your EnemyController if available
        if (go.TryGetComponent<EnemyController>(out var ctrl))
        {
            // Expected: ctrl.Apply(...) sets health, speed and injects PathData to mover
            ctrl.Apply(data);
        }

        // Per-wave scaling (and boss bump)
        float hpScale = 1f + Mathf.Max(0f, hpMultiplierPerWave) * currentWaveNumber;
        float spdScale = 1f + Mathf.Max(0f, speedMultiplierPerWave) * currentWaveNumber;
        if (isBoss) { hpScale *= 3f; spdScale *= 1.2f; }

        if (go.TryGetComponent<EnemyHealthController>(out var eh))
        {
            int scaledMax = Mathf.RoundToInt(Mathf.Max(1f, data.maxHealth) * hpScale);
            eh.SetMaxHealth(scaledMax);
        }

        if (go.TryGetComponent<EnemyMover>(out var mover))
        {
            mover.MoveSpeed = Mathf.Max(0.01f, data.moveSpeed) * spdScale;

            // If your EnemyController didn't inject the path and your mover supports Vector3[]:
            var setWaypoints = mover.GetType().GetMethod("SetWaypoints", new[] { typeof(Vector3[]) });
            if (setWaypoints != null && data.pathData != null)
                setWaypoints.Invoke(mover, new object[] { data.pathData.waypoints });
        }
    }

    private void UpdateWaveLabel()
    {
        if (!waveLabel) return;
        int total = (waveSet && waveSet.waves != null) ? waveSet.waves.Length : 0;
        waveLabel.text = $"Wave {Mathf.Clamp(waveIndex + 1, 1, Mathf.Max(1, total))}";
    }

    /// Public API to call next wave early (bind to UI Button)
    public void CallNextWaveEarly()
    {
        if (!counting) return;
        countdown = 0f;
        UpdateStatusLabel(force: true); // reflect the skip immediately
        Debug.Log("[WaveSpawner] Next wave called early.");
        ToastManager.Instance.ShowToast("Next wave incoming!");
    }

    private void UpdateStatusLabel(bool force = false)
    {
        if (!statusLabel) return;

        if (counting || force)
            statusLabel.text = $"Wave: {waveIndex + 1}\nCountdown: {Mathf.Max(0f, countdown):0.0}s";
        else
            statusLabel.text = $"Wave: {waveIndex + 1}\nEnemies: {EnemyCounter.ActiveCount}";
    }
    


}


