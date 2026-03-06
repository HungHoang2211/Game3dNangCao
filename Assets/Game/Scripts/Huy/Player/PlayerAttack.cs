using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Base attack range without any weapon")]
    public float baseAttackRange = 2f;

    public float attackSpeed = 1f;

    [Header("Range Visual")]
    [Tooltip("Circle projector/sprite that shows attack range on ground")]
    public GameObject attackRangeCircle;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private float nextAttackTime;
    private Animator animator;
    private PlayerStats playerStats;

    private PlayerAttackSound attackSound;


    private float currentAttackRange;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerStats = GetComponent<PlayerStats>();

        attackSound = GetComponent<PlayerAttackSound>();

        if (playerStats == null)
        {
            Debug.LogError("[PlayerAttack] PlayerStats component not found!");
        }
    }

    void Start()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        if (playerStats != null)
        {
            playerStats.OnStatsChanged += UpdateAttackRange;
        }

        UpdateAttackRange();
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateAttackRange;
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

    private void UpdateAttackRange()
    {
        float rangeBonus = 0f;

        if (playerStats != null)
        {
            rangeBonus = playerStats.GetAttackRangeBonus();
        }

        currentAttackRange = baseAttackRange + rangeBonus;

        UpdateRangeCircleScale();

        Debug.Log($"[PlayerAttack] Range updated: {baseAttackRange} + {rangeBonus} = {currentAttackRange}");
    }
    private void UpdateRangeCircleScale()
    {
        if (attackRangeCircle == null) return;

        float diameter = currentAttackRange * 2f;
        attackRangeCircle.transform.localScale = new Vector3(diameter, diameter, diameter);
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
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        float damage = GetPlayerDamage();

        Collider[] hits = Physics.OverlapSphere(transform.position, currentAttackRange);
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

        if (attackSound != null)
        {
            if (enemiesHit > 0)
            {
                attackSound.PlayEnemyHitSound();
            }
            else
            {
                attackSound.PlayAirSound();
            }
        }

        Debug.Log($"[PlayerAttack] Slash! Range: {currentAttackRange:F1}, Hit {enemiesHit} enemies");
    }

    private float GetPlayerDamage()
    {
        if (playerStats != null)
        {
            return playerStats.CalculateDamage();
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] PlayerStats not found! Using default damage.");
            return 20f;
        }
    }

    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }
    }

    public float GetCurrentAttackRange()
    {
        return currentAttackRange;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float range = Application.isPlaying ? currentAttackRange : baseAttackRange;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
