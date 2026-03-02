using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main UI controller for inventory system
/// Manages inventory grid, equipment slots, and tooltips
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
        // Auto-find references if not assigned
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

        // Subscribe to events
        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshInventoryUI;
        }

        if (equipment != null)
        {
            equipment.OnEquipmentChanged += OnEquipmentChanged;
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }

        // Hide tooltip
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }

        // Start with inventory closed
        CloseInventory();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
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

        // CRITICAL FIX: Don't create slots if already created
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

                // Highlight equipped items
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
        // Update equipment slot UI
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

        // Refresh inventory to update highlights
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

        // Handle different item types
        switch (item.itemType)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                // Equip/Unequip
                equipment.ToggleEquip(item);
                break;

            case ItemType.Consumable:
                // Use consumable
                UseConsumable(item, slotIndex);
                break;

            case ItemType.Material:
                // Materials cannot be used directly
                Debug.Log($"[InventoryUI] {item.itemName} is a material. Use it in crafting/upgrading.");
                break;
        }
    }

    public void OnSlotRightClicked(int slotIndex)
    {
        // Right click to drop item (PC only feature for now)
        InventorySlot slot = inventory.GetSlot(slotIndex);

        if (slot == null || slot.IsEmpty()) return;

        Debug.Log($"[InventoryUI] Dropping {slot.item.itemName}");
        inventory.RemoveItemFromSlot(slotIndex, 1);
    }

    public void OnEquipmentSlotClicked(EquipmentSlot slotType)
    {
        // Unequip item
        equipment.UnequipItem(slotType);
    }

    // ============================================
    // USE CONSUMABLE
    // ============================================

    private void UseConsumable(ItemObject item, int slotIndex)
    {
        if (item.itemType != ItemType.Consumable) return;

        // Find PlayerStats
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();

        if (playerStats != null && item.hpRestore > 0)
        {
            playerStats.Heal(item.hpRestore);
            Debug.Log($"[InventoryUI] Used {item.itemName}, restored {item.hpRestore} HP");

            // Remove 1 from stack
            inventory.RemoveItemFromSlot(slotIndex, 1);
        }
    }

    // ============================================
    // TOOLTIP
    // ============================================

    public void ShowTooltip(ItemObject item)
    {
        if (tooltipPanel == null || tooltipText == null || item == null) return;

        tooltipText.text = item.GetTooltip();
        tooltipPanel.SetActive(true);

        // Position tooltip near mouse (for PC)
        // For mobile: position near touched slot
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

        // Show inventory and equipment panels
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(true);
        }

        isInventoryOpen = true;

        // Hide other canvases (e.g., HUD)
        HideOtherCanvases();

        // Refresh UI
        RefreshInventoryUI();

        // Optional: Pause game
        // Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        Debug.Log("[InventoryUI] Closing inventory");

        // Hide inventory and equipment panels
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }

        isInventoryOpen = false;

        // Show other canvases again
        ShowOtherCanvases();

        // Hide tooltip
        HideTooltip();

        // Optional: Resume game
        // Time.timeScale = 1f;
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
        // Toggle inventory with I key (PC)
        // For mobile: use UI button
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // Close with Escape
        if (Input.GetKeyDown(KeyCode.Escape) && isInventoryOpen)
        {
            CloseInventory();
        }
    }
}