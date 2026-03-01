using UnityEngine;

/// <summary>
/// Types of items in the game
/// </summary>
public enum ItemType
{
    Weapon,      // Equipable weapon (+damage)
    Armor,       // Equipable armor (+defense)
    Accessory,   // Equipable accessory (+speed/crit)
    Consumable,  // Usable items (health potions)
    Material     // Crafting materials (stackable, sellable)
}

/// <summary>
/// Equipment slot types
/// </summary>
public enum EquipmentSlot
{
    None,
    Weapon,
    Armor,
    Accessory
}

/// <summary>
/// Item rarity (optional - for future use)
/// </summary>
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}