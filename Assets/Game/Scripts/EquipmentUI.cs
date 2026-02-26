using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler
{
    public ItemType slotType;
    public Image icon;
    private ItemStatus currentItem;

    public void SetItem(ItemStatus item)
    {
        currentItem = item;
        icon.sprite = item.itemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop: Bắt đầu xử lý drop vào slot " + slotType);

        InventorySlotUI draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot == null)
        {
            Debug.LogWarning("OnDrop: draggedSlot null!");
            return;
        }

        ItemStatus item = draggedSlot.GetItem();
        if (item == null)
        {
            Debug.LogWarning("OnDrop: item null!");
            return;
        }

        Debug.Log("OnDrop: Thả item " + item.itemName + " vào slot " + slotType);

        if (item.itemType == slotType)
        {
            SetItem(item);
            Debug.Log("OnDrop: Item hợp lệ, gọi PlayerAttack.EquipWeapon");

            PlayerAttack player = FindObjectOfType<PlayerAttack>();
            if (player != null && slotType == ItemType.Weapon)
            {
                player.EquipWeapon(item);
            }
        }
        else
        {
            Debug.LogWarning("OnDrop: ItemType không khớp slotType!");
        }
    }

}
