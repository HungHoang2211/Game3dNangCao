using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main UI controller for inventory system
/// FIXED: Proper swap logic + Equipment swap when full
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private EquipmentManager equipment;

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentPanel;

    [Header("Inventory Grid")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject slotPrefab;

    [Header("Equipment Slots")]
    [SerializeField] private EquipmentSlotUI weaponSlot;
    [SerializeField] private EquipmentSlotUI armorSlot;
    [SerializeField] private EquipmentSlotUI accessorySlot;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI inventoryInfoText;
    [SerializeField] private Button closeButton;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [Header("Other Canvases to Hide")]
    [Tooltip("Canvas(es) to hide when inventory is open (e.g., HUD Canvas)")]
    [SerializeField] private GameObject[] canvasesToHide;

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
    private bool isInventoryOpen = false;

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<InventoryManager>();
        }

        if (equipment == null)
        {
            equipment = FindAnyObjectByType<EquipmentManager>();
        }
    }

    private void Start()
    {
        CreateInventorySlots();
        InitializeEquipmentSlots();

        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshInventoryUI;
        }

        if (equipment != null)
        {
            equipment.OnEquipmentChanged += OnEquipmentChanged;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }

        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }

        CloseInventory();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshInventoryUI;
        }

        if (equipment != null)
        {
            equipment.OnEquipmentChanged -= OnEquipmentChanged;
        }
    }

    // ============================================
    // CREATE SLOTS
    // ============================================

    private void CreateInventorySlots()
    {
        if (slotPrefab == null || gridContainer == null)
        {
            Debug.LogError("[InventoryUI] Slot prefab or grid container not assigned!");
            return;
        }

        if (slotUIList.Count > 0)
        {
            Debug.LogWarning("[InventoryUI] Slots already created. Skipping...");
            return;
        }

        int inventorySize = inventory.GetInventorySize();

        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, gridContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(i, this);
                slotUIList.Add(slotUI);
            }
            else
            {
                Debug.LogError("[InventoryUI] Slot prefab missing InventorySlotUI component!");
            }
        }

        Debug.Log($"[InventoryUI] Created {slotUIList.Count} inventory slots");
    }

    private void InitializeEquipmentSlots()
    {
        if (weaponSlot != null) weaponSlot.Initialize(this);
        if (armorSlot != null) armorSlot.Initialize(this);
        if (accessorySlot != null) accessorySlot.Initialize(this);
    }

    // ============================================
    // REFRESH UI
    // ============================================

    public void RefreshInventoryUI()
    {
        InventorySlot[] slots = inventory.GetAllSlots();

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i < slots.Length)
            {
                slotUIList[i].UpdateSlotUI(slots[i]);

                if (!slots[i].IsEmpty() && equipment.IsEquipped(slots[i].item))
                {
                    slotUIList[i].SetHighlight(true);
                }
                else
                {
                    slotUIList[i].SetHighlight(false);
                }
            }
        }

        UpdateInventoryInfo();
    }

    private void UpdateInventoryInfo()
    {
        if (inventoryInfoText != null)
        {
            int usedSlots = inventory.GetInventorySize() - inventory.GetEmptySlotCount();
            int totalSlots = inventory.GetInventorySize();
            inventoryInfoText.text = $"{usedSlots}/{totalSlots} slots used";
        }
    }

    private void OnEquipmentChanged(EquipmentSlot slot, ItemObject item)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                if (weaponSlot != null) weaponSlot.UpdateSlotUI(item);
                break;
            case EquipmentSlot.Armor:
                if (armorSlot != null) armorSlot.UpdateSlotUI(item);
                break;
            case EquipmentSlot.Accessory:
                if (accessorySlot != null) accessorySlot.UpdateSlotUI(item);
                break;
        }

        RefreshInventoryUI();
    }

    // ============================================
    // SLOT INTERACTIONS
    // ============================================

    public void OnSlotClicked(int slotIndex)
    {
        InventorySlot slot = inventory.GetSlot(slotIndex);

        if (slot == null || slot.IsEmpty()) return;

        ItemObject item = slot.item;

        switch (item.itemType)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                equipment.ToggleEquip(item);
                break;

            case ItemType.Consumable:
                UseConsumable(item, slotIndex);
                break;

            case ItemType.Material:
                Debug.Log($"[InventoryUI] {item.itemName} is a material. Use it in crafting/upgrading.");
                break;
        }
    }

    public void OnSlotRightClicked(int slotIndex)
    {
        InventorySlot slot = inventory.GetSlot(slotIndex);

        if (slot == null || slot.IsEmpty()) return;

        Debug.Log($"[InventoryUI] Dropping {slot.item.itemName}");
        inventory.RemoveItemFromSlot(slotIndex, 1);
    }

    public void OnEquipmentSlotClicked(EquipmentSlot slotType)
    {
        equipment.UnequipItem(slotType);
    }

    // ============================================
    // USE CONSUMABLE
    // ============================================

    private void UseConsumable(ItemObject item, int slotIndex)
    {
        if (item.itemType != ItemType.Consumable) return;

        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();

        if (playerStats != null && item.hpRestore > 0)
        {
            playerStats.Heal(item.hpRestore);
            Debug.Log($"[InventoryUI] Used {item.itemName}, restored {item.hpRestore} HP");

            inventory.RemoveItemFromSlot(slotIndex, 1);
        }
    }

    // ============================================
    // DRAG AND DROP SWAP OPERATIONS
    // ============================================

    /// <summary>
    /// ULTRA SAFE swap - No events, direct manipulation
    /// </summary>
    public void SwapInventorySlots(int fromIndex, int toIndex)
    {
        if (inventory == null)
        {
            Debug.LogError("[InventoryUI] Inventory is null!");
            return;
        }

        InventorySlot[] allSlots = inventory.GetAllSlots();

        // Validate indices
        if (fromIndex < 0 || fromIndex >= allSlots.Length)
        {
            Debug.LogError($"[InventoryUI] Invalid fromIndex: {fromIndex}");
            return;
        }

        if (toIndex < 0 || toIndex >= allSlots.Length)
        {
            Debug.LogError($"[InventoryUI] Invalid toIndex: {toIndex}");
            return;
        }

        // Get references
        InventorySlot fromSlot = allSlots[fromIndex];
        InventorySlot toSlot = allSlots[toIndex];

        // Store values (not references!)
        ItemObject tempItem = fromSlot.item;
        int tempQuantity = fromSlot.quantity;

        // Swap in one atomic operation
        fromSlot.item = toSlot.item;
        fromSlot.quantity = toSlot.quantity;

        toSlot.item = tempItem;
        toSlot.quantity = tempQuantity;

        // Verify swap
        Debug.Log($"[InventoryUI] SWAP COMPLETE:");
        Debug.Log($"  fromSlot ({fromIndex}): {(fromSlot.item != null ? fromSlot.item.itemName + " x" + fromSlot.quantity : "EMPTY")}");
        Debug.Log($"  toSlot ({toIndex}): {(toSlot.item != null ? toSlot.item.itemName + " x" + toSlot.quantity : "EMPTY")}");

        // Force UI refresh
        RefreshInventoryUI();
    }

    /// <summary>
    /// Equip item from inventory slot to equipment slot (drag-drop)
    /// </summary>
    public void EquipToSlot(int inventorySlotIndex, EquipmentSlot equipmentSlotType)
    {
        if (inventory == null || equipment == null) return;

        InventorySlot invSlot = inventory.GetSlot(inventorySlotIndex);
        if (invSlot == null || invSlot.IsEmpty()) return;

        ItemObject item = invSlot.item;

        EquipmentSlot itemSlotType = item.GetEquipmentSlot();

        if (itemSlotType != equipmentSlotType)
        {
            Debug.LogWarning($"[InventoryUI] Cannot equip {item.itemName} ({itemSlotType}) to {equipmentSlotType} slot!");
            return;
        }

        equipment.EquipItem(item);

        Debug.Log($"[InventoryUI] Equipped {item.itemName} from inventory to {equipmentSlotType} slot");
    }

    /// <summary>
    /// Unequip item from equipment slot to specific inventory slot (drag-drop)
    /// FIXED: Allow swap even when inventory is full
    /// </summary>
    public void UnequipToSlot(EquipmentSlot equipmentSlotType, int targetInventorySlot)
    {
        if (equipment == null || inventory == null) return;

        // Get currently equipped item
        ItemObject equippedItem = null;

        switch (equipmentSlotType)
        {
            case EquipmentSlot.Weapon:
                equippedItem = equipment.GetWeapon();
                break;
            case EquipmentSlot.Armor:
                equippedItem = equipment.GetArmor();
                break;
            case EquipmentSlot.Accessory:
                equippedItem = equipment.GetAccessory();
                break;
        }

        if (equippedItem == null)
        {
            Debug.LogWarning($"[InventoryUI] No item equipped in {equipmentSlotType} slot!");
            return;
        }

        // Check target inventory slot
        InventorySlot targetSlot = inventory.GetSlot(targetInventorySlot);
        if (targetSlot == null) return;

        // CASE 1: Target slot is empty - simple unequip
        if (targetSlot.IsEmpty())
        {
            if (inventory.IsFull())
            {
                Debug.LogWarning("[InventoryUI] Cannot unequip - inventory full!");
                return;
            }

            equipment.UnequipItem(equipmentSlotType);
            Debug.Log($"[InventoryUI] Unequipped {equippedItem.itemName} to empty slot {targetInventorySlot}");
            return;
        }

        // CASE 2: Target slot has item - try to swap
        ItemObject targetItem = targetSlot.item;

        // Check if target item can be equipped to same slot
        if (targetItem.GetEquipmentSlot() == equipmentSlotType)
        {
            // SWAP: This works even when inventory is full!
            Debug.Log($"[InventoryUI] Swapping {equippedItem.itemName} ↔ {targetItem.itemName}");

            // Step 1: Remove target item from inventory (make space)
            inventory.RemoveItem(targetItem, 1);

            // Step 2: Unequip current item (goes to inventory)
            equipment.UnequipItem(equipmentSlotType);

            // Step 3: Equip target item
            equipment.EquipItem(targetItem);

            Debug.Log($"[InventoryUI] Swap complete!");
        }
        else
        {
            // Can't swap - target item is different type
            if (inventory.IsFull())
            {
                Debug.LogWarning("[InventoryUI] Inventory full! Cannot unequip (target item not compatible).");
                return;
            }

            equipment.UnequipItem(equipmentSlotType);
            Debug.Log($"[InventoryUI] Unequipped {equippedItem.itemName}");
        }
    }

    /// <summary>
    /// Swap two equipment slots (drag-drop between equipment slots)
    /// </summary>
    public void SwapEquipmentSlots(EquipmentSlot fromSlot, EquipmentSlot toSlot)
    {
        if (equipment == null) return;

        if (fromSlot == toSlot)
        {
            Debug.LogWarning("[InventoryUI] Cannot swap equipment slot with itself!");
            return;
        }

        ItemObject fromItem = null;
        ItemObject toItem = null;

        switch (fromSlot)
        {
            case EquipmentSlot.Weapon:
                fromItem = equipment.GetWeapon();
                break;
            case EquipmentSlot.Armor:
                fromItem = equipment.GetArmor();
                break;
            case EquipmentSlot.Accessory:
                fromItem = equipment.GetAccessory();
                break;
        }

        switch (toSlot)
        {
            case EquipmentSlot.Weapon:
                toItem = equipment.GetWeapon();
                break;
            case EquipmentSlot.Armor:
                toItem = equipment.GetArmor();
                break;
            case EquipmentSlot.Accessory:
                toItem = equipment.GetAccessory();
                break;
        }

        if (fromItem == null)
        {
            Debug.LogWarning($"[InventoryUI] No item in {fromSlot} slot!");
            return;
        }

        if (fromItem.GetEquipmentSlot() != toSlot)
        {
            Debug.LogWarning($"[InventoryUI] Cannot equip {fromItem.itemName} ({fromItem.GetEquipmentSlot()}) to {toSlot} slot!");
            return;
        }

        if (toItem != null && toItem.GetEquipmentSlot() != fromSlot)
        {
            Debug.LogWarning($"[InventoryUI] Cannot swap - items are incompatible!");
            return;
        }

        equipment.UnequipItem(fromSlot);
        if (toItem != null)
        {
            equipment.UnequipItem(toSlot);
        }

        equipment.EquipItem(fromItem);
        if (toItem != null)
        {
            equipment.EquipItem(toItem);
        }

        Debug.Log($"[InventoryUI] Swapped {fromSlot} ↔ {toSlot}");
    }

    // ============================================
    // TOOLTIP
    // ============================================

    public void ShowTooltip(ItemObject item)
    {
        if (tooltipPanel == null || tooltipText == null || item == null) return;

        tooltipText.text = item.GetTooltip();
        tooltipPanel.SetActive(true);

        Vector2 mousePos = Input.mousePosition;
        tooltipPanel.transform.position = mousePos + new Vector2(10, -10);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    // ============================================
    // OPEN / CLOSE INVENTORY
    // ============================================

    public void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        Debug.Log("[InventoryUI] Opening inventory");

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(true);
        }

        isInventoryOpen = true;

        HideOtherCanvases();

        RefreshInventoryUI();
    }

    public void CloseInventory()
    {
        Debug.Log("[InventoryUI] Closing inventory");

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }

        isInventoryOpen = false;

        ShowOtherCanvases();

        HideTooltip();
    }

    // ============================================
    // CANVAS VISIBILITY MANAGEMENT
    // ============================================

    private void HideOtherCanvases()
    {
        if (canvasesToHide == null || canvasesToHide.Length == 0) return;

        foreach (GameObject canvas in canvasesToHide)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
                Debug.Log($"[InventoryUI] Hiding canvas: {canvas.name}");
            }
        }
    }

    private void ShowOtherCanvases()
    {
        if (canvasesToHide == null || canvasesToHide.Length == 0) return;

        foreach (GameObject canvas in canvasesToHide)
        {
            if (canvas != null)
            {
                canvas.SetActive(true);
                Debug.Log($"[InventoryUI] Showing canvas: {canvas.name}");
            }
        }
    }

    // ============================================
    // INPUT HANDLING
    // ============================================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isInventoryOpen)
        {
            CloseInventory();
        }
    }
}
