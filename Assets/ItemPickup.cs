using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemStatus itemData;

    private void OnTriggerEnter(Collider other)
    {
        // Thay vì tìm Inventory trên Player, ta tìm InventoryManager
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            bool added = inventory.AddItem(itemData);
            if (added)
            {
                Debug.Log("Đã nhặt: " + itemData.itemName);
                Destroy(gameObject);
            }
        }
    }
}
