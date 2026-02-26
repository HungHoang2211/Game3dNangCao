using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Health")]
    public PlayerStat playerStat;

    public int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = playerStat.health;
        healthBar.SetMaxHealth(playerStat.health);
    }

    private void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
            Die();
        }
        
    }

    void Die()
    {

        Debug.Log("Player die!");
    }
}
