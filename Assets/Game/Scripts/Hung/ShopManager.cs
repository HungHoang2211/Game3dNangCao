using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private EquipmentManager equipment;

    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Tabs")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;
    [SerializeField] private Button upgradeTabButton;

    [Header("Content Areas")]
    [SerializeField] private GameObject buyContent;
    [SerializeField] private GameObject sellContent;
    [SerializeField] private GameObject upgradeContent;

    [Header("Shop Item Grid")]
    [SerializeField] private Transform buyGridContainer;
    [SerializeField] private Transform sellGridContainer;
    [SerializeField] private GameObject shopItemSlotPrefab;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI shopGoldText;
    [SerializeField] private Button closeButton;

    [Header("Items To Sell (Shop Inventory)")]
    [SerializeField] private List<ItemObject> shopItems = new List<ItemObject>();

    [Header("Other Canvases to Hide")]
    [Tooltip("Canvas(es) to hide when shop is open (e.g., HUD Canvas)")]
    [SerializeField] private GameObject[] canvasesToHide;

    private List<ShopItemSlot> buySlots = new List<ShopItemSlot>();
    private List<ShopItemSlot> sellSlots = new List<ShopItemSlot>();

    private enum ShopTab { Buy, Sell, Upgrade }
    private ShopTab currentTab = ShopTab.Buy;

    [Header("Upgrade Slots")]
    [SerializeField] private UpgradeSlotUI weaponUpgradeSlot;
    [SerializeField] private UpgradeSlotUI armorUpgradeSlot;
    [SerializeField] private UpgradeSlotUI accessoryUpgradeSlot;


    // ============================================
    // KHỞI TẠO
    // ============================================

    private void Awake()
    {
        Debug.Log("[ShopManager] Awake - Initializing...");

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


        // Setup tab buttons
        if (buyTabButton != null)
            buyTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Buy));
        else
            Debug.LogError("[ShopManager] ❌ buyTabButton is NULL!");

        if (sellTabButton != null)
            sellTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Sell));
        else
            Debug.LogError("[ShopManager] ❌ sellTabButton is NULL!");

        if (upgradeTabButton != null)
            upgradeTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Upgrade));
        else
            Debug.LogError("[ShopManager] ❌ upgradeTabButton is NULL!");

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
        else
            Debug.LogError("[ShopManager] ❌ closeButton is NULL!");

        // Subscribe to currency changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged += OnGoldChanged;
        }
        else
        {
            Debug.LogError("[ShopManager] ❌ CurrencyManager.Instance is NULL!");
        }

        // Đóng shop ban đầu
        CloseShop();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= OnGoldChanged;
        }
    }

    public void OpenShop()
    {
        HideOtherCanvases();

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            SwitchTab(ShopTab.Buy);
            UpdateGoldDisplay();
            Time.timeScale = 0f;

        }
        else
        {
            Debug.LogError("[ShopManager] Cannot open shop - shopPanel is NULL!");
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            ShowOtherCanvases();
            Time.timeScale = 1f;
        }
    }
    private void SwitchTab(ShopTab tab)
    {
        Debug.Log($"[ShopManager] SwitchTab to: {tab}");

        currentTab = tab;

        if (buyContent != null) buyContent.SetActive(false);
        if (sellContent != null) sellContent.SetActive(false);
        if (upgradeContent != null) upgradeContent.SetActive(false);

        switch (tab)
        {
            case ShopTab.Buy:
                if (buyContent != null) buyContent.SetActive(true);
                PopulateBuyGrid();
                break;

            case ShopTab.Sell:
                if (sellContent != null) sellContent.SetActive(true);
                PopulateSellGrid();
                break;

            case ShopTab.Upgrade:
                if (upgradeContent != null) upgradeContent.SetActive(true);
                PopulateUpgradeGrid();
                break;
        }
    }

    private void PopulateBuyGrid()
    {
        Debug.Log($"[ShopManager] === PopulateBuyGrid START ===");
        Debug.Log($"[ShopManager] shopItems count: {shopItems.Count}");
        Debug.Log($"[ShopManager] buyGridContainer: {(buyGridContainer != null ? "OK" : "NULL")}");
        Debug.Log($"[ShopManager] shopItemSlotPrefab: {(shopItemSlotPrefab != null ? "OK" : "NULL")}");

        foreach (var slot in buySlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        buySlots.Clear();

        if (buyGridContainer == null)
        {
            Debug.LogError("[ShopManager] ❌ buyGridContainer is NULL! Check Inspector assignments!");
            return;
        }

        if (shopItemSlotPrefab == null)
        {
            Debug.LogError("[ShopManager] ❌ shopItemSlotPrefab is NULL! Assign prefab in Inspector!");
            return;
        }

        // Tạo slots mới cho items trong shop
        int createdCount = 0;
        foreach (var item in shopItems)
        {
            if (item == null)
            {
                Debug.LogWarning("[ShopManager] ⚠️ Item in shopItems list is NULL!");
                continue;
            }

            Debug.Log($"[ShopManager] Processing item: {item.itemName}");
            Debug.Log($"  - buyPrice: {item.buyPrice}");
            Debug.Log($"  - sellPrice: {item.sellPrice}");

            if (item.buyPrice <= 0)
            {
                Debug.LogWarning($"[ShopManager] ⚠️ {item.itemName} has buyPrice = {item.buyPrice}, skipping");
                continue;
            }

            GameObject slotObj = Instantiate(shopItemSlotPrefab, buyGridContainer);
            if (slotObj == null)
            {
                Debug.LogError("[ShopManager] ❌ Failed to instantiate prefab!");
                continue;
            }

            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupBuySlot(item, this);
                buySlots.Add(slot);
                createdCount++;
                Debug.Log($"[ShopManager] ✅ Created slot #{createdCount} for {item.itemName}");
            }
            else
            {
                Debug.LogError($"[ShopManager] ❌ ShopItemSlot component not found on instantiated prefab!");
                Destroy(slotObj);
            }
        }

        Debug.Log($"[ShopManager] === PopulateBuyGrid END === Created {buySlots.Count} slots");
    }

    private void PopulateSellGrid()
    {
        Debug.Log("[ShopManager] === PopulateSellGrid START ===");

        // Xóa slots cũ
        foreach (var slot in sellSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        sellSlots.Clear();

        if (inventory == null)
        {
            Debug.LogError("[ShopManager] ❌ Inventory is NULL!");
            return;
        }

        if (sellGridContainer == null)
        {
            Debug.LogError("[ShopManager] ❌ sellGridContainer is NULL!");
            return;
        }

        if (shopItemSlotPrefab == null)
        {
            Debug.LogError("[ShopManager] ❌ shopItemSlotPrefab is NULL!");
            return;
        }

        // Tạo slots cho items trong inventory có thể bán
        InventorySlot[] slots = inventory.GetAllSlots();

        Debug.Log($"[ShopManager] Inventory has {slots.Length} total slots");

        int itemsFound = 0;
        int itemsCreated = 0;

        foreach (var invSlot in slots)
        {
            if (invSlot.IsEmpty())
            {
                continue;
            }

            itemsFound++;
            Debug.Log($"[ShopManager] Found item #{itemsFound}: {invSlot.item.itemName}");
            Debug.Log($"  - Quantity: {invSlot.quantity}");
            Debug.Log($"  - SellPrice: {invSlot.item.sellPrice}");

            if (invSlot.item.sellPrice <= 0)
            {
                Debug.LogWarning($"[ShopManager] ⚠️ {invSlot.item.itemName} cannot be sold (sellPrice = 0)");
                continue;
            }

            GameObject slotObj = Instantiate(shopItemSlotPrefab, sellGridContainer);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupSellSlot(invSlot.item, invSlot.quantity, this);
                sellSlots.Add(slot);
                itemsCreated++;
                Debug.Log($"[ShopManager] ✅ Created sell slot #{itemsCreated} for {invSlot.item.itemName}");
            }
            else
            {
                Debug.LogError($"[ShopManager] ❌ ShopItemSlot component not found!");
                Destroy(slotObj);
            }
        }

        Debug.Log($"[ShopManager] === PopulateSellGrid END ===");
        Debug.Log($"[ShopManager] Items found in inventory: {itemsFound}");
        Debug.Log($"[ShopManager] Sellable slots created: {sellSlots.Count}");
    }

    private void PopulateUpgradeGrid()
    {
        Debug.Log("[ShopManager] Populating Upgrade Grid");

        if (equipment == null)
        {
            Debug.LogError("[ShopManager] Equipment is NULL!");
            return;
        }

        // Setup weapon slot
        if (weaponUpgradeSlot != null)
        {
            ItemObject weapon = equipment.GetWeapon();
            weaponUpgradeSlot.Setup(EquipmentSlot.Weapon, weapon, this);
        }

        // Setup armor slot
        if (armorUpgradeSlot != null)
        {
            ItemObject armor = equipment.GetArmor();
            armorUpgradeSlot.Setup(EquipmentSlot.Armor, armor, this);
        }

        // Setup accessory slot
        if (accessoryUpgradeSlot != null)
        {
            ItemObject accessory = equipment.GetAccessory();
            accessoryUpgradeSlot.Setup(EquipmentSlot.Accessory, accessory, this);
        }
    }

    /// <summary>
    /// Upgrade equipped item
    /// </summary>
    public void UpgradeItem(EquipmentSlot slot, ItemObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("[ShopManager] Cannot upgrade null item!");
            return;
        }

        if (!item.CanUpgrade())
        {
            Debug.LogWarning($"[ShopManager] {item.itemName} is already max level!");
            return;
        }

        int cost = item.GetUpgradeCost();

        // Check if has enough gold
        if (!CurrencyManager.Instance.HasEnoughGold(cost))
        {
            Debug.LogWarning($"[ShopManager] Không đủ vàng để nâng cấp! Cần {cost} vàng");
            return;
        }

        // Spend gold
        if (CurrencyManager.Instance.SpendGold(cost))
        {
            // Store old stats for comparison
            float oldDamage = item.GetUpgradedDamage();
            float oldDefense = item.GetUpgradedDefense();

            // IMPORTANT: Need to update equipment stats BEFORE upgrade
            // Remove old bonuses
            if (equipment.IsEquipped(item))
            {
                equipment.UnequipItem(slot);
            }

            // Upgrade the item
            item.Upgrade();

            // Re-equip with new stats
            equipment.EquipItem(item);

            Debug.Log($"[ShopManager] Nâng cấp {item.itemName} lên +{item.upgradeLevel}!");
            Debug.Log($"  Old DMG: {oldDamage:F0} → New DMG: {item.GetUpgradedDamage():F0}");

            // Refresh upgrade grid
            PopulateUpgradeGrid();
        }
    }

    // ============================================
    // MUA / BÁN ITEMS
    // ============================================

    public void BuyItem(ItemObject item)
    {
        if (item == null) return;

        Debug.Log($"[ShopManager] BuyItem: {item.itemName}, Price: {item.buyPrice}");

        // Kiểm tra có đủ tiền không
        if (!CurrencyManager.Instance.HasEnoughGold(item.buyPrice))
        {
            Debug.LogWarning($"[ShopManager] ❌ Không đủ tiền mua {item.itemName}!");
            return;
        }

        // Kiểm tra inventory có chỗ không
        if (inventory.IsFull() && !item.isStackable)
        {
            Debug.LogWarning($"[ShopManager] ❌ Túi đồ đầy! Không thể mua {item.itemName}");
            return;
        }

        // Trừ tiền
        if (CurrencyManager.Instance.SpendGold(item.buyPrice))
        {
            // Thêm vào inventory
            inventory.AddItem(item, 1);

            Debug.Log($"[ShopManager] ✅ Đã mua {item.itemName} với giá {item.buyPrice} vàng");

            // Refresh buy grid để update button states
            RefreshBuySlots();
        }
    }

    public void SellItem(ItemObject item)
    {
        if (item == null) return;

        Debug.Log($"[ShopManager] SellItem: {item.itemName}, Price: {item.sellPrice}");

        // Kiểm tra có item trong inventory không
        if (!inventory.HasItem(item, 1))
        {
            Debug.LogWarning($"[ShopManager] ❌ Không có {item.itemName} để bán!");
            return;
        }

        // Xóa khỏi inventory
        if (inventory.RemoveItem(item, 1))
        {
            // Cộng tiền
            CurrencyManager.Instance.AddGold(item.sellPrice);

            Debug.Log($"[ShopManager] ✅ Đã bán {item.itemName} với giá {item.sellPrice} vàng");

            // Refresh sell grid
            PopulateSellGrid();
        }
    }

    // ============================================
    // UI UPDATES
    // ============================================

    private void OnGoldChanged(int newGold)
    {
        UpdateGoldDisplay();
        RefreshBuySlots();
        RefreshUpgradeSlots();
    }
    private void RefreshUpgradeSlots()
    {
        if (weaponUpgradeSlot != null) weaponUpgradeSlot.Refresh();
        if (armorUpgradeSlot != null) armorUpgradeSlot.Refresh();
        if (accessoryUpgradeSlot != null) accessoryUpgradeSlot.Refresh();
    }
    private void UpdateGoldDisplay()
    {
        if (shopGoldText != null && CurrencyManager.Instance != null)
        {
            int gold = CurrencyManager.Instance.GetGold();
            shopGoldText.text = $"{gold:N0}";
        }
    }

    private void RefreshBuySlots()
    {
        foreach (var slot in buySlots)
        {
            if (slot != null)
            {
                slot.RefreshAffordability();
            }
        }
    }

    private void HideOtherCanvases()
    {
        if (canvasesToHide == null) return;

        foreach (GameObject canvas in canvasesToHide)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }
    }

    private void ShowOtherCanvases()
    {
        if (canvasesToHide == null) return;

        foreach (GameObject canvas in canvasesToHide)
        {
            if (canvas != null)
                canvas.SetActive(true);
        }
    }

}