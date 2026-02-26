using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform slotParent;
    private InventorySlotUI[] uiSlots;

    void Start()
    {
        inventory.onInventoryChanged += UpdateUI;
        uiSlots = slotParent.GetComponentsInChildren<InventorySlotUI>();
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < inventory.items.Count)
                uiSlots[i].SetItem(inventory.items[i]);
            else
                uiSlots[i].ClearSlot();
        }
    }
}