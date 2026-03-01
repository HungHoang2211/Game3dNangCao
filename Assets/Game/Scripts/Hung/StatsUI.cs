using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Manager for displaying player stats
/// Subscribes to PlayerStats events for real-time updates
/// </summary>
public class StatsUI : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Level & EXP")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider expBar;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI critRateText;

    [Header("Visual Effects")]
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private float statUpdateAnimDuration = 0.3f;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("[StatsUI] PlayerStats not found!");
        }
    }

    private void Start()
    {
        if (playerStats != null)
        {
            // Subscribe to events
            playerStats.OnStatsChanged += UpdateAllStats;
            playerStats.OnLevelUp += OnLevelUp;
            playerStats.OnExpChanged += UpdateExpBar;
            playerStats.OnHealthChanged += UpdateHealth;

            // Initial update
            UpdateAllStats();
            UpdateExpBar(playerStats.GetCurrentExp(), playerStats.GetExpToNextLevel());
        }
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            // Unsubscribe from events
            playerStats.OnStatsChanged -= UpdateAllStats;
            playerStats.OnLevelUp -= OnLevelUp;
            playerStats.OnExpChanged -= UpdateExpBar;
            playerStats.OnHealthChanged -= UpdateHealth;
        }
    }

    // ============================================
    // UPDATE METHODS
    // ============================================

    /// <summary>
    /// Update all stats display
    /// </summary>
    private void UpdateAllStats()
    {
        if (playerStats == null) return;

        // Level
        if (levelText != null)
        {
            levelText.text = $"LEVEL {playerStats.GetCurrentLevel()}";
        }

        // HP
        if (hpText != null)
        {
            hpText.text = $"{playerStats.GetCurrentHP():F0} / {playerStats.GetMaxHP():F0}";
        }

        // Damage
        if (damageText != null)
        {
            damageText.text = $"{playerStats.GetTotalDamage():F1}";
        }

        // Defense
        if (defenseText != null)
        {
            defenseText.text = $"{playerStats.GetTotalDefense():F1}";
        }

        // Speed
        if (speedText != null)
        {
            speedText.text = $"{playerStats.GetTotalSpeed():F1}";
        }

        // Crit Rate
        if (critRateText != null)
        {
            critRateText.text = $"{playerStats.GetTotalCritRate():F1}%";
        }


        Debug.Log("[StatsUI] All stats updated");
    }

    /// <summary>
    /// Update EXP bar
    /// </summary>
    private void UpdateExpBar(int currentExp, int expToNextLevel)
    {
        if (expBar != null)
        {
            expBar.maxValue = expToNextLevel;
            expBar.value = currentExp;
        }

        if (expText != null)
        {
            expText.text = $"{currentExp} / {expToNextLevel} EXP";
        }

        Debug.Log($"[StatsUI] EXP updated: {currentExp}/{expToNextLevel}");
    }

    /// <summary>
    /// Update health display
    /// </summary>
    private void UpdateHealth(float currentHP, float maxHP)
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHP:F0} / {maxHP:F0}";
        }
    }

    /// <summary>
    /// Handle level up event
    /// </summary>
    private void OnLevelUp(int newLevel)
    {
        Debug.Log($"[StatsUI] Level up to {newLevel}!");

        // Update level text
        if (levelText != null)
        {
            levelText.text = $"LEVEL {newLevel}";
        }

        // Play level up effect
        if (levelUpEffect != null)
        {
            GameObject effect = Instantiate(levelUpEffect, transform);
            Destroy(effect, 2f);
        }

        // TODO: Add level up animation/sound
    }
}