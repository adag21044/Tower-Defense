using UnityEngine;
using DG.Tweening; // DOTween 

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

    [Header("Melee")]
    [SerializeField] private GameObject meleeObject;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeCooldown = 1f;

    private float nextFireTime;
    private Tween meleeTween; // aktif tween referansı

    [Header("Test")]
    [SerializeField] private bool enableLaser = true;

    [SerializeField] private WeaponConfig weaponConfig;
    [SerializeField] private WeaponConfig[] weaponTypes;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (muzzle == null) muzzle = transform;

        // önce default silahı seç
        weaponConfig = weaponTypes[0];
        Debug.Log($"[Attack] Weapon set to {weaponConfig.name}");

        // sonra değerleri atayalım
        range = weaponConfig.range;
        fireRate = weaponConfig.fireRate;
        damagePerShot = weaponConfig.damage;
    }


    private void Update()
    {
        HandleWeaponHotkeys(); // <- ÖNCE BUNU ÇALIŞTIR

        Transform target = FindClosestTarget();
        if (target == null)
        {
            StopMeleeAnimation();
            return;              // <- Erken çıkış bundan sonra
        }

        TryFireAtTarget(target);
    }

    private void HandleWeaponHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            SetWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            SetWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            SetWeapon(2);
    }

    private void SetWeapon(int index)
    {
        if (index < 0 || index >= weaponTypes.Length) return;

        weaponConfig = weaponTypes[index];
        ApplyWeaponConfig();

        Debug.Log($"[Attack] Weapon switched to {weaponConfig.name}");
        ToastManager.Instance.ShowToast($"Weapon switched to {weaponConfig.name}");
    }

    private void ApplyWeaponConfig()
    {
        range        = weaponConfig.range;
        fireRate     = weaponConfig.fireRate;
        damagePerShot= weaponConfig.damage;
    }

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

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= meleeRange)
        {
            PerformMeleeAttack();
        }
        else
        {
            StopMeleeAnimation(); // melee menzilden çıkınca animasyonu kes

            if (enableLaser) // sadece flag true ise lazer çalışsın
            {
                Vector3 dir = (target.position - muzzle.position).normalized;
                ShootLaser(dir, target);
            }
            else
            {
                Debug.Log("[Attack] Laser disabled via flag.");
            }
        }
    }

    private void ShootLaser(Vector3 dir, Transform target)
    {
        audioSource?.PlayOneShot(audioSource.clip);

        GameObject laser = PoolManager.Instance != null
            ? PoolManager.Instance.Spawn(laserPrefab, muzzle.position, Quaternion.LookRotation(dir))
            : Instantiate(laserPrefab, muzzle.position, Quaternion.LookRotation(dir));

        

        if (laser.TryGetComponent<LaserProjectile>(out var proj))
        {
            proj.target = target;
            proj.damage = (int)damagePerShot;
            proj.hitMask = hitMask;
            proj.homing = true;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

    private void PerformMeleeAttack()
    {
        StartMeleeAnimation();

        Collider[] hits = Physics.OverlapSphere(transform.position, meleeRange, hitMask, QueryTriggerInteraction.Collide);
        bool any = false;
        foreach (var c in hits)
        {
            // İstersen tag kontrolü
            if (!c.CompareTag(targetTag)) continue;

            var health = c.GetComponentInParent<EnemyHealthController>() ?? c.GetComponent<EnemyHealthController>();
            if (health != null)
            {
                health.TakeDamage((int)meleeDamage);
                any = true;
                Debug.Log($"[Melee] Overlap hit {c.name} for {meleeDamage}");
                ToastManager.Instance.ShowToast("Melee hit an enemy!");
            }
        }

        if (!any) Debug.Log("[Melee] OverlapSphere hiç hedef bulamadı.");
    }


    private void StartMeleeAnimation()
    {
        if (meleeObject == null) return;

        meleeTween?.Kill();
        meleeObject.transform.localRotation = Quaternion.identity;

        meleeTween = meleeObject.transform
            .DOLocalRotate(new Vector3(90f, 0f, 0f), 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopMeleeAnimation()
    {
        if (meleeTween != null && meleeTween.IsActive())
        {
            meleeTween.Kill();
            meleeObject.transform.localRotation = Quaternion.identity;
        }
    }
}
