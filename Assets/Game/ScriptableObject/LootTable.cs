using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines what items an enemy can drop when killed.
/// Create via: Assets > Create > Loot > Loot Table
/// </summary>
[CreateAssetMenu(fileName = "New LootTable", menuName = "Loot/Loot Table", order = 1)]
public class LootTable : ScriptableObject
{
    [Header("Drop Settings")]
    [Tooltip("Maximum number of different items that can drop at once")]
    public int maxDrops = 3;

    [Header("Loot Entries")]
    public List<LootEntry> lootEntries = new List<LootEntry>();

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
                    results.Add(new LootResult { item = entry.item, quantity = qty });
                }
            }
        }

        return results;
    }
}

[System.Serializable]
public class LootEntry
{
    public ItemObject item;

    [Range(0f, 100f)]
    public float dropChance = 50f;

    public int minQuantity = 1;
    public int maxQuantity = 1;
}

public class LootResult
{
    public ItemObject item;
    public int quantity;
}
