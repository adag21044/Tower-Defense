using UnityEngine;

public class AtackController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float range = 8f;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private LayerMask hitMask;

    [Header("Laser")]
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private float damagePerShot = 10f;
    [SerializeField] private Transform muzzle; 
    [SerializeField] private GameObject laserPrefab; 

    private float nextFireTime;

    private void Awake()
    {
        if (muzzle == null) muzzle = transform;
    }

    private void Update()
    {
        Transform target = FindClosestTarget();
        if (target == null) return;

        TryFireAtTarget(target);
    }

    // --- Yeni fonksiyonlar ---

    private Transform FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        Transform closest = null;
        float closestDistSqr = Mathf.Infinity;

        foreach (var c in hits)
        {
            if (!c.CompareTag(targetTag)) continue;
            float d2 = (c.transform.position - transform.position).sqrMagnitude;
            if (d2 < closestDistSqr)
            {
                closestDistSqr = d2;
                closest = c.transform;
            }
        }

        return closest;
    }

    private void TryFireAtTarget(Transform target)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);
        Vector3 dir = (target.position - muzzle.position).normalized;
        ShootLaser(dir, target);
    }

    private void ShootLaser(Vector3 dir, Transform target)
    {
        GameObject laser = PoolManager.Instance != null
            ? PoolManager.Instance.Spawn(laserPrefab, muzzle.position, Quaternion.LookRotation(dir))
            : Instantiate(laserPrefab, muzzle.position, Quaternion.LookRotation(dir));

        if (laser.TryGetComponent<LaserProjectile>(out var proj))
        {
            proj.target  = target;
            proj.damage  = (int)damagePerShot;
            proj.hitMask = hitMask;
            proj.homing  = true;
        }
        else
        {
            Debug.LogWarning("Laser prefab does not have LaserProjectile script!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
