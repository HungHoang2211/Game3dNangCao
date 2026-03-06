using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health UI")]
    public HealthBar healthBar;

    private PlayerStats playerStats;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("[Player] PlayerStats component not found! Add it to Player GameObject.");
        }
    }

    void Start()
    {


        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(playerStats.GetCurrentHP(), playerStats.GetMaxHP());
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHP, float maxHP)
    {
        if (healthBar != null)
        {
            int currentHealthInt = Mathf.RoundToInt(currentHP);
            int maxHealthInt = Mathf.RoundToInt(maxHP);

            healthBar.SetMaxHealth(maxHealthInt);
            healthBar.SetHealth(currentHealthInt);

            Debug.Log($"[Player] Health Bar updated: {currentHealthInt}/{maxHealthInt}");
        }
    }
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage); 
    }

    public void TakeDamage(float damage)
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
    }
}