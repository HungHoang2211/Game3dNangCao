using UnityEngine;
using UnityEngine.AI;

public class EnemyVuAdapter : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f; // ← CHANGED: int → float
    private float currentHealth; // ← CHANGED: int → float
    private bool isDead = false;

    [Header("References")]
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        enemyAI = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Debug.Log($"[EnemyVuAdapter] Initialized on {gameObject.name} with {maxHealth} HP");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"[EnemyVuAdapter] {gameObject.name} took {damage:F1} damage. HP: {currentHealth:F1}/{maxHealth:F1}");

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
        Debug.Log($"[EnemyVuAdapter] {gameObject.name} died!");

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnEnemyKilled();
        }

        if (enemyAI != null) enemyAI.enabled = false;
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;

        Destroy(gameObject, 2f);
    }
}