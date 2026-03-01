using System;

/// <summary>
/// Represents a single inventory slot
/// </summary>
[Serializable]
public class InventorySlot
{
    public ItemObject item;      // Item in this slot (null if empty)
    public int quantity;          // Stack quantity

    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    public InventorySlot(ItemObject item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    /// <summary>
    /// Check if slot is empty
    /// </summary>
    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    /// <summary>
    /// Check if can add more of this item to stack
    /// </summary>
    public bool CanAddToStack(ItemObject itemToAdd, int amount = 1)
    {
        if (item == null || itemToAdd == null) return false;
        if (item.id != itemToAdd.id) return false;
        if (!item.isStackable) return false;

        return quantity + amount <= item.maxStackSize;
    }

    /// <summary>
    /// Add to stack (returns overflow amount)
    /// </summary>
    public int AddToStack(int amount)
    {
        if (item == null || !item.isStackable) return amount;

        int spaceLeft = item.maxStackSize - quantity;
        int amountToAdd = Math.Min(amount, spaceLeft);

        quantity += amountToAdd;

        return amount - amountToAdd; // Return overflow
    }

    /// <summary>
    /// Remove from stack
    /// </summary>
    public void RemoveFromStack(int amount)
    {
        quantity -= amount;

        if (quantity <= 0)
        {
            Clear();
        }
    }

    /// <summary>
    /// Clear slot
    /// </summary>
    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    /// <summary>
    /// Clone this slot
    /// </summary>
    public InventorySlot Clone()
    {
        return new InventorySlot(item, quantity);
    }
}