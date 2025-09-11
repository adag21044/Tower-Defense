using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.05f; // float karşılaştırmada tolerans

    [Header("Path")]
    [SerializeField] private Transform[] waypoints; // Inspector’dan ayarlanacak
    private int idx = 0;

    private void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    private void Update()
    {
        if (waypoints == null || idx >= waypoints.Length - 1) return;

        Vector3 target = waypoints[idx + 1].position;
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) <= waypointThreshold)
        {
            idx++;
        }
    }
}
