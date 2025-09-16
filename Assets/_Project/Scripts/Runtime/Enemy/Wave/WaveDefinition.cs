using UnityEngine;

[CreateAssetMenu(fileName = "WaveDefinition", menuName = "Data/Wave Definition")]
public class WaveDefinition : ScriptableObject
{
    public string waveName = "Wave";
    public WaveEntry[] entries;
    public bool waitForClear = false;         
    [Min(0f)] public float extraDelayAfter = 0f;
}
