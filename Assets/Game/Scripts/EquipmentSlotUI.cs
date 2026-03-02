using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for equipment slots (Weapon, Armor, Accessory)
/// </summary>
public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuration")]
    [SerializeField] private EquipmentSlot slotType;

    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.8f, 1f);

    private ItemObject equippedItem;
    private InventoryUI inventoryUI;

    // ============================================
    // INITIALIZATION
    // ============================================

    public void Initialize(InventoryUI ui)
    {
        inventoryUI = ui;

        if (slotLabel != null)
        {
            slotLabel.text = slotType.ToString();
        }

        UpdateSlotUI(null);
    }

    // ============================================
    // UPDATE UI
    // ============================================

    public void UpdateSlotUI(ItemObject item)
    {
        equippedItem = item;

        if (item == null)
        {
            // Empty slot
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.color = emptyColor;
            }
        }
        else
        {
            // Equipped item
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.color = Color.white;
            }
        }
    }

    // ============================================
    // POINTER EVENTS
    // ============================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (equippedItem != null)
        {
            // Unequip item
            inventoryUI.OnEquipmentSlotClicked(slotType);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }

        if (equippedItem != null)
        {
            inventoryUI.ShowTooltip(equippedItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }

        inventoryUI.HideTooltip();
    }

    // ============================================
    // GETTERS
    // ============================================

    public EquipmentSlot GetSlotType() => slotType;
    public ItemObject GetEquippedItem() => equippedItem;
}