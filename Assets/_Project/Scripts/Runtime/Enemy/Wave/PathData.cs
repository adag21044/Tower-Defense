using UnityEngine;

[CreateAssetMenu(fileName = "PathData", menuName = "Data/Path Definition")]
public class PathData : ScriptableObject
{
    [Tooltip("List of waypoint positions in world/local space")]
    public Vector3[] waypoints;
}
