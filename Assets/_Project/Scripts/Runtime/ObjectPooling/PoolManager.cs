using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool system for Enemies, Lasers and Particles.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    // Pools are stored by prefab reference
    private Dictionary<GameObject, Queue<GameObject>> pools = new();

    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    [Header("Prewarm Pools")]
    [SerializeField] private PoolEntry[] prewarmEntries;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Optional: DontDestroyOnLoad(gameObject);

        // Prewarm pools
        foreach (var entry in prewarmEntries)
        {
            Prewarm(entry.prefab, entry.initialSize);
        }
    }

    /// <summary>
    /// Ensure pool exists and fill it with count objects.
    /// </summary>
    public void Prewarm(GameObject prefab, int count)
    {
        if (prefab == null) return;
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pools[prefab].Enqueue(obj);
        }
    }

    /// <summary>
    /// Spawn object from pool or create if empty.
    /// </summary>
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (prefab == null) return null;
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);

        return obj;
    }

    /// <summary>
    /// Return object back into its pool.
    /// </summary>
    public void Despawn(GameObject obj)
    {
        if (obj == null) return;

        // find prefab key
        GameObject prefabKey = FindPrefabKey(obj);
        if (prefabKey == null)
        {
            Destroy(obj); // fallback if prefab not tracked
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pools[prefabKey].Enqueue(obj);
    }

    // Try to match prefab key (best effort)
    private GameObject FindPrefabKey(GameObject obj)
    {
        foreach (var kvp in pools)
        {
            if (obj.name.StartsWith(kvp.Key.name))
                return kvp.Key;
        }
        return null;
    }
}
