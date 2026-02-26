using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    private ItemStatus currentItem;

    public void SetItem(ItemStatus item)
    {
        currentItem = item;
        icon.sprite = currentItem.itemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}