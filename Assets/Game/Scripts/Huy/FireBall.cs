using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 5f;

    [Header("Damage")]
    [Tooltip("FireBall's own damage (independent from player ATK)")]
    public float fireBallDamage = 50f;

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
            Debug.Log($"[FireBall] Hit {other.name} for {fireBallDamage:F1} damage!");

            enemy.TakeDamage(fireBallDamage);

            SpawnHitEffect();
            Destroy(gameObject);
            return;
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
