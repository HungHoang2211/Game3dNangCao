using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Adapter for Vu's enemies (don't have EnemyHealth component)
/// Creates new health system compatible with IEnemy
/// </summary>
public class EnemyVuAdapter : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("References")]
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        // Get references
        enemyAI = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Debug.Log($"[EnemyVuAdapter] Initialized on {gameObject.name} with {maxHealth} HP");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Prevent negative HP

        Debug.Log($"[EnemyVuAdapter] {gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetCurrentHP()
    {
        return currentHealth;
    }

    public int GetMaxHP()
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
        Debug.Log($"[EnemyVuAdapter] {gameObject.name} died!");

        // Notify quest manager
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnEnemyKilled();
        }

        // Disable AI components
        if (enemyAI != null) enemyAI.enabled = false;
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;

        // Destroy after delay
        Destroy(gameObject, 2f);
    }
}