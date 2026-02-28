using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthAdapter : MonoBehaviour, IEnemy
{
    private EnemyHealth enemyHealth;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Debug.LogError($"[EnemyHealthAdapter] ERROR: EnemyHealth component NOT FOUND on {gameObject.name}!");
        }
        else
        {
            Debug.Log($"[EnemyHealthAdapter] ✅ Initialized on {gameObject.name}");
            Debug.Log($"  - Max HP: {enemyHealth.MaxHealth}");
            Debug.Log($"  - Current HP: {enemyHealth.currentHp}");
        }
    }

    void Start()
    {
        Debug.Log($"[EnemyHealthAdapter] START - {gameObject.name} ready!");
        Debug.Log($"  - GameObject: {gameObject.name}");
        Debug.Log($"  - Tag: {gameObject.tag}");
        Debug.Log($"  - Layer: {gameObject.layer}");
        Debug.Log($"  - Active: {gameObject.activeInHierarchy}");
    }

    public void TakeDamage(int damage)
    {
        if (enemyHealth == null)
        {
            Debug.LogError($"[EnemyHealthAdapter] Cannot take damage - EnemyHealth is NULL!");
            return;
        }

        Debug.Log($"[EnemyHealthAdapter] {gameObject.name} taking {damage} damage!");
        Debug.Log($"  - HP before: {enemyHealth.currentHp}/{enemyHealth.MaxHealth}");

        enemyHealth.TakeDamage(damage);

        Debug.Log($"  - HP after: {enemyHealth.currentHp}/{enemyHealth.MaxHealth}");

        if (IsDead())
        {
            Debug.Log($"[EnemyHealthAdapter] {gameObject.name} DIED!");
        }
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
        if (enemyHealth != null && !IsDead())
        {
            Debug.Log($"[EnemyHealthAdapter] Force killing {gameObject.name}");
            enemyHealth.TakeDamage(enemyHealth.currentHp);
        }
    }
}