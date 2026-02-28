using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Adapter for Kien's enemies (use Animator State Machine)
/// Creates new health system compatible with IEnemy
/// </summary>
public class EnemyKienAdapter : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 80;
    private int currentHealth;
    private bool isDead = false;

    [Header("References")]
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;

        // Get references
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Debug.Log($"[EnemyKienAdapter] Initialized on {gameObject.name} with {maxHealth} HP");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Prevent negative HP

        Debug.Log($"[EnemyKienAdapter] {gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

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
        Debug.Log($"[EnemyKienAdapter] {gameObject.name} died!");

        // Notify quest manager
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnEnemyKilled();
        }

        // Stop animator and agent
        if (animator != null)
        {
            animator.enabled = false;
        }

        if (agent != null)
        {
            agent.enabled = false;
        }

        // Destroy after delay
        Destroy(gameObject, 2f);
    }
}