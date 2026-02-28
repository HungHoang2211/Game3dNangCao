using UnityEngine;

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
        Debug.Log($"[FireBall] Hit something: {other.gameObject.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");

        // Try to get IEnemy interface
        IEnemy enemy = other.GetComponent<IEnemy>();

        if (enemy != null)
        {
            Debug.Log($"[FireBall] Found IEnemy on {other.name}!");

            if (!enemy.IsDead())
            {
                Debug.Log($"[FireBall] Dealing {damage} damage to {other.name}");
                enemy.TakeDamage(damage);

                // Spawn hit effect
                SpawnHitEffect();

                // Destroy fireball
                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.Log($"[FireBall] {other.name} is already dead, skipping");
            }
        }
        else
        {
            Debug.LogWarning($"[FireBall] Hit {other.name} but NO IEnemy found!");

            // Debug: List all components
            Component[] components = other.GetComponents<Component>();
            Debug.Log($"[FireBall] Components on {other.name}:");
            foreach (Component comp in components)
            {
                Debug.Log($"  - {comp.GetType().Name}");
            }
        }

        // Warning if enemy tag but no interface
        if (other.CompareTag("Enemy"))
        {
            Debug.LogError($"[FireBall] CRITICAL: {other.name} has 'Enemy' tag but NO IEnemy adapter!");
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