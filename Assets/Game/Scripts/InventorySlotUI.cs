using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for a single inventory slot
/// WITH SIMPLE DRAG AND DROP
/// </summary>
public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,    // ADD
    IDragHandler,         // ADD
    IEndDragHandler       // ADD
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

    // Drag and drop
    private GameObject draggedIcon;
    private Canvas canvas;
    private bool isDragging = false;

    // ============================================
    // INITIALIZATION
    // ============================================

    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        inventoryUI = ui;

        // Find canvas for drag operations
        canvas = GetComponentInParent<Canvas>();

        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(false);
        }

        UpdateSlotUI(null);
    }

    // ============================================
    // UPDATE UI
    // ============================================

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

    public void SetHighlight(bool highlighted)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(highlighted);
        }
    }

    // ============================================
    // POINTER EVENTS
    // ============================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotData == null || slotData.IsEmpty()) return;
        if (isDragging) return; // Don't process click if was dragging

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
    // DRAG AND DROP
    // ============================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData == null || slotData.IsEmpty()) return;

        isDragging = true;

        // Create dragged icon
        CreateDragIcon();

        Debug.Log($"[InventorySlotUI] Dragging {slotData.item.itemName}");
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
            // Check if dropped on another inventory slot
            InventorySlotUI targetSlot = droppedOn.GetComponent<InventorySlotUI>();

            if (targetSlot != null && targetSlot != this)
            {
                // Swap with another inventory slot
                inventoryUI.SwapInventorySlots(slotIndex, targetSlot.GetSlotIndex());
            }
            else
            {
                // Check if dropped on equipment slot
                EquipmentSlotUI equipSlot = droppedOn.GetComponent<EquipmentSlotUI>();

                if (equipSlot != null)
                {
                    // Try to equip to equipment slot
                    inventoryUI.EquipToSlot(slotIndex, equipSlot.GetSlotType());
                }
            }
        }

        isDragging = false;

        Debug.Log("[InventorySlotUI] Drag ended");
    }

    private void CreateDragIcon()
    {
        if (slotData == null || slotData.IsEmpty()) return;

        // Create temporary GameObject for drag icon
        draggedIcon = new GameObject("DragIcon");

        // Setup as UI element
        RectTransform rectTransform = draggedIcon.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(80, 80);

        // Add image
        Image img = draggedIcon.AddComponent<Image>();
        img.sprite = slotData.item.icon;
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

    public InventorySlot GetSlotData() => slotData;
    public int GetSlotIndex() => slotIndex;
}