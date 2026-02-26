using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    private ItemStatus currentItem;
    private Transform originalParent;
    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(ItemStatus item)
    {
        currentItem = item;
        icon.sprite = item.itemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public ItemStatus GetItem() => currentItem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        Debug.Log("OnBeginDrag: Bắt đầu kéo item " + currentItem.itemName);

        originalParent = transform.parent;
        transform.SetParent(canvas.transform); // đưa icon ra ngoài canvas để kéo
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: Thả item " + (currentItem != null ? currentItem.itemName : "null"));

        // Nếu thả vào slot hợp lệ thì EquipmentSlotUI sẽ xử lý
        // Nếu không, mới reset về parent
        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<EquipmentSlotUI>() == null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }

}
