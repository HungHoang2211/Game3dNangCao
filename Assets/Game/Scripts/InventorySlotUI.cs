using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for a single inventory slot
/// Attach to each slot prefab
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image highlightImage;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.8f, 1f);
    [SerializeField] private Color equippedColor = new Color(0.5f, 1f, 0.5f, 1f);

    private InventorySlot slotData;
    private int slotIndex;
    private InventoryUI inventoryUI;

    // ============================================
    // INITIALIZATION
    // ============================================

    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        inventoryUI = ui;

        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }

        UpdateSlotUI(null);
    }

    // ============================================
    // UPDATE UI
    // ============================================

    /// <summary>
    /// Update slot visual based on data
    /// </summary>
    public void UpdateSlotUI(InventorySlot slot)
    {
        slotData = slot;

        if (slot == null || slot.IsEmpty())
        {
            // Empty slot
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.color = emptyColor;
            }

            if (quantityText != null)
            {
                quantityText.text = "";
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }
        else
        {
            // Slot has item
            if (itemIcon != null)
            {
                itemIcon.sprite = slot.item.icon;
                itemIcon.color = Color.white;
            }

            if (quantityText != null)
            {
                if (slot.quantity > 1)
                {
                    quantityText.text = slot.quantity.ToString();
                }
                else
                {
                    quantityText.text = "";
                }
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = normalColor;
            }
        }
    }

    /// <summary>
    /// Highlight slot (for equipped items)
    /// </summary>
    public void SetHighlight(bool highlighted)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(highlighted);
        }
    }

    // ============================================
    // POINTER EVENTS (Mouse + Touch compatible)
    // ============================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotData == null || slotData.IsEmpty()) return;

        // Left click / Touch = Use/Equip item
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryUI.OnSlotClicked(slotIndex);
        }
        // Right click = Drop/Remove item (PC only for now)
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryUI.OnSlotRightClicked(slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }

        if (slotData != null && !slotData.IsEmpty())
        {
            inventoryUI.ShowTooltip(slotData.item);
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

    public InventorySlot GetSlotData() => slotData;
    public int GetSlotIndex() => slotIndex;
}