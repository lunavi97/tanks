using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Encontrar todos los tanques en el area alrededor de la bala y provocar el daño
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        foreach (Collider collider in colliders){
            Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth) continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.SetParent(null);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(gameObject);
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calcular la cantidad de daño que un objetivo debería recibir en base a su posición
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = Mathf.Min(explosionToTarget.magnitude, m_ExplosionRadius);
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        return damage;
    }
}