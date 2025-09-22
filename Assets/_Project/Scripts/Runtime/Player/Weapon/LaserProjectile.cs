using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] public float speed = 25f;
    [SerializeField] public float maxLife = 2f;
    [SerializeField] public bool homing = true;

    [HideInInspector] public Transform target;
    [HideInInspector] public float damage;
    [HideInInspector] public LayerMask hitMask;

    private float lifeTimer;
    [SerializeField] private WeaponConfig weaponConfig;

    private void OnEnable()
    {
        if (weaponConfig != null)
        {
            damage = weaponConfig.damage;
            speed = weaponConfig.projectileSpeed;
            maxLife = weaponConfig.range / weaponConfig.projectileSpeed;
        }
        
        Debug.Log($"[Laser] Speed set to {speed}");
        Debug.Log($"[Laser] MaxLife set to {maxLife}");
        Debug.Log($"[Laser] Damage set to {damage}");
        ToastManager.Instance.ShowToast("Laser setted up!");
        // Reset transient state whenever pulled from pool
        lifeTimer = 0f;
        // tip: reset trail/particle if needed (clear trail, restart particle, etc.)
        var trail = GetComponent<TrailRenderer>();
        if (trail) trail.Clear();
    }

    private void OnDisable()
    {
        // Optional: clear refs to avoid holding onto dead targets
        target = null;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLife)
        {
            PoolManager.Instance.Despawn(gameObject);
            return;
        }

        // Movement direction
        Vector3 dir = (homing && target != null)
            ? (target.position - transform.position).normalized
            : transform.forward;

        float step = speed * Time.deltaTime;

        // Prevent tunneling (short ray ahead)
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, step + 0.1f, hitMask, QueryTriggerInteraction.Ignore))
        {
            var health = hit.collider.GetComponent<EnemyHealthController>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // TODO: spawn hit VFX via pool if you want
            PoolManager.Instance.Despawn(gameObject);
            return;
        }

        // Move forward
        transform.position += dir * step;

        // Align visual
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
