using UnityEngine;
using System;


public class EnemyMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.05f; 

    [Header("Path")]
    [SerializeField] private Transform[] waypoints; 
    private int idx = 0;
    public static event Action OnEnemyReachedEnd;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

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
            ToastManager.Instance.ShowToast("An enemy has reached the end!");
            OnEnemyReachedEnd?.Invoke();
            PoolManager.Instance.Despawn(gameObject); // pooling-friendly end
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
        {
            if (wp != null)
                Gizmos.DrawSphere(wp.position, 0.2f);
        }

        if (waypoints[0] != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoints[0].position, 0.3f);
        }

        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(waypoints[waypoints.Length - 1].position, Vector3.one * 0.4f);
        }
    }


    public void SetWaypoints(Vector3[] points)
    {
        if (points == null || points.Length == 0) return;

        waypoints = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            // runtime dummy waypoint objeleri oluştur
            GameObject wp = new GameObject("WP_" + i);
            wp.transform.position = points[i];
            waypoints[i] = wp.transform;
        }

        idx = 0;
        transform.position = waypoints[0].position;
    }


    private void OnEnable()
    {
        idx = 0; // reset path index on spawn
        if (waypoints != null && waypoints.Length > 0)
            transform.position = waypoints[0].position; // start at first waypoint
    }
}
