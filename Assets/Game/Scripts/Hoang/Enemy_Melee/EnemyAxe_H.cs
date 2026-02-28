using UnityEngine;

public class EnemyAxe_H : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed;
    private float timer = 1;

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        rotationSpeed = 1600;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer > 0 && player != null)
            direction = player.position + Vector3.up - transform.position;

        rb.linearVelocity = direction.normalized * flySpeed;

        if (rb.linearVelocity != Vector3.zero)
            transform.forward = rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit by bullet
        if (other.GetComponent<BulletH>() != null)
        {
            Impact();
            return;
        }

        // Hit player using tag (no need for Player script)
        if (other.CompareTag("Player"))
        {
            Impact();

            // Try to damage player if they have health component
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(15); // Axe damage
                Debug.Log("[EnemyAxe_H] Hit player for 15 damage");
            }
        }
    }

    private void Impact()
    {
        GameObject newFx = ObjectPoolH.instance.GetObject(impactFx);
        newFx.transform.position = transform.position;

        ObjectPoolH.instance.ReturnObject(gameObject);
        ObjectPoolH.instance.ReturnObject(newFx, 1f);
    }
}