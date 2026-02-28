using UnityEngine;

public class BulletH : MonoBehaviour
{
    public float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(float flyDistance, float impactForce)
    {
        this.impactForce = impactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ReturnBulletToPool();

        // Check for shield first (Hoang's enemy specific)
        Shield_Hoang shield = collision.gameObject.GetComponent<Shield_Hoang>();
        if (shield != null)
        {
            shield.ReduceDurability();
            return;
        }

        // Use IEnemy interface for damage
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy != null && !enemy.IsDead())
        {
            Debug.Log($"[BulletH] Hit {collision.gameObject.name}");
            enemy.TakeDamage(10); // Bullet damage

            // If it's Hoang's enemy, apply physics impact
            Enemy_Hoang enemyHoang = collision.gameObject.GetComponent<Enemy_Hoang>();
            if (enemyHoang != null)
            {
                Vector3 force = rb.linearVelocity.normalized * impactForce;
                Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

                enemyHoang.GetHit(); // Trigger battle mode
                enemyHoang.DeathImpact(force, collision.contacts[0].point, hitRigidbody);
            }
        }
    }

    private void ReturnBulletToPool() => ObjectPoolH.instance.ReturnObject(gameObject);

    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFx = ObjectPoolH.instance.GetObject(bulletImpactFX);
            newImpactFx.transform.position = contact.point;

            ObjectPoolH.instance.ReturnObject(newImpactFx, 1);
        }
    }
}