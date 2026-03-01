using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages player inventory (30 slots)
/// Attach to Player GameObject or create separate InventorySystem GameObject
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int inventorySize = 30;

    [Header("Database")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Current Inventory (Runtime)")]
    [SerializeField] private InventorySlot[] slots;

    // Events
    public event Action OnInventoryChanged;
    public event Action<ItemObject, int> OnItemAdded;   // (item, quantity)
    public event Action<ItemObject, int> OnItemRemoved; // (item, quantity)
    public event Action<string> OnInventoryMessage;     // For UI messages

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Awake()
    {
        InitializeInventory();

        if (itemDatabase != null)
        {
            itemDatabase.Initialize();
        }
        else
        {
            Debug.LogError("[InventoryManager] No ItemDatabase assigned!");
        }
    }

    private void InitializeInventory()
    {
        slots = new InventorySlot[inventorySize];

        for (int i = 0; i < inventorySize; i++)
        {
            slots[i] = new InventorySlot();
        }

        Debug.Log($"[InventoryManager] Initialized with {inventorySize} slots");
    }

    // ============================================
    // ADD ITEMS
    // ============================================

    /// <summary>
    /// Add item to inventory
    /// Returns true if successful
    /// </summary>
    public bool AddItem(ItemObject item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventoryManager] Trying to add null item!");
            return false;
        }

        Debug.Log($"[InventoryManager] Adding {quantity}x {item.itemName}");

        int remainingQuantity = quantity;

        // If stackable, try to add to existing stacks first
        if (item.isStackable)
        {
            remainingQuantity = AddToExistingStacks(item, remainingQuantity);
        }

        // If still have items left, add to empty slots
        if (remainingQuantity > 0)
        {
            remainingQuantity = AddToEmptySlots(item, remainingQuantity);
        }

        // Check if all items were added
        if (remainingQuantity > 0)
        {
            Debug.LogWarning($"[InventoryManager] Inventory full! Could not add {remainingQuantity}x {item.itemName}");
            OnInventoryMessage?.Invoke($"Inventory full! Lost {remainingQuantity}x {item.itemName}");

            // Partial success
            if (remainingQuantity < quantity)
            {
                int addedAmount = quantity - remainingQuantity;
                OnItemAdded?.Invoke(item, addedAmount);
                OnInventoryChanged?.Invoke();
            }

            return false;
        }

        Debug.Log($"[InventoryManager] Successfully added {quantity}x {item.itemName}");
        OnItemAdded?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Add item by ID
    /// </summary>
    public bool AddItem(int itemID, int quantity = 1)
    {
        ItemObject item = itemDatabase.GetItemByID(itemID);

        if (item == null)
        {
            Debug.LogError($"[InventoryManager] Item ID {itemID} not found in database!");
            return false;
        }

        return AddItem(item, quantity);
    }

    private int AddToExistingStacks(ItemObject item, int quantity)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty()) continue;
            if (slots[i].item.id != item.id) continue;
            if (!slots[i].CanAddToStack(item, quantity)) continue;

            int overflow = slots[i].AddToStack(quantity);

            if (overflow == 0)
            {
                // All items added to this stack
                return 0;
            }

            quantity = overflow;
        }

        return quantity;
    }

    private int AddToEmptySlots(ItemObject item, int quantity)
    {
        while (quantity > 0)
        {
            int emptySlotIndex = FindEmptySlot();

            if (emptySlotIndex == -1)
            {
                // No empty slots
                return quantity;
            }

            if (item.isStackable)
            {
                int amountToAdd = Mathf.Min(quantity, item.maxStackSize);
                slots[emptySlotIndex].item = item;
                slots[emptySlotIndex].quantity = amountToAdd;
                quantity -= amountToAdd;
            }
            else
            {
                // Non-stackable items take 1 slot each
                slots[emptySlotIndex].item = item;
                slots[emptySlotIndex].quantity = 1;
                quantity--;
            }
        }

        return 0;
    }

    // ============================================
    // REMOVE ITEMS
    // ============================================

    /// <summary>
    /// Remove item from inventory
    /// Returns true if successful
    /// </summary>
    public bool RemoveItem(ItemObject item, int quantity = 1)
    {
        if (item == null) return false;

        if (!HasItem(item, quantity))
        {
            Debug.LogWarning($"[InventoryManager] Not enough {item.itemName} to remove!");
            return false;
        }

        Debug.Log($"[InventoryManager] Removing {quantity}x {item.itemName}");

        int remainingToRemove = quantity;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty()) continue;
            if (slots[i].item.id != item.id) continue;

            if (slots[i].quantity >= remainingToRemove)
            {
                // This slot has enough
                slots[i].RemoveFromStack(remainingToRemove);
                remainingToRemove = 0;
                break;
            }
            else
            {
                // Take all from this slot and continue
                remainingToRemove -= slots[i].quantity;
                slots[i].Clear();
            }
        }

        OnItemRemoved?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Remove item by ID
    /// </summary>
    public bool RemoveItem(int itemID, int quantity = 1)
    {
        ItemObject item = itemDatabase.GetItemByID(itemID);
        return RemoveItem(item, quantity);
    }

    /// <summary>
    /// Remove item from specific slot
    /// </summary>
    public bool RemoveItemFromSlot(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogError($"[InventoryManager] Invalid slot index: {slotIndex}");
            return false;
        }

        if (slots[slotIndex].IsEmpty()) return false;

        ItemObject item = slots[slotIndex].item;
        int actualQuantity = Mathf.Min(quantity, slots[slotIndex].quantity);

        slots[slotIndex].RemoveFromStack(actualQuantity);

        OnItemRemoved?.Invoke(item, actualQuantity);
        OnInventoryChanged?.Invoke();

        return true;
    }

    // ============================================
    // QUERIES
    // ============================================

    /// <summary>
    /// Check if inventory has item
    /// </summary>
    public bool HasItem(ItemObject item, int quantity = 1)
    {
        if (item == null) return false;

        int count = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty()) continue;
            if (slots[i].item.id != item.id) continue;

            count += slots[i].quantity;

            if (count >= quantity) return true;
        }

        return false;
    }

    /// <summary>
    /// Get total quantity of item in inventory
    /// </summary>
    public int GetItemCount(ItemObject item)
    {
        if (item == null) return 0;

        int count = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty()) continue;
            if (slots[i].item.id == item.id)
            {
                count += slots[i].quantity;
            }
        }

        return count;
    }

    /// <summary>
    /// Find first empty slot index
    /// </summary>
    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Get number of empty slots
    /// </summary>
    public int GetEmptySlotCount()
    {
        int count = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Check if inventory is full
    /// </summary>
    public bool IsFull()
    {
        return GetEmptySlotCount() == 0;
    }

    // ============================================
    // SLOT ACCESS
    // ============================================

    /// <summary>
    /// Get slot at index
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogError($"[InventoryManager] Invalid slot index: {index}");
            return null;
        }

        return slots[index];
    }

    /// <summary>
    /// Get all slots
    /// </summary>
    public InventorySlot[] GetAllSlots()
    {
        return slots;
    }

    /// <summary>
    /// Get inventory size
    /// </summary>
    public int GetInventorySize()
    {
        return inventorySize;
    }

    // ============================================
    // DEBUG
    // ============================================

    /// <summary>
    /// Print inventory contents
    /// </summary>
    [ContextMenu("Print Inventory")]
    public void PrintInventory()
    {
        Debug.Log("=== INVENTORY CONTENTS ===");

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty())
            {
                Debug.Log($"Slot {i}: {slots[i].quantity}x {slots[i].item.itemName}");
            }
        }

        Debug.Log($"Empty slots: {GetEmptySlotCount()}/{inventorySize}");
    }
}