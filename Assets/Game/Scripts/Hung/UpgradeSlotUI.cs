using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI slot cho equipment upgrade
/// Hiển thị equipped item với upgrade info
/// </summary>
public class UpgradeSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI currentStatsText;
    [SerializeField] private TextMeshProUGUI upgradedStatsText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject emptyState; // Hiện khi không có item

    [Header("Colors")]
    [SerializeField] private Color canUpgradeColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color cannotUpgradeColor = Color.gray;
    [SerializeField] private Color maxLevelColor = new Color(1f, 0.84f, 0f); // Gold

    private ItemObject item;
    private EquipmentSlot slotType;
    private ShopManager shopManager;

    /// <summary>
    /// Setup slot với equipped item
    /// </summary>
    public void Setup(EquipmentSlot slot, ItemObject equippedItem, ShopManager manager)
    {
        slotType = slot;
        item = equippedItem;
        shopManager = manager;

        UpdateDisplay();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }
    }

    private void UpdateDisplay()
    {
        // Nếu không có item equipped
        if (item == null)
        {
            ShowEmptyState();
            return;
        }

        HideEmptyState();

        // Icon
        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = Color.white;
        }

        // Tên + level
        if (itemNameText != null)
        {
            itemNameText.text = item.GetDisplayName();
        }

        // Level indicator
        if (levelText != null)
        {
            levelText.text = $"Level: +{item.upgradeLevel} / +{item.maxUpgradeLevel}";
        }

        // Current stats
        if (currentStatsText != null)
        {
            string statsStr = "";
            if (item.damageBonus > 0)
                statsStr += $"DMG: {item.GetUpgradedDamage():F0}\n";
            if (item.defenseBonus > 0)
                statsStr += $"DEF: {item.GetUpgradedDefense():F0}\n";
            if (item.speedBonus > 0)
                statsStr += $"SPD: {item.GetUpgradedSpeed():F0}\n";

            currentStatsText.text = "Hiện tại:\n" + statsStr;
        }

        // Upgraded stats (preview)
        if (upgradedStatsText != null)
        {
            if (item.CanUpgrade())
            {
                // Tạm thời +1 level để preview
                int tempLevel = item.upgradeLevel + 1;
                float previewDamage = item.damageBonus * (1f + (tempLevel * 0.1f));
                float previewDefense = item.defenseBonus * (1f + (tempLevel * 0.1f));
                float previewSpeed = item.speedBonus * (1f + (tempLevel * 0.1f));

                string statsStr = "";
                if (item.damageBonus > 0)
                    statsStr += $"DMG: {previewDamage:F0} (+{(previewDamage - item.GetUpgradedDamage()):F0})\n";
                if (item.defenseBonus > 0)
                    statsStr += $"DEF: {previewDefense:F0} (+{(previewDefense - item.GetUpgradedDefense()):F0})\n";
                if (item.speedBonus > 0)
                    statsStr += $"SPD: {previewSpeed:F0} (+{(previewSpeed - item.GetUpgradedSpeed()):F0})\n";

                upgradedStatsText.text = "Sau khi nâng:\n" + statsStr;
                upgradedStatsText.color = Color.green;
            }
            else
            {
                upgradedStatsText.text = "MAX LEVEL!";
                upgradedStatsText.color = maxLevelColor;
            }
        }

        // Cost
        if (costText != null)
        {
            if (item.CanUpgrade())
            {
                int cost = item.GetUpgradeCost();
                costText.text = $"Chi phí: {cost} Vàng";
            }
            else
            {
                costText.text = "Đã max level";
            }
        }

        // Button
        if (upgradeButton != null)
        {
            if (!item.CanUpgrade())
            {
                // Max level
                upgradeButton.interactable = false;
                if (buttonText != null) buttonText.text = "MAX";

                var colors = upgradeButton.colors;
                colors.normalColor = maxLevelColor;
                upgradeButton.colors = colors;
            }
            else
            {
                // Check if can afford
                int cost = item.GetUpgradeCost();
                bool canAfford = CurrencyManager.Instance != null &&
                                 CurrencyManager.Instance.HasEnoughGold(cost);

                upgradeButton.interactable = canAfford;
                if (buttonText != null) buttonText.text = "NÂNG CẤP";

                var colors = upgradeButton.colors;
                colors.normalColor = canAfford ? canUpgradeColor : cannotUpgradeColor;
                upgradeButton.colors = colors;
            }
        }
    }

    private void ShowEmptyState()
    {
        if (emptyState != null) emptyState.SetActive(true);
        if (itemIcon != null) itemIcon.gameObject.SetActive(false);
        if (itemNameText != null) itemNameText.gameObject.SetActive(false);
        if (currentStatsText != null) currentStatsText.gameObject.SetActive(false);
        if (upgradedStatsText != null) upgradedStatsText.gameObject.SetActive(false);
        if (costText != null) costText.gameObject.SetActive(false);
        if (levelText != null) levelText.gameObject.SetActive(false);
        if (upgradeButton != null) upgradeButton.gameObject.SetActive(false);
    }

    private void HideEmptyState()
    {
        if (emptyState != null) emptyState.SetActive(false);
        if (itemIcon != null) itemIcon.gameObject.SetActive(true);
        if (itemNameText != null) itemNameText.gameObject.SetActive(true);
        if (currentStatsText != null) currentStatsText.gameObject.SetActive(true);
        if (upgradedStatsText != null) upgradedStatsText.gameObject.SetActive(true);
        if (costText != null) costText.gameObject.SetActive(true);
        if (levelText != null) levelText.gameObject.SetActive(true);
        if (upgradeButton != null) upgradeButton.gameObject.SetActive(true);
    }

    private void OnUpgradeClicked()
    {
        if (shopManager != null && item != null)
        {
            shopManager.UpgradeItem(slotType, item);
        }
    }

    /// <summary>
    /// Refresh display (khi gold thay đổi)
    /// </summary>
    public void Refresh()
    {
        UpdateDisplay();
    }
}