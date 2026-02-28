using UnityEngine;

/// <summary>
/// Configuration for player stats and progression
/// Create via: Assets → Create → Game Config → Player Stats Config
/// </summary>
[CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "Game Config/Player Stats Config", order = 1)]
public class PlayerStatsConfig : ScriptableObject
{
    [Header("Base Stats (Level 1)")]
    [Tooltip("Maximum health points")]
    public float baseMaxHP = 100f;

    [Tooltip("Base damage per attack")]
    public float baseDamage = 10f;

    [Tooltip("Damage reduction from attacks")]
    public float baseDefense = 5f;

    [Tooltip("Movement speed multiplier")]
    public float baseSpeed = 5f;

    [Tooltip("Critical hit chance (0-100%)")]
    public float baseCritRate = 5f;

    [Tooltip("Critical hit damage multiplier (100% = 2x damage)")]
    public float baseCritDamage = 150f;

    [Header("Leveling System")]
    [Tooltip("Starting level")]
    public int startingLevel = 1;

    [Tooltip("Maximum achievable level")]
    public int maxLevel = 50;

    [Tooltip("EXP formula: baseExpPerLevel * (level ^ expCurveExponent)")]
    public float baseExpPerLevel = 100f;

    [Tooltip("Exponential curve for EXP scaling (1.5 = moderate, 2.0 = steep)")]
    public float expCurveExponent = 1.5f;

    [Header("Stat Growth Per Level")]
    [Tooltip("HP gained per level")]
    public float hpPerLevel = 10f;

    [Tooltip("Damage gained per level")]
    public float damagePerLevel = 2f;

    [Tooltip("Defense gained per level")]
    public float defensePerLevel = 1f;

    [Tooltip("Speed gained per level")]
    public float speedPerLevel = 0.1f;

    [Tooltip("Crit Rate gained per level (%)")]
    public float critRatePerLevel = 0.5f;

    [Tooltip("Crit Damage gained per level (%)")]
    public float critDamagePerLevel = 2f;

    // ============================================
    // CALCULATED STATS
    // ============================================

    /// <summary>
    /// Calculate max HP at specific level
    /// </summary>
    public float GetMaxHPForLevel(int level)
    {
        return baseMaxHP + (hpPerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate base damage at specific level
    /// </summary>
    public float GetDamageForLevel(int level)
    {
        return baseDamage + (damagePerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate defense at specific level
    /// </summary>
    public float GetDefenseForLevel(int level)
    {
        return baseDefense + (defensePerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate speed at specific level
    /// </summary>
    public float GetSpeedForLevel(int level)
    {
        return baseSpeed + (speedPerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate crit rate at specific level
    /// </summary>
    public float GetCritRateForLevel(int level)
    {
        return baseCritRate + (critRatePerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate crit damage at specific level
    /// </summary>
    public float GetCritDamageForLevel(int level)
    {
        return baseCritDamage + (critDamagePerLevel * (level - 1));
    }

    /// <summary>
    /// Calculate EXP required to reach next level
    /// Formula: baseExpPerLevel * (level ^ expCurveExponent)
    /// </summary>
    public int GetExpForLevel(int level)
    {
        if (level >= maxLevel) return 0;
        return Mathf.RoundToInt(baseExpPerLevel * Mathf.Pow(level, expCurveExponent));
    }

    /// <summary>
    /// Get total EXP required from level 1 to target level
    /// </summary>
    public int GetTotalExpForLevel(int targetLevel)
    {
        int totalExp = 0;
        for (int i = 1; i < targetLevel; i++)
        {
            totalExp += GetExpForLevel(i);
        }
        return totalExp;
    }
}