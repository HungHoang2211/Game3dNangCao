using UnityEngine;

/// <summary>
/// Adapter for Hoang's enemies (already have EnemyHealth component)
/// Wraps existing EnemyHealth to implement IEnemy interface
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthAdapter : MonoBehaviour, IEnemy
{
    private EnemyHealth enemyHealth;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Debug.LogError($"[EnemyHealthAdapter] EnemyHealth component not found on {gameObject.name}!");
        }
        else
        {
            Debug.Log($"[EnemyHealthAdapter] Initialized on {gameObject.name}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (enemyHealth == null) return;

        enemyHealth.TakeDamage(damage);
        Debug.Log($"[EnemyHealthAdapter] {gameObject.name} took {damage} damage. HP: {GetCurrentHP()}/{GetMaxHP()}");
    }

    public int GetCurrentHP()
    {
        return enemyHealth != null ? enemyHealth.currentHp : 0;
    }

    public int GetMaxHP()
    {
        return enemyHealth != null ? enemyHealth.MaxHealth : 0;
    }

    public bool IsDead()
    {
        return enemyHealth == null || enemyHealth.currentHp <= 0;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Die()
    {
        // EnemyHealth already handles death in TakeDamage
        // Just ensure HP is 0
        if (enemyHealth != null && !IsDead())
        {
            enemyHealth.TakeDamage(enemyHealth.currentHp);
        }
    }
}