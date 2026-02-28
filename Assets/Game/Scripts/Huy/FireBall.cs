using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 5f;

    [Header("VFX")]
    public GameObject effect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IEnemy enemy = other.GetComponent<IEnemy>();

        if (enemy != null && !enemy.IsDead())
        {
            // Get damage from PlayerStats
            float damage = GetPlayerDamage();

            Debug.Log($"[FireBall] Hit {other.name} for {damage:F1} damage!");

            enemy.TakeDamage(damage);

            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.LogWarning($"[FireBall] Hit enemy {other.name} but no IEnemy adapter found!");
        }
    }

    /// <summary>
    /// Get damage from PlayerStats (with crit calculation)
    /// </summary>
    private float GetPlayerDamage()
    {
        // Find player in scene
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats != null)
        {
            // Use PlayerStats.CalculateDamage() for crit rolls
            return playerStats.CalculateDamage();
        }
        else
        {
            Debug.LogWarning("[FireBall] PlayerStats not found! Using default damage.");
            return 50f; // Fallback damage
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