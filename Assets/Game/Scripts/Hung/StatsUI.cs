using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Manager for displaying player stats
/// Uses OnEnable/OnDisable for safe toggle with Inventory panel
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

    // Subscribe mỗi lần panel được bật
    private void OnEnable()
    {
        if (playerStats == null) return;

        playerStats.OnStatsChanged += UpdateAllStats;
        playerStats.OnLevelUp += OnLevelUp;
        playerStats.OnExpChanged += UpdateExpBar;
        playerStats.OnHealthChanged += UpdateHealth;

        // Cập nhật ngay khi mở để đồng bộ data mới nhất
        UpdateAllStats();
        UpdateExpBar(playerStats.GetCurrentExp(), playerStats.GetExpToNextLevel());
    }

    // Unsubscribe mỗi lần panel bị tắt
    private void OnDisable()
    {
        if (playerStats == null) return;

        playerStats.OnStatsChanged -= UpdateAllStats;
        playerStats.OnLevelUp -= OnLevelUp;
        playerStats.OnExpChanged -= UpdateExpBar;
        playerStats.OnHealthChanged -= UpdateHealth;
    }

    // ============================================
    // UPDATE METHODS
    // ============================================

    private void UpdateAllStats()
    {
        if (playerStats == null) return;

        if (levelText != null)
        {
            levelText.text = $"LEVEL {playerStats.GetCurrentLevel()}";
        }

        if (hpText != null)
        {
            hpText.text = $"{playerStats.GetCurrentHP():F0} / {playerStats.GetMaxHP():F0}";
        }

        if (damageText != null)
        {
            damageText.text = $"{playerStats.GetTotalDamage():F1}";
        }

        if (defenseText != null)
        {
            defenseText.text = $"{playerStats.GetTotalDefense():F1}";
        }

        if (speedText != null)
        {
            speedText.text = $"{playerStats.GetTotalSpeed():F1}";
        }

        if (critRateText != null)
        {
            critRateText.text = $"{playerStats.GetTotalCritRate():F1}%";
        }
    }

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
    }

    private void UpdateHealth(float currentHP, float maxHP)
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHP:F0} / {maxHP:F0}";
        }
    }

    private void OnLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"LEVEL {newLevel}";
        }

        if (levelUpEffect != null)
        {
            GameObject effect = Instantiate(levelUpEffect, transform);
            Destroy(effect, 2f);
        }
    }
}