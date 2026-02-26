using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    private ItemStatus currentItem;
    private InventoryUI inventoryUI;

    void Awake()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    public void SetItem(InventorySlot slot)
    {
        currentItem = slot.itemData;
        icon.sprite = currentItem.itemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // Gắn hàm này vào Event Trigger "Pointer Click" hoặc Button của Slot
    public void OnSlotClicked()
    {
        if (currentItem != null)
        {
            inventoryUI.DisplayItemInfo(currentItem);
        }
    }
}