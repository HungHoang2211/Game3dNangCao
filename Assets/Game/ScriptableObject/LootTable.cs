using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines what items an enemy can drop when killed.
/// Create via: Assets > Create > Loot > Loot Table
/// 
/// SETUP:
/// 1. Right-click in Project > Create > Loot > Loot Table
/// 2. Add items to the lootEntries list
/// 3. Set dropChance (0-100%) and quantity for each entry
/// 4. Assign this LootTable to EnemyLootDrop component on enemy prefab
/// </summary>
[CreateAssetMenu(fileName = "New LootTable", menuName = "Loot/Loot Table", order = 1)]
public class LootTable : ScriptableObject
{
    [Header("Drop Settings")]
    [Tooltip("Maximum number of different items that can drop at once")]
    public int maxDrops = 3;

    [Tooltip("Guaranteed gold drop range (0 = no gold)")]
    public int minGold = 0;
    public int maxGold = 50;

    [Header("Loot Entries")]
    public List<LootEntry> lootEntries = new List<LootEntry>();

    /// <summary>
    /// Roll all loot entries and return what should drop
    /// </summary>
    public List<LootResult> RollLoot()
    {
        List<LootResult> results = new List<LootResult>();

        foreach (LootEntry entry in lootEntries)
        {
            if (entry.item == null) continue;
            if (results.Count >= maxDrops) break;

            float roll = Random.Range(0f, 100f);

            if (roll <= entry.dropChance)
            {
                int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);

                if (qty > 0)
                {
                    results.Add(new LootResult
                    {
                        item = entry.item,
                        quantity = qty
                    });

                    Debug.Log($"[LootTable] Rolled {qty}x {entry.item.itemName} (chance: {entry.dropChance}%, rolled: {roll:F1})");
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Roll gold amount
    /// </summary>
    public int RollGold()
    {
        if (maxGold <= 0) return 0;
        return Random.Range(minGold, maxGold + 1);
    }
}

/// <summary>
/// Single entry in a loot table
/// </summary>
[System.Serializable]
public class LootEntry
{
    [Tooltip("Item that can drop")]
    public ItemObject item;

    [Tooltip("Chance to drop (0-100%)")]
    [Range(0f, 100f)]
    public float dropChance = 50f;

    [Tooltip("Minimum quantity if dropped")]
    public int minQuantity = 1;

    [Tooltip("Maximum quantity if dropped")]
    public int maxQuantity = 1;
}

/// <summary>
/// Result of a loot roll
/// </summary>
public class LootResult
{
    public ItemObject item;
    public int quantity;
}
