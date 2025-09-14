using UnityEngine;
using System;


public class EnemyMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.05f; // float karşılaştırmada tolerans

    [Header("Path")]
    [SerializeField] private Transform[] waypoints; // Inspector’dan ayarlanacak
    private int idx = 0;
    public static event Action OnEnemyReachedEnd;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    private int index;

    private void Start()
    {
        waypoints[0].position = this.transform.position;

        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }


    public void MoveAlongPath()
    {
        if (waypoints == null || idx >= waypoints.Length - 1) return;

        Vector3 target = waypoints[idx + 1].position;
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) <= waypointThreshold)
        {
            idx++;
        }

        if (idx >= waypoints.Length - 1)
        {
            Debug.Log("Enemy reached the end of the path.");
            OnEnemyReachedEnd?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Waypointler arası çizgi
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // Başlangıç noktası
        if (waypoints[0] != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoints[0].position, 0.3f);
        }

        // Bitiş noktası
        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(waypoints[waypoints.Length - 1].position, Vector3.one * 0.4f);
        }
    }

    public void SetPath(Transform[] wps)
    {
        waypoints = wps;
        index = 0;

        if (waypoints != null && waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        if (index >= waypoints.Length - 1) return;

        Vector3 target = waypoints[index + 1].position;
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) <= waypointThreshold)
            index++;
    }
}
