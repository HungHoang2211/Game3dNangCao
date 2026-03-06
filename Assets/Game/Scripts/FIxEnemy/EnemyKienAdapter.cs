using UnityEngine;
using UnityEngine.AI;

public class EnemyKienAdapter : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 80f; // ← CHANGED: int → float
    private float currentHealth; // ← CHANGED: int → float
    private bool isDead = false;

    [Header("References")]
    private Animator animator;
    private NavMeshAgent agent;
    private EnemyLootDrop lootDrop;
    private EnemyReward enemyReward;
    void Start()
    {
        lootDrop = GetComponent<EnemyLootDrop>();
        enemyReward = GetComponent<EnemyReward>();
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Debug.Log($"[EnemyKienAdapter] Initialized on {gameObject.name} with {maxHealth} HP");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"[EnemyKienAdapter] {gameObject.name} took {damage:F1} damage. HP: {currentHealth:F1}/{maxHealth:F1}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }

    public float GetMaxHP()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"[EnemyKienAdapter] {gameObject.name} died!");

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnEnemyKilled(gameObject.name);
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        if (agent != null)
        {
            agent.enabled = false;
        }
        if (enemyReward != null) enemyReward.GiveReward();
        if (lootDrop != null) lootDrop.DropLoot();
        Destroy(gameObject, 0.5f);
    }
}