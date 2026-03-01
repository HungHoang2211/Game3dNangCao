using UnityEngine;
using System;

/// <summary>
/// Manages player stats, leveling, and equipment bonuses
/// Attach to Player GameObject
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerStatsConfig config;

    [Header("Current Stats (Runtime)")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private float currentHP;

    [Header("Equipment Bonuses")]
    [SerializeField] private float equipmentDamageBonus = 0f;
    [SerializeField] private float equipmentDefenseBonus = 0f;
    [SerializeField] private float equipmentSpeedBonus = 0f;
    [SerializeField] private float equipmentCritRateBonus = 0f;
    [SerializeField] private float equipmentCritDamageBonus = 0f;

    [Header("Experience Manager")]
    [SerializeField] private ExperienceManager experienceManager;

    // Events
    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelUp;
    public event Action<float, float> OnHealthChanged;
    public event Action OnStatsChanged;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        if (config == null)
        {
            Debug.LogError("[PlayerStats] No config assigned!");
            return;
        }

        currentLevel = config.startingLevel;
        currentHP = GetMaxHP();

        Debug.Log($"[PlayerStats] Initialized - Level {currentLevel}, HP: {currentHP}/{GetMaxHP()}");
    }

    private void Start()
    {
        OnStatsChanged?.Invoke();
        OnHealthChanged?.Invoke(currentHP, GetMaxHP());
    }

    public float GetMaxHP()
    {
        return config.GetMaxHPForLevel(currentLevel);
    }

    public float GetTotalDamage()
    {
        return config.GetDamageForLevel(currentLevel) + equipmentDamageBonus;
    }

    public float GetTotalDefense()
    {
        return config.GetDefenseForLevel(currentLevel) + equipmentDefenseBonus;
    }

    public float GetTotalSpeed()
    {
        return config.GetSpeedForLevel(currentLevel) + equipmentSpeedBonus;
    }

    public float GetTotalCritRate()
    {
        return Mathf.Clamp(config.GetCritRateForLevel(currentLevel) + equipmentCritRateBonus, 0f, 100f);
    }

    public float GetTotalCritDamage()
    {
        return config.GetCritDamageForLevel(currentLevel) + equipmentCritDamageBonus;
    }

    public float CalculateDamage()
    {
        float baseDamage = GetTotalDamage();

        if (RollCritical())
        {
            float critMultiplier = GetTotalCritDamage() / 100f;
            float critDamage = baseDamage * critMultiplier;
            Debug.Log($"[PlayerStats] CRITICAL HIT! {baseDamage:F1} × {critMultiplier:F2} = {critDamage:F1}");
            return critDamage;
        }

        return baseDamage;
    }

    public bool RollCritical()
    {
        return UnityEngine.Random.Range(0f, 100f) < GetTotalCritRate();
    }

    public void TakeDamage(float damage)
    {
        float defense = GetTotalDefense();
        float damageReduction = defense / (defense + 100f);
        float finalDamage = damage * (1f - damageReduction);

        currentHP -= finalDamage;
        currentHP = Mathf.Max(0, currentHP);

        Debug.Log($"[PlayerStats] Took {damage:F1} damage (reduced to {finalDamage:F1}). HP: {currentHP:F1}/{GetMaxHP():F1}");

        OnHealthChanged?.Invoke(currentHP, GetMaxHP());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, GetMaxHP());

        Debug.Log($"[PlayerStats] Healed {amount:F1}. HP: {currentHP:F1}/{GetMaxHP():F1}");

        OnHealthChanged?.Invoke(currentHP, GetMaxHP());
    }

    private void Die()
    {
        Debug.Log("[PlayerStats] Player died!");
        OnPlayerDeath?.Invoke();
    }

    // ============================================
    // LEVELING
    // ============================================

    public void AddExp(int amount)
    {
        if (currentLevel >= config.maxLevel)
        {
            Debug.Log("[PlayerStats] Already at max level!");
            return;
        }

        if (experienceManager != null)
        {
            experienceManager.AddExperience(amount);
        }
        else
        {
            currentExp += amount;
            Debug.Log($"[PlayerStats] Gained {amount} EXP. Total: {currentExp}/{config.GetExpForLevel(currentLevel)}");

            OnExpChanged?.Invoke(currentExp, config.GetExpForLevel(currentLevel));

            while (currentExp >= config.GetExpForLevel(currentLevel) && currentLevel < config.maxLevel)
            {
                LevelUp();
            }
        }
    }

    private void LevelUp()
    {
        int expForCurrentLevel = config.GetExpForLevel(currentLevel);
        currentExp -= expForCurrentLevel;
        currentLevel++;

        currentHP = GetMaxHP();

        Debug.Log($"[PlayerStats] LEVEL UP! Now level {currentLevel}");
        Debug.Log($"  - Max HP: {GetMaxHP():F1}");
        Debug.Log($"  - Damage: {GetTotalDamage():F1}");
        Debug.Log($"  - Defense: {GetTotalDefense():F1}");

        OnLevelUp?.Invoke(currentLevel);
        OnStatsChanged?.Invoke();
        OnHealthChanged?.Invoke(currentHP, GetMaxHP());
        OnExpChanged?.Invoke(currentExp, config.GetExpForLevel(currentLevel));
    }

    /// <summary>
    /// Sync level from ExperienceManager
    /// Called by ExperienceManager when player levels up
    /// </summary>
    public void SyncLevel(int newLevel)
    {
        Debug.Log($"[PlayerStats] SyncLevel called! Current: {currentLevel}, New: {newLevel}");

        if (newLevel <= currentLevel)
        {
            Debug.LogWarning($"[PlayerStats] New level {newLevel} is not higher than current {currentLevel}. Skipping.");
            return;
        }

        int oldLevel = currentLevel;
        currentLevel = newLevel;

        // Heal to full on level up
        float oldMaxHP = config.GetMaxHPForLevel(oldLevel);
        float newMaxHP = GetMaxHP();
        currentHP = newMaxHP;

        Debug.Log($"[PlayerStats] ✅ Synced to level {currentLevel}");
        Debug.Log($"  - Max HP: {oldMaxHP:F1} → {newMaxHP:F1}");
        Debug.Log($"  - Damage: {GetTotalDamage():F1}");
        Debug.Log($"  - Defense: {GetTotalDefense():F1}");
        Debug.Log($"  - Speed: {GetTotalSpeed():F1}");
        Debug.Log($"  - Crit Rate: {GetTotalCritRate():F1}%");

        // Fire events
        OnLevelUp?.Invoke(currentLevel);
        OnStatsChanged?.Invoke();
        OnHealthChanged?.Invoke(currentHP, GetMaxHP());
    }

    // ============================================
    // EQUIPMENT BONUSES
    // ============================================

    public void AddEquipmentBonus(float damage = 0, float defense = 0, float speed = 0, float critRate = 0, float critDamage = 0)
    {
        equipmentDamageBonus += damage;
        equipmentDefenseBonus += defense;
        equipmentSpeedBonus += speed;
        equipmentCritRateBonus += critRate;
        equipmentCritDamageBonus += critDamage;

        Debug.Log($"[PlayerStats] Equipment bonus added. Total damage: {GetTotalDamage():F1}");

        OnStatsChanged?.Invoke();
    }

    public void RemoveEquipmentBonus(float damage = 0, float defense = 0, float speed = 0, float critRate = 0, float critDamage = 0)
    {
        equipmentDamageBonus -= damage;
        equipmentDefenseBonus -= defense;
        equipmentSpeedBonus -= speed;
        equipmentCritRateBonus -= critRate;
        equipmentCritDamageBonus -= critDamage;

        Debug.Log($"[PlayerStats] Equipment bonus removed. Total damage: {GetTotalDamage():F1}");

        OnStatsChanged?.Invoke();
    }

    public void ResetEquipmentBonuses()
    {
        equipmentDamageBonus = 0;
        equipmentDefenseBonus = 0;
        equipmentSpeedBonus = 0;
        equipmentCritRateBonus = 0;
        equipmentCritDamageBonus = 0;

        Debug.Log("[PlayerStats] All equipment bonuses reset");

        OnStatsChanged?.Invoke();
    }

    // ============================================
    // GETTERS
    // ============================================

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentExp() => currentExp;
    public float GetCurrentHP() => currentHP;
    public float GetCurrentHPPercent() => currentHP / GetMaxHP();
    public int GetExpToNextLevel() => config.GetExpForLevel(currentLevel);
    public bool IsMaxLevel() => currentLevel >= config.maxLevel;
}