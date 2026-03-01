using UnityEngine;

/// <summary>
/// ScriptableObject for defining items
/// Create via: Assets → Create → Inventory → Item
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
public class ItemObject : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Unique ID for this item")]
    public int id;

    [Tooltip("Display name")]
    public string itemName;

    [Tooltip("Item icon for UI")]
    public Sprite icon;

    [TextArea(3, 5)]
    [Tooltip("Item description")]
    public string description;

    [Header("Item Type")]
    public ItemType itemType;

    [Header("Stacking")]
    [Tooltip("Can this item stack in inventory?")]
    public bool isStackable = false;

    [Tooltip("Maximum stack size (only for stackable items)")]
    public int maxStackSize = 99;

    [Header("Economy")]
    [Tooltip("Buy price (0 = cannot buy)")]
    public int buyPrice = 0;

    [Tooltip("Sell price (0 = cannot sell)")]
    public int sellPrice = 0;

    [Header("Equipment Stats (for Weapon/Armor/Accessory)")]
    [Tooltip("Damage bonus")]
    public float damageBonus = 0f;

    [Tooltip("Defense bonus")]
    public float defenseBonus = 0f;

    [Tooltip("Speed bonus")]
    public float speedBonus = 0f;

    [Tooltip("Crit Rate bonus (%)")]
    public float critRateBonus = 0f;


    [Header("Upgrade System (for Equipment)")]
    [Tooltip("Current upgrade level (0-10)")]
    public int upgradeLevel = 0;

    [Tooltip("Maximum upgrade level")]
    public int maxUpgradeLevel = 10;

    [Tooltip("Gold cost per upgrade level")]
    public int upgradeCostPerLevel = 100;

    [Tooltip("Colors for each upgrade level")]
    public Color[] upgradeColors = new Color[]
    {
        Color.white,     // Level 0
        Color.green,     // Level 1-2
        Color.blue,      // Level 3-4
        Color.magenta,   // Level 5-6
        Color.yellow,    // Level 7-8
        new Color(1f, 0.5f, 0f) // Level 9-10 (Orange)
    };

    [Header("Consumable Effects (for Consumable type)")]
    [Tooltip("HP restored when used")]
    public float hpRestore = 0f;

    [Tooltip("Can be used in combat?")]
    public bool usableInCombat = true;

    // ============================================
    // HELPER METHODS
    // ============================================

    /// <summary>
    /// Get equipment slot for this item
    /// </summary>
    public EquipmentSlot GetEquipmentSlot()
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return EquipmentSlot.Weapon;
            case ItemType.Armor:
                return EquipmentSlot.Armor;
            case ItemType.Accessory:
                return EquipmentSlot.Accessory;
            default:
                return EquipmentSlot.None;
        }
    }

    /// <summary>
    /// Get color for current upgrade level
    /// </summary>
    public Color GetUpgradeColor()
    {
        if (upgradeColors == null || upgradeColors.Length == 0)
            return Color.white;

        // Map upgrade level to color index
        int colorIndex = Mathf.Clamp(upgradeLevel / 2, 0, upgradeColors.Length - 1);
        return upgradeColors[colorIndex];
    }

    /// <summary>
    /// Check if item can be upgraded
    /// </summary>
    public bool CanUpgrade()
    {
        return upgradeLevel < maxUpgradeLevel &&
               (itemType == ItemType.Weapon ||
                itemType == ItemType.Armor ||
                itemType == ItemType.Accessory);
    }

    /// <summary>
    /// Get cost to upgrade to next level
    /// </summary>
    public int GetUpgradeCost()
    {
        if (!CanUpgrade()) return 0;
        return upgradeCostPerLevel * (upgradeLevel + 1);
    }

    /// <summary>
    /// Get formatted tooltip text
    /// </summary>
    public string GetTooltip()
    {
        string tooltip = $"<b>{itemName}</b>\n";
        tooltip += $"<color=grey>{description}</color>\n\n";

        // Equipment stats
        if (itemType == ItemType.Weapon || itemType == ItemType.Armor || itemType == ItemType.Accessory)
        {
            tooltip += "<b>Stats:</b>\n";
            if (damageBonus > 0) tooltip += $"Damage: +{damageBonus}\n";
            if (defenseBonus > 0) tooltip += $"Defense: +{defenseBonus}\n";
            if (speedBonus > 0) tooltip += $"Speed: +{speedBonus}\n";
            if (critRateBonus > 0) tooltip += $"Crit Rate: +{critRateBonus}%\n";

            if (upgradeLevel > 0)
            {
                tooltip += $"\n<color=yellow>Upgrade Level: +{upgradeLevel}</color>\n";
            }
        }

        // Consumable effects
        if (itemType == ItemType.Consumable && hpRestore > 0)
        {
            tooltip += $"<color=green>Restores {hpRestore} HP</color>\n";
        }

        // Economy
        if (sellPrice > 0)
        {
            tooltip += $"\n<color=yellow>Sell: {sellPrice} Gold</color>";
        }

        return tooltip;
    }
}