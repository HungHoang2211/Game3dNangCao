using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Quản lý shop UI và giao dịch mua/bán
/// </summary>
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

    private List<ShopItemSlot> buySlots = new List<ShopItemSlot>();
    private List<ShopItemSlot> sellSlots = new List<ShopItemSlot>();

    private enum ShopTab { Buy, Sell, Upgrade }
    private ShopTab currentTab = ShopTab.Buy;

    // ============================================
    // KHỞI TẠO
    // ============================================

    private void Awake()
    {
        if (inventory == null)
            inventory = FindAnyObjectByType<InventoryManager>();

        if (equipment == null)
            equipment = FindAnyObjectByType<EquipmentManager>();
    }

    private void Start()
    {
        // Setup tab buttons
        if (buyTabButton != null)
            buyTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Buy));

        if (sellTabButton != null)
            sellTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Sell));

        if (upgradeTabButton != null)
            upgradeTabButton.onClick.AddListener(() => SwitchTab(ShopTab.Upgrade));

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        // Subscribe to currency changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged += OnGoldChanged;
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

    // ============================================
    // MỞ / ĐÓNG SHOP
    // ============================================

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            SwitchTab(ShopTab.Buy);
            UpdateGoldDisplay();

            Debug.Log("[ShopManager] Mở shop");
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log("[ShopManager] Đóng shop");
        }
    }

    // ============================================
    // CHUYỂN TAB
    // ============================================

    private void SwitchTab(ShopTab tab)
    {
        currentTab = tab;

        // Ẩn tất cả content
        if (buyContent != null) buyContent.SetActive(false);
        if (sellContent != null) sellContent.SetActive(false);
        if (upgradeContent != null) upgradeContent.SetActive(false);

        // Hiện content được chọn
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

        Debug.Log($"[ShopManager] Chuyển sang tab: {tab}");
    }

    // ============================================
    // POPULATE GRIDS
    // ============================================

    private void PopulateBuyGrid()
    {
        // Xóa slots cũ
        foreach (var slot in buySlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        buySlots.Clear();

        // Tạo slots mới cho items trong shop
        foreach (var item in shopItems)
        {
            if (item == null || item.buyPrice <= 0) continue;

            GameObject slotObj = Instantiate(shopItemSlotPrefab, buyGridContainer);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupBuySlot(item, this);
                buySlots.Add(slot);
            }
        }

        Debug.Log($"[ShopManager] Tạo {buySlots.Count} items để mua");
    }

    private void PopulateSellGrid()
    {
        // Xóa slots cũ
        foreach (var slot in sellSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        sellSlots.Clear();

        if (inventory == null) return;

        // Tạo slots cho items trong inventory có thể bán
        InventorySlot[] slots = inventory.GetAllSlots();

        foreach (var invSlot in slots)
        {
            if (invSlot.IsEmpty()) continue;
            if (invSlot.item.sellPrice <= 0) continue; // Không bán được

            GameObject slotObj = Instantiate(shopItemSlotPrefab, sellGridContainer);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupSellSlot(invSlot.item, invSlot.quantity, this);
                sellSlots.Add(slot);
            }
        }

        Debug.Log($"[ShopManager] Tạo {sellSlots.Count} items để bán");
    }

    private void PopulateUpgradeGrid()
    {
        // TODO: Implement upgrade grid
        Debug.Log("[ShopManager] Upgrade tab - sẽ làm ở step 4");
    }

    // ============================================
    // MUA / BÁN ITEMS
    // ============================================

    public void BuyItem(ItemObject item)
    {
        if (item == null) return;

        // Kiểm tra có đủ tiền không
        if (!CurrencyManager.Instance.HasEnoughGold(item.buyPrice))
        {
            Debug.LogWarning($"[ShopManager] Không đủ tiền mua {item.itemName}!");
            return;
        }

        // Kiểm tra inventory có chỗ không
        if (inventory.IsFull() && !item.isStackable)
        {
            Debug.LogWarning($"[ShopManager] Túi đồ đầy! Không thể mua {item.itemName}");
            return;
        }

        // Trừ tiền
        if (CurrencyManager.Instance.SpendGold(item.buyPrice))
        {
            // Thêm vào inventory
            inventory.AddItem(item, 1);

            Debug.Log($"[ShopManager] Đã mua {item.itemName} với giá {item.buyPrice} vàng");

            // Refresh buy grid để update button states
            RefreshBuySlots();
        }
    }

    public void SellItem(ItemObject item)
    {
        if (item == null) return;

        // Kiểm tra có item trong inventory không
        if (!inventory.HasItem(item, 1))
        {
            Debug.LogWarning($"[ShopManager] Không có {item.itemName} để bán!");
            return;
        }

        // Xóa khỏi inventory
        if (inventory.RemoveItem(item, 1))
        {
            // Cộng tiền
            CurrencyManager.Instance.AddGold(item.sellPrice);

            Debug.Log($"[ShopManager] Đã bán {item.itemName} với giá {item.sellPrice} vàng");

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
    }

    private void UpdateGoldDisplay()
    {
        if (shopGoldText != null && CurrencyManager.Instance != null)
        {
            int gold = CurrencyManager.Instance.GetGold();
            shopGoldText.text = $"Vàng của bạn: {gold:N0}";
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
}