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
        // Efekt çalışmaya başlasın
        if (ps == null) ps = GetComponent<ParticleSystem>();
        ps?.Play();
    }

    private void Update()
    {
        // Eğer partikül artık yaşamıyorsa otomatik pool’a geri gönder
        if (ps != null && !ps.IsAlive(true))
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
