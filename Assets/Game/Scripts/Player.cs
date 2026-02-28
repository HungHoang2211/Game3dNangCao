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
        // Subscribe to PlayerStats events
        if (playerStats != null)
        {
            // Update health bar when HP changes
            playerStats.OnHealthChanged += UpdateHealthBar;

            // Initialize health bar
            UpdateHealthBar(playerStats.GetCurrentHP(), playerStats.GetMaxHP());
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthBar;
        }
    }

    /// <summary>
    /// Update health bar display
    /// </summary>
    private void UpdateHealthBar(float currentHP, float maxHP)
    {
        if (healthBar != null)
        {
            // Convert float HP to int for old HealthBar
            int currentHealthInt = Mathf.RoundToInt(currentHP);
            int maxHealthInt = Mathf.RoundToInt(maxHP);

            healthBar.SetMaxHealth(maxHealthInt);
            healthBar.SetHealth(currentHealthInt);

            Debug.Log($"[Player] Health Bar updated: {currentHealthInt}/{maxHealthInt}");
        }
    }

    /// <summary>
    /// Take damage (called by enemies) - int version for compatibility
    /// </summary>
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage); // Convert to float
    }

    /// <summary>
    /// Take damage (uses PlayerStats with defense calculation)
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (playerStats != null)
        {
            // PlayerStats will apply defense and update HP
            playerStats.TakeDamage(damage);
            // Health bar will auto-update via OnHealthChanged event
        }
        else
        {
            Debug.LogError("[Player] Cannot take damage - PlayerStats not found!");
        }
    }

    /// <summary>
    /// Die (called by PlayerStats OnPlayerDeath event)
    /// </summary>
    private void Die()
    {
        Debug.Log("[Player] Player died!");

        // TODO: Death handling
        // - Show death UI
        // - Disable controls
        // - Trigger respawn or game over
    }
}