using UnityEngine;
using System.Linq;

public class WaypointPath  : MonoBehaviour
{
    public Transform[] Points =>
        GetComponentsInChildren<Transform>(includeInactive: false)
        .Where(t => t != transform)
        .ToArray();
}