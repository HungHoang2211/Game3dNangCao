using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Giới hạn số ô đồ theo UI của bạn (ví dụ 20 ô)
    public int inventorySize = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();

    // Sự kiện để báo cho UI cập nhật mỗi khi dữ liệu thay đổi
    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChanged;

    public bool AddItem(ItemStatus newItem, int amount = 1)
    {
        // 1. Kiểm tra xem item đã tồn tại để cộng dồn chưa (Logic Stack)
        foreach (var slot in slots)
        {
            if (slot.itemData == newItem)
            {
                slot.count += amount;
                onInventoryChanged?.Invoke();
                return true;
            }
        }

        // 2. Nếu là item mới, kiểm tra xem còn chỗ trống không
        if (slots.Count < inventorySize)
        {
            slots.Add(new InventorySlot(newItem, amount));
            onInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Hành trang đã đầy!");
        return false;
    }

    public void RemoveItem(int index)
    {
        if (index < slots.Count)
        {
            slots.RemoveAt(index);
            onInventoryChanged?.Invoke();
        }
    }
}