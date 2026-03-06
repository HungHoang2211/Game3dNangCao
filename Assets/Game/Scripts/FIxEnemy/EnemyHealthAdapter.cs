using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthAdapter : MonoBehaviour, IEnemy
{
    private EnemyHealth enemyHealth;
    private EnemyLootDrop lootDrop;
    private EnemyReward enemyReward;
    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Debug.LogError($"[EnemyHealthAdapter] EnemyHealth component NOT FOUND on {gameObject.name}!");
        }
        else
        {
            Debug.Log($"[EnemyHealthAdapter] Initialized on {gameObject.name}");
        }
    }
    private void Start()
    {
        lootDrop = GetComponent<EnemyLootDrop>();
        enemyReward = GetComponent<EnemyReward>();
    }
    // ============================================
    // FLOAT DAMAGE (NEW)
    // ============================================

    public void TakeDamage(float damage)
    {
        if (enemyHealth == null)
        {
            return;
        }

        // Convert float to int for legacy EnemyHealth
        int intDamage = Mathf.RoundToInt(damage);

        Debug.Log($"[EnemyHealthAdapter] {gameObject.name} taking {damage:F1} damage (rounded to {intDamage})");

        enemyHealth.TakeDamage(intDamage);

        if (IsDead())
        {
            if (enemyReward != null) enemyReward.GiveReward();
            if (lootDrop != null) lootDrop.DropLoot();
            Debug.Log($"[EnemyHealthAdapter] {gameObject.name} DIED!");
        }
    }

    public float GetCurrentHP()
    {
        return enemyHealth != null ? enemyHealth.currentHp : 0f;
    }

    public float GetMaxHP()
    {
        return enemyHealth != null ? enemyHealth.MaxHealth : 0f;
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