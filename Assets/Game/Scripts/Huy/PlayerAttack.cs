using UnityEngine;

/// <summary>
/// Player melee attack system using IEnemy interface
/// Detects and damages all enemies in attack range
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public int attackDamage = 20;
    public float attackSpeed = 1f;

    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private float nextAttackTime;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Spawn player at spawn point if set
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
    }

    void Update()
    {
        // Handle attack input
        if (Input.GetKey(KeyCode.Space))
        {
            OnAttackHold();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnAttackRelease();
        }
    }

    public void OnAttackHold()
    {
        ShowRange(true);

        // Check cooldown
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + 1f / attackSpeed;
        DoAttack();
    }

    public void OnAttackRelease()
    {
        ShowRange(false);
    }

    void DoAttack()
    {
        // Trigger animation
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        // Detect enemies in range using IEnemy interface
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        int enemiesHit = 0;

        foreach (Collider hit in hits)
        {
            IEnemy enemy = hit.GetComponent<IEnemy>();

            if (enemy != null && !enemy.IsDead())
            {
                Debug.Log($"[PlayerAttack] Hit {hit.name} for {attackDamage} damage!");
                enemy.TakeDamage(attackDamage);
                enemiesHit++;
            }
        }

        Debug.Log($"[PlayerAttack] Slash! Hit {enemiesHit} enemies");
    }

    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }
    }

    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}