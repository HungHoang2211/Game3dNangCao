using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 20;
    public List<ItemStatus> items = new List<ItemStatus>();

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChanged;

    public bool AddItem(ItemStatus newItem)
    {
        if (items.Count < inventorySize)
        {
            items.Add(newItem);
            onInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Inventory đầy!");
        return false;
    }

    public void RemoveItem(ItemStatus item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onInventoryChanged?.Invoke();
        }
    }
}