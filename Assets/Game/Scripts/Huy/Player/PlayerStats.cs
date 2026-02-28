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

    // Events
    public event Action<int, int> OnExpChanged; // (currentExp, expToNextLevel)
    public event Action<int> OnLevelUp; // (newLevel)
    public event Action<float, float> OnHealthChanged; // (currentHP, maxHP)
    public event Action OnStatsChanged;
    public event Action OnPlayerDeath;

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Awake()
    {
        if (config == null)
        {
            Debug.LogError("[PlayerStats] No config assigned! Create one via Assets → Create → Game Config → Player Stats Config");
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
        Debug.Log("=== TESTING EQUIPMENT BONUS ===");
        Debug.Log($"Damage BEFORE: {GetTotalDamage():F1}");
        AddEquipmentBonus(damage: 20f); // +20 damage from weapon
        Debug.Log($"Damage AFTER: {GetTotalDamage():F1}");

    }

    // ============================================
    // CORE STATS (Base + Equipment)
    // ============================================

    /// <summary>
    /// Get total max HP (base + equipment)
    /// </summary>
    public float GetMaxHP()
    {
        return config.GetMaxHPForLevel(currentLevel);
    }

    /// <summary>
    /// Get total damage (base + equipment)
    /// </summary>
    public float GetTotalDamage()
    {
        return config.GetDamageForLevel(currentLevel) + equipmentDamageBonus;
    }

    /// <summary>
    /// Get total defense (base + equipment)
    /// </summary>
    public float GetTotalDefense()
    {
        return config.GetDefenseForLevel(currentLevel) + equipmentDefenseBonus;
    }

    /// <summary>
    /// Get total speed (base + equipment)
    /// </summary>
    public float GetTotalSpeed()
    {
        return config.GetSpeedForLevel(currentLevel) + equipmentSpeedBonus;
    }

    /// <summary>
    /// Get total crit rate (base + equipment) as percentage (0-100)
    /// </summary>
    public float GetTotalCritRate()
    {
        return Mathf.Clamp(config.GetCritRateForLevel(currentLevel) + equipmentCritRateBonus, 0f, 100f);
    }

    /// <summary>
    /// Get total crit damage multiplier (base + equipment) as percentage
    /// </summary>
    public float GetTotalCritDamage()
    {
        return config.GetCritDamageForLevel(currentLevel) + equipmentCritDamageBonus;
    }

    // ============================================
    // COMBAT
    // ============================================

    /// <summary>
    /// Calculate final damage with crit roll
    /// </summary>
    public float CalculateDamage()
    {
        float baseDamage = GetTotalDamage();

        // Roll for critical hit
        if (RollCritical())
        {
            float critMultiplier = GetTotalCritDamage() / 100f;
            float critDamage = baseDamage * critMultiplier;
            Debug.Log($"[PlayerStats] CRITICAL HIT! {baseDamage:F1} × {critMultiplier:F2} = {critDamage:F1}");
            return critDamage;
        }

        return baseDamage;
    }

    /// <summary>
    /// Roll for critical hit based on crit rate
    /// </summary>
    public bool RollCritical()
    {
        return UnityEngine.Random.Range(0f, 100f) < GetTotalCritRate();
    }

    /// <summary>
    /// Take damage with defense calculation
    /// </summary>
    public void TakeDamage(float damage)
    {
        // Apply defense reduction
        float defense = GetTotalDefense();
        float damageReduction = defense / (defense + 100f); // Diminishing returns formula
        float finalDamage = damage * (1f - damageReduction);

        currentHP -= finalDamage;
        currentHP = Mathf.Max(0, currentHP);

        Debug.Log($"[PlayerStats] Took {damage:F1} damage (reduced by {defense:F1} DEF to {finalDamage:F1}). HP: {currentHP:F1}/{GetMaxHP():F1}");

        OnHealthChanged?.Invoke(currentHP, GetMaxHP());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal player
    /// </summary>
    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, GetMaxHP());

        Debug.Log($"[PlayerStats] Healed {amount:F1}. HP: {currentHP:F1}/{GetMaxHP():F1}");

        OnHealthChanged?.Invoke(currentHP, GetMaxHP());
    }

    /// <summary>
    /// Player death
    /// </summary>
    private void Die()
    {
        Debug.Log("[PlayerStats] Player died!");
        OnPlayerDeath?.Invoke();

        // TODO: Death handling (game over, respawn, etc)
    }

    // ============================================
    // LEVELING
    // ============================================

    /// <summary>
    /// Add experience points
    /// </summary>
    public void AddExp(int amount)
    {
        if (currentLevel >= config.maxLevel)
        {
            Debug.Log("[PlayerStats] Already at max level!");
            return;
        }

        currentExp += amount;
        Debug.Log($"[PlayerStats] Gained {amount} EXP. Total: {currentExp}/{config.GetExpForLevel(currentLevel)}");

        OnExpChanged?.Invoke(currentExp, config.GetExpForLevel(currentLevel));

        // Check for level up
        while (currentExp >= config.GetExpForLevel(currentLevel) && currentLevel < config.maxLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Level up player
    /// </summary>
    private void LevelUp()
    {
        int expForCurrentLevel = config.GetExpForLevel(currentLevel);
        currentExp -= expForCurrentLevel;
        currentLevel++;

        // Heal to full on level up
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

    // ============================================
    // EQUIPMENT BONUSES
    // ============================================

    /// <summary>
    /// Add equipment stat bonuses
    /// </summary>
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

    /// <summary>
    /// Remove equipment stat bonuses
    /// </summary>
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

    /// <summary>
    /// Reset all equipment bonuses
    /// </summary>
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