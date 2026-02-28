using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public float attackSpeed = 1f;

    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private float nextAttackTime;
    private Animator animator;
    private PlayerStats playerStats;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("[PlayerAttack] PlayerStats component not found! Add it to Player GameObject.");
        }
    }

    void Start()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }
    }

    void Update()
    {
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

        // Get damage from PlayerStats
        float damage = GetPlayerDamage();

        // Detect enemies in range
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        int enemiesHit = 0;

        foreach (Collider hit in hits)
        {
            IEnemy enemy = hit.GetComponent<IEnemy>();

            if (enemy != null && !enemy.IsDead())
            {
                Debug.Log($"[PlayerAttack] Hit {hit.name} for {damage:F1} damage!");
                enemy.TakeDamage(damage);
                enemiesHit++;
            }
        }

        Debug.Log($"[PlayerAttack] Slash! Hit {enemiesHit} enemies");
    }

    /// <summary>
    /// Get damage from PlayerStats (with crit calculation)
    /// </summary>
    private float GetPlayerDamage()
    {
        if (playerStats != null)
        {
            return playerStats.CalculateDamage();
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] PlayerStats not found! Using default damage.");
            return 20f; // Fallback damage
        }
    }

    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}