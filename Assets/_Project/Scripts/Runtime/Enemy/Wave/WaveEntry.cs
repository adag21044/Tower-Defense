using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public EnemyData enemy;      // Enemy tanımı (ScriptableObject)
    [Min(1)] public int count = 5;
    [Min(0.1f)] public float rate = 1f; // enemies per second
}
