using UnityEngine;

/// <summary>
/// Debug tool for testing inventory system
/// Attach to Player or InventorySystem GameObject
/// </summary>
public class InventoryDebugger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private EquipmentManager equipment;
    [SerializeField] private ItemDatabase itemDatabase;

    private void Awake()
    {
        if (inventory == null) inventory = GetComponent<InventoryManager>();
        if (equipment == null) equipment = GetComponent<EquipmentManager>();
    }

    private void Update()
    {
        // Add items
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.AddItem(1, 1); // Iron Sword
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.AddItem(2, 1); // Leather Armor
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.AddItem(3, 5); // 5x Health Potion
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.AddItem(4, 10); // 10x Iron Ore
        }

        // Equip/Unequip
        if (Input.GetKeyDown(KeyCode.E))
        {
            ItemObject weapon = itemDatabase.GetItemByID(1);
            equipment.ToggleEquip(weapon);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ItemObject armor = itemDatabase.GetItemByID(2);
            equipment.ToggleEquip(armor);
        }

        // Print
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.PrintInventory();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            equipment.PrintEquipment();
        }
    }
}