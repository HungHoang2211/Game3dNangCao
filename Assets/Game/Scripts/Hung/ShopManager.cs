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
        SFXManager.Instance.ClickButton();
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
        SFXManager.Instance.ClickButton();
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            ShowOtherCanvases();
            Time.timeScale = 1f;
        }
    }
    private void SwitchTab(ShopTab tab)
    {

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

        foreach (var slot in buySlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        buySlots.Clear();

        if (buyGridContainer == null)
        {
            return;
        }

        if (shopItemSlotPrefab == null)
        {
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

            if (item.buyPrice <= 0)
            {
                continue;
            }

            GameObject slotObj = Instantiate(shopItemSlotPrefab, buyGridContainer);
            if (slotObj == null)
            {
                continue;
            }

            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupBuySlot(item, this);
                buySlots.Add(slot);
                createdCount++;
            }
            else
            {
                Destroy(slotObj);
            }
        }
    }

    private void PopulateSellGrid()
    {
        // Xóa slots cũ
        foreach (var slot in sellSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        sellSlots.Clear();

        if (inventory == null)
        {
            return;
        }

        if (sellGridContainer == null)
        {
            return;
        }

        if (shopItemSlotPrefab == null)
        {
            return;
        }

        // Tạo slots cho items trong inventory có thể bán
        InventorySlot[] slots = inventory.GetAllSlots();


        int itemsFound = 0;
        int itemsCreated = 0;

        foreach (var invSlot in slots)
        {
            if (invSlot.IsEmpty())
            {
                continue;
            }

            itemsFound++;

            if (invSlot.item.sellPrice <= 0)
            {
                continue;
            }

            GameObject slotObj = Instantiate(shopItemSlotPrefab, sellGridContainer);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            if (slot != null)
            {
                slot.SetupSellSlot(invSlot.item, invSlot.quantity, this);
                sellSlots.Add(slot);
                itemsCreated++;
            }
            else
            {
                Destroy(slotObj);
            }
        }

    }

    private void PopulateUpgradeGrid()
    {

        if (equipment == null)
        {
            return;
        }

        if (weaponUpgradeSlot != null)
        {
            ItemObject weapon = equipment.GetWeapon();
            weaponUpgradeSlot.Setup(EquipmentSlot.Weapon, weapon, this);
        }

        if (armorUpgradeSlot != null)
        {
            ItemObject armor = equipment.GetArmor();
            armorUpgradeSlot.Setup(EquipmentSlot.Armor, armor, this);
        }
        if (accessoryUpgradeSlot != null)
        {
            ItemObject accessory = equipment.GetAccessory();
            accessoryUpgradeSlot.Setup(EquipmentSlot.Accessory, accessory, this);
        }
    }

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

        if (!CurrencyManager.Instance.HasEnoughGold(cost))
        {
            return;
        }


        if (CurrencyManager.Instance.SpendGold(cost))
        {
            float oldDamage = item.GetUpgradedDamage();
            float oldDefense = item.GetUpgradedDefense();


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


    public void BuyItem(ItemObject item)
    {
        if (item == null) return;

        if (!CurrencyManager.Instance.HasEnoughGold(item.buyPrice))
        {
            Debug.LogWarning($"[ShopManager] ❌ Không đủ tiền mua {item.itemName}!");
            return;
        }

        if (inventory.IsFull() && !item.isStackable)
        {
            Debug.LogWarning($"[ShopManager] ❌ Túi đồ đầy! Không thể mua {item.itemName}");
            return;
        }

        if (CurrencyManager.Instance.SpendGold(item.buyPrice))
        {
            inventory.AddItem(item, 1);

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