using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu bạn dùng TextMeshPro cho số lượng

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // Tham chiếu tới Script Inventory lúc nãy
    public Transform slotParent; // Kéo "GridPanel" vào đây
    public InventorySlotUI[] uiSlots; // Mảng chứa các ô UI

    [Header("Info Panel (Khung trắng)")]
    public Image infoIcon;
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoDescription;

    void Start()
    {
        // Đăng ký sự kiện: Khi inventory thay đổi thì gọi hàm UpdateUI
        inventory.onInventoryChanged += UpdateUI;

        // Khởi tạo danh sách UI Slots từ Grid
        uiSlots = slotParent.GetComponentsInChildren<InventorySlotUI>();

        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < inventory.slots.Count)
            {
                // Nếu ô này có item, hiển thị nó lên
                uiSlots[i].SetItem(inventory.slots[i]);
            }
            else
            {
                // Nếu không, để ô trống
                uiSlots[i].ClearSlot();
            }
        }
    }

    // Hàm này được gọi khi bạn chạm vào một ô bất kỳ
    public void DisplayItemInfo(ItemStatus data)
    {
        if (data == null) return;

        infoIcon.sprite = data.itemIcon;
        infoIcon.enabled = true;
        infoName.text = data.itemName;

        // Hiển thị các chỉ số (Stats)
        infoDescription.text = $"Sát thương: {data.damage}\n" +
                               $"Tốc độ đánh: {data.attackSpeed}\n" +
                               $"Tầm đánh: {data.attackRange}";
    }
}