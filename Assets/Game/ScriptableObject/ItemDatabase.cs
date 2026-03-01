using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Database of all items in the game
/// Create via: Assets → Create → Inventory → Item Database
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database", order = 2)]
public class ItemDatabase : ScriptableObject
{
    [Header("All Items")]
    [Tooltip("List of all items in the game")]
    public List<ItemObject> items = new List<ItemObject>();

    private Dictionary<int, ItemObject> itemDictionary;

    /// <summary>
    /// Initialize database (call at start)
    /// </summary>
    public void Initialize()
    {
        itemDictionary = new Dictionary<int, ItemObject>();

        foreach (ItemObject item in items)
        {
            if (item == null)
            {
                Debug.LogWarning("[ItemDatabase] Null item found in list!");
                continue;
            }

            if (itemDictionary.ContainsKey(item.id))
            {
                Debug.LogError($"[ItemDatabase] Duplicate item ID {item.id} found! Item: {item.itemName}");
                continue;
            }

            itemDictionary.Add(item.id, item);
        }

        Debug.Log($"[ItemDatabase] Initialized with {itemDictionary.Count} items");
    }

    /// <summary>
    /// Get item by ID
    /// </summary>
    public ItemObject GetItemByID(int id)
    {
        if (itemDictionary == null)
        {
            Initialize();
        }

        if (itemDictionary.TryGetValue(id, out ItemObject item))
        {
            return item;
        }

        Debug.LogWarning($"[ItemDatabase] Item with ID {id} not found!");
        return null;
    }

    /// <summary>
    /// Get all items of a specific type
    /// </summary>
    public List<ItemObject> GetItemsByType(ItemType type)
    {
        if (itemDictionary == null)
        {
            Initialize();
        }

        return items.Where(item => item != null && item.itemType == type).ToList();
    }

    /// <summary>
    /// Get all equipment items
    /// </summary>
    public List<ItemObject> GetAllEquipment()
    {
        if (itemDictionary == null)
        {
            Initialize();
        }

        return items.Where(item =>
            item != null &&
            (item.itemType == ItemType.Weapon ||
             item.itemType == ItemType.Armor ||
             item.itemType == ItemType.Accessory)
        ).ToList();
    }
}