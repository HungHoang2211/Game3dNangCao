using UnityEngine;
using System;

/// <summary>
/// Manages equipped items (Weapon, Armor, Accessory)
/// Integrates with PlayerStats for stat bonuses
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InventoryManager inventory;

    [Header("Equipment Slots")]
    [SerializeField] private ItemObject equippedWeapon;
    [SerializeField] private ItemObject equippedArmor;
    [SerializeField] private ItemObject equippedAccessory;

    // Events
    public event Action<EquipmentSlot, ItemObject> OnEquipmentChanged; // (slot, item)

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (inventory == null)
        {
            inventory = GetComponent<InventoryManager>();
        }

        if (playerStats == null)
        {
            Debug.LogError("[EquipmentManager] PlayerStats not found!");
        }

        if (inventory == null)
        {
            Debug.LogError("[EquipmentManager] InventoryManager not found!");
        }
    }

    // ============================================
    // EQUIP / UNEQUIP
    // ============================================

    /// <summary>
    /// Equip item from inventory
    /// </summary>
    public bool EquipItem(ItemObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("[EquipmentManager] Cannot equip null item!");
            return false;
        }

        // Check if item is equipment
        EquipmentSlot slot = item.GetEquipmentSlot();
        if (slot == EquipmentSlot.None)
        {
            Debug.LogWarning($"[EquipmentManager] {item.itemName} is not equipable!");
            return false;
        }

        // Check if player has item in inventory
        if (!inventory.HasItem(item, 1))
        {
            Debug.LogWarning($"[EquipmentManager] {item.itemName} not found in inventory!");
            return false;
        }

        // Unequip current item in that slot (if any)
        ItemObject currentItem = GetEquippedItem(slot);
        if (currentItem != null)
        {
            UnequipItem(slot);
        }

        // Remove from inventory
        inventory.RemoveItem(item, 1);

        // Equip new item
        SetEquippedItem(slot, item);

        // Apply stat bonuses
        ApplyEquipmentBonuses(item);

        Debug.Log($"[EquipmentManager] Equipped {item.itemName} to {slot} slot");

        OnEquipmentChanged?.Invoke(slot, item);

        return true;
    }

    /// <summary>
    /// Unequip item and return to inventory
    /// </summary>
    public bool UnequipItem(EquipmentSlot slot)
    {
        ItemObject item = GetEquippedItem(slot);

        if (item == null)
        {
            Debug.LogWarning($"[EquipmentManager] No item equipped in {slot} slot!");
            return false;
        }

        // Check if inventory has space
        if (inventory.IsFull() && !item.isStackable)
        {
            Debug.LogWarning("[EquipmentManager] Inventory full! Cannot unequip.");
            return false;
        }

        // Remove stat bonuses
        RemoveEquipmentBonuses(item);

        // Add back to inventory
        inventory.AddItem(item, 1);

        // Clear slot
        SetEquippedItem(slot, null);

        Debug.Log($"[EquipmentManager] Unequipped {item.itemName} from {slot} slot");

        OnEquipmentChanged?.Invoke(slot, null);

        return true;
    }

    /// <summary>
    /// Quick equip/unequip toggle
    /// </summary>
    public void ToggleEquip(ItemObject item)
    {
        if (item == null) return;

        EquipmentSlot slot = item.GetEquipmentSlot();
        if (slot == EquipmentSlot.None) return;

        ItemObject currentItem = GetEquippedItem(slot);

        if (currentItem != null && currentItem.id == item.id)
        {
            // Same item equipped, unequip it
            UnequipItem(slot);
        }
        else
        {
            // Different item or empty slot, equip it
            EquipItem(item);
        }
    }

    // ============================================
    // STAT BONUSES
    // ============================================

    private void ApplyEquipmentBonuses(ItemObject item)
    {
        if (playerStats == null || item == null) return;

        playerStats.AddEquipmentBonus(
            damage: item.damageBonus,
            defense: item.defenseBonus,
            speed: item.speedBonus,
            critRate: item.critRateBonus

        );
    }

    private void RemoveEquipmentBonuses(ItemObject item)
    {
        if (playerStats == null || item == null) return;

        playerStats.RemoveEquipmentBonus(
            damage: item.damageBonus,
            defense: item.defenseBonus,
            speed: item.speedBonus,
            critRate: item.critRateBonus

        );
    }

    // ============================================
    // GETTERS / SETTERS
    // ============================================

    private ItemObject GetEquippedItem(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                return equippedWeapon;
            case EquipmentSlot.Armor:
                return equippedArmor;
            case EquipmentSlot.Accessory:
                return equippedAccessory;
            default:
                return null;
        }
    }

    private void SetEquippedItem(EquipmentSlot slot, ItemObject item)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                equippedWeapon = item;
                break;
            case EquipmentSlot.Armor:
                equippedArmor = item;
                break;
            case EquipmentSlot.Accessory:
                equippedAccessory = item;
                break;
        }
    }

    public ItemObject GetWeapon() => equippedWeapon;
    public ItemObject GetArmor() => equippedArmor;
    public ItemObject GetAccessory() => equippedAccessory;

    /// <summary>
    /// Check if item is currently equipped
    /// </summary>
    public bool IsEquipped(ItemObject item)
    {
        if (item == null) return false;

        return (equippedWeapon != null && equippedWeapon.id == item.id) ||
               (equippedArmor != null && equippedArmor.id == item.id) ||
               (equippedAccessory != null && equippedAccessory.id == item.id);
    }

    // ============================================
    // DEBUG
    // ============================================

    [ContextMenu("Print Equipment")]
    public void PrintEquipment()
    {
        Debug.Log("=== EQUIPPED ITEMS ===");
        Debug.Log($"Weapon: {(equippedWeapon != null ? equippedWeapon.itemName : "None")}");
        Debug.Log($"Armor: {(equippedArmor != null ? equippedArmor.itemName : "None")}");
        Debug.Log($"Accessory: {(equippedAccessory != null ? equippedAccessory.itemName : "None")}");
    }
}