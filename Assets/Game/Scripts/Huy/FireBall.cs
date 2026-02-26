using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 50;
    public float lifeTime = 5f;

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
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (effect != null)
            {
                GameObject hit = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(hit, 2f);
                Debug.Log("Booom");
            }

            Destroy(gameObject);
        }
    }
}