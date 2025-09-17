using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
    }
    
    [Header("Pools (set in Inspector)")]
    [SerializeField] private PoolConfig[] poolConfigs;

    // Prefab -> Queue of pooled instances
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var config in poolConfigs)
        {
            if (config.prefab == null || config.initialSize <= 0) continue;
            Prewarm(config.prefab, config.initialSize);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }

        GameObject obj;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            if (parent) obj.transform.SetParent(parent, worldPositionStays: true);
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation, parent);
            // Remember original prefab
            var po = obj.GetComponent<PooledObject>();
            if (po == null) po = obj.AddComponent<PooledObject>();
            po.sourcePrefab = prefab;
        }
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null) return;

        var po = obj.GetComponent<PooledObject>();
        if (po == null || po.sourcePrefab == null)
        {
            // Fallback: if not poolable, destroy to avoid leaks
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform, worldPositionStays: false); // keep hierarchy tidy

        if (!pools.TryGetValue(po.sourcePrefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[po.sourcePrefab] = queue;
        }
        queue.Enqueue(obj);
    }

    // Optional prewarm
    public void Prewarm(GameObject prefab, int count, Transform parent = null)
    {
        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent ? parent : transform);
            var po = obj.GetComponent<PooledObject>() ?? obj.gameObject.AddComponent<PooledObject>();
            po.sourcePrefab = prefab;
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }
}
