using UnityEngine;

/// <summary>
/// Projectile that damages enemies using IEnemy interface
/// Works with all enemy types through adapter pattern
/// </summary>
public class FireBall : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public int damage = 50;
    public float lifeTime = 5f;

    [Header("VFX")]
    public GameObject effect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Debug.Log("[FireBall] Created");
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Try to get IEnemy interface
        IEnemy enemy = other.GetComponent<IEnemy>();

        if (enemy != null && !enemy.IsDead())
        {
            Debug.Log($"[FireBall] Hit {other.name} for {damage} damage!");

            // Deal damage through interface
            enemy.TakeDamage(damage);

            // Spawn hit effect
            SpawnHitEffect();

            // Destroy fireball
            Destroy(gameObject);
            return;
        }

        // Warning if enemy tag but no interface
        if (other.CompareTag("Enemy"))
        {
            Debug.LogWarning($"[FireBall] Hit enemy {other.name} but no IEnemy adapter found! Add EnemyHealthAdapter/EnemyVuAdapter/EnemyKienAdapter");
        }
    }

    private void SpawnHitEffect()
    {
        if (effect != null)
        {
            GameObject hit = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(hit, 2f);
        }
    }
}