using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemStatus itemData; // Asset ScriptableObject của item này

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có Inventory không
        Inventory inventory = other.GetComponent<Inventory>();
        if (inventory != null)
        {
            bool added = inventory.AddItem(itemData);
            if (added)
            {
                Debug.Log("Player đã nhặt: " + itemData.itemName);
                Destroy(gameObject); // Xóa item trong thế giới sau khi nhặt
            }
        }
    }
}