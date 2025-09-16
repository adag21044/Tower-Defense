using UnityEngine;

[CreateAssetMenu(fileName = "WaveSet", menuName = "Data/Wave Set")]
public class WaveSet : ScriptableObject
{
    public WaveDefinition[] waves;
}
