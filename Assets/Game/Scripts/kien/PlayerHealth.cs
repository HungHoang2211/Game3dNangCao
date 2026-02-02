using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        Debug.Log("player chete");

    
        this.enabled = false;
    }
}