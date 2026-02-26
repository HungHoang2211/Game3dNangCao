using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP Setting")]
    public int currentHp;
    public int MaxHealth = 30;
    public GameObject itemPrefab;
    private bool isDead = false;
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Destroy(gameObject);
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnEnemyKilled();
        }

        DropItem();
        Debug.Log("Enemy Die");
    }
    void DropItem() { Instantiate(itemPrefab, transform.position, Quaternion.identity); }
}
