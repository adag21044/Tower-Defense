using UnityEngine;

public class AutoDespawnParticle : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        ps?.Play();
    }

    private void Update()
    {
        if (ps != null && !ps.IsAlive(true))
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
