using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hiển thị 1 item trong shop (để mua hoặc bán)
/// </summary>
public class ShopItemSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Colors")]
    [SerializeField] private Color buyColor = new Color(0.2f, 0.8f, 0.2f); // Xanh lá
    [SerializeField] private Color sellColor = new Color(0.8f, 0.6f, 0.2f); // Vàng
    [SerializeField] private Color cannotAffordColor = Color.gray;

    private ItemObject item;
    private int price;
    private bool isBuyMode; // true = mua, false = bán
    private ShopManager shopManager;

    /// <summary>
    /// Khởi tạo slot với item để MUA
    /// </summary>
    public void SetupBuySlot(ItemObject itemData, ShopManager manager)
    {
        item = itemData;
        price = itemData.buyPrice;
        isBuyMode = true;
        shopManager = manager;

        UpdateDisplay();

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnBuyClicked);
        }
    }

    /// <summary>
    /// Khởi tạo slot với item để BÁN
    /// </summary>
    public void SetupSellSlot(ItemObject itemData, int quantity, ShopManager manager)
    {
        item = itemData;
        price = itemData.sellPrice;
        isBuyMode = false;
        shopManager = manager;

        UpdateDisplay();

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnSellClicked);
        }
    }

    private void UpdateDisplay()
    {
        if (item == null) return;

        // Icon
        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = Color.white;
        }

        // Tên item
        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        // Giá
        if (priceText != null)
        {
            priceText.text = $"{price} Vàng";
        }

        // Button text
        if (buttonText != null)
        {
            buttonText.text = isBuyMode ? "MUA" : "BÁN";
        }

        // Button color
        if (actionButton != null)
        {
            var colors = actionButton.colors;
            colors.normalColor = isBuyMode ? buyColor : sellColor;
            actionButton.colors = colors;

            // Kiểm tra có đủ tiền mua không
            if (isBuyMode && CurrencyManager.Instance != null)
            {
                bool canAfford = CurrencyManager.Instance.HasEnoughGold(price);
                actionButton.interactable = canAfford;

                if (!canAfford)
                {
                    colors.normalColor = cannotAffordColor;
                    actionButton.colors = colors;
                }
            }
        }
    }

    private void OnBuyClicked()
    {
        if (shopManager != null && item != null)
        {
            shopManager.BuyItem(item);
            UpdateDisplay(); // Cập nhật lại button state
        }
    }

    private void OnSellClicked()
    {
        if (shopManager != null && item != null)
        {
            shopManager.SellItem(item);
        }
    }

    /// <summary>
    /// Refresh display (khi gold thay đổi)
    /// </summary>
    public void RefreshAffordability()
    {
        UpdateDisplay();
    }
}