using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for equipment slots (Weapon, Armor, Accessory)
/// WITH SIMPLE DRAG AND DROP
/// </summary>
public class EquipmentSlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,    // ADD
    IDragHandler,         // ADD
    IEndDragHandler       // ADD
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

    // Drag and drop
    private GameObject draggedIcon;
    private Canvas canvas;
    private bool isDragging = false;

    // ============================================
    // INITIALIZATION
    // ============================================

    public void Initialize(InventoryUI ui)
    {
        inventoryUI = ui;

        // Find canvas for drag operations
        canvas = GetComponentInParent<Canvas>();

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
        if (isDragging) return; // Don't click if was dragging

        if (equippedItem != null)
        {
            // Click to unequip
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
    // DRAG FROM EQUIPMENT SLOT
    // ============================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem == null) return;

        isDragging = true;

        // Create dragged icon
        CreateDragIcon();

        Debug.Log($"[EquipmentSlotUI] Dragging {equippedItem.itemName} from {slotType}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            draggedIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Clean up dragged icon
        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
        }

        // Check where we dropped
        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;

        if (droppedOn != null)
        {
            // Check if dropped on inventory slot
            InventorySlotUI invSlot = droppedOn.GetComponent<InventorySlotUI>();

            if (invSlot != null)
            {
                // Unequip to inventory slot
                inventoryUI.UnequipToSlot(slotType, invSlot.GetSlotIndex());
            }
            else
            {
                // Check if dropped on another equipment slot
                EquipmentSlotUI equipSlot = droppedOn.GetComponent<EquipmentSlotUI>();

                if (equipSlot != null && equipSlot != this)
                {
                    // Swap equipment slots (if compatible)
                    inventoryUI.SwapEquipmentSlots(slotType, equipSlot.GetSlotType());
                }
            }
        }

        isDragging = false;
    }

    private void CreateDragIcon()
    {
        if (equippedItem == null) return;

        // Create temporary GameObject for drag icon
        draggedIcon = new GameObject("DragIcon");

        // Setup as UI element
        RectTransform rectTransform = draggedIcon.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100);

        // Add image
        Image img = draggedIcon.AddComponent<Image>();
        img.sprite = equippedItem.icon;
        img.raycastTarget = false;

        // Set parent to canvas root
        if (canvas != null)
        {
            draggedIcon.transform.SetParent(canvas.transform);
            draggedIcon.transform.SetAsLastSibling();
        }
        else
        {
            draggedIcon.transform.SetParent(transform.root);
        }

        // Make slightly transparent
        Color iconColor = img.color;
        iconColor.a = 0.7f;
        img.color = iconColor;
    }

    // ============================================
    // GETTERS
    // ============================================

    public EquipmentSlot GetSlotType() => slotType;
    public ItemObject GetEquippedItem() => equippedItem;


}