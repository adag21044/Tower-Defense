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
}
