using System;


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

    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    public bool CanAddToStack(ItemObject itemToAdd, int amount = 1)
    {
        if (item == null || itemToAdd == null) return false;
        if (item.id != itemToAdd.id) return false;
        if (!item.isStackable) return false;

        return quantity + amount <= item.maxStackSize;
    }

    public int AddToStack(int amount)
    {
        if (item == null || !item.isStackable) return amount;

        int spaceLeft = item.maxStackSize - quantity;
        int amountToAdd = Math.Min(amount, spaceLeft);

        quantity += amountToAdd;

        return amount - amountToAdd; 
    }

    public void RemoveFromStack(int amount)
    {
        quantity -= amount;

        if (quantity <= 0)
        {
            Clear();
        }
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    public InventorySlot Clone()
    {
        return new InventorySlot(item, quantity);
    }
}