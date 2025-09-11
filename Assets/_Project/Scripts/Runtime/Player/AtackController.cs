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
    [SerializeField] private Transform muzzle; // laser start point
    [SerializeField] private GameObject laserPrefab; // your laser prefab

    private float nextFireTime;

    private void Awake()
    {
        if (muzzle == null) muzzle = transform; // fallback
    }

    private void Update()
    {
        // find closest target
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

        if (closest == null) return;

        Vector3 dir = (closest.position - muzzle.position).normalized;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            ShootLaser(dir, closest);
        }
    }

    private void ShootLaser(Vector3 dir, Transform target)
    {
        // Spawn laser projectile from pool (or Instantiate if pool yoksa)
        GameObject laser = PoolManager.Instance != null
            ? PoolManager.Instance.Spawn(laserPrefab, muzzle.position, Quaternion.LookRotation(dir))
            : Instantiate(laserPrefab, muzzle.position, Quaternion.LookRotation(dir));

        // Configure projectile
        if (laser.TryGetComponent<LaserProjectile>(out var proj))
        {
            proj.target  = target;
            proj.damage  = (int)damagePerShot;
            proj.hitMask = hitMask;
            proj.homing  = true; // veya inspector’dan kontrol
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
