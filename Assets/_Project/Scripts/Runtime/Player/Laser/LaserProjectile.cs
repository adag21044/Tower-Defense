using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] public float speed = 25f;
    [SerializeField] public float maxLife = 2f;
    [SerializeField] public bool homing = true; // target hareket ediyorsa takip etsin mi

    [HideInInspector] public Transform target;  // AtackController atayacak
    [HideInInspector] public int damage;
    [HideInInspector] public LayerMask hitMask;

    private float lifeTimer;

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLife)
        {
            Destroy(gameObject);
            return;
        }

        // Hareket yönü
        Vector3 dir;
        if (homing && target != null)
            dir = (target.position - transform.position).normalized;
        else
            dir = transform.forward;

        // Tünellemeyi engellemek için raycast
        float step = speed * Time.deltaTime;
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, step + 0.1f, hitMask))
        {
            // Hasar uygula
            var health = hit.collider.GetComponent<HealthController>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // İstersen burada çarpma VFX’i oluştur
            Destroy(gameObject);
            return;
        }

        // İleri hareket
        transform.position += dir * step;

        // Yönü güncelle (görsel doğru baksın)
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
