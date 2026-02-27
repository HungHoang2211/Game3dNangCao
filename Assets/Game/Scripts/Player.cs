using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Health")]
   

    public int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
       
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
