using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("UI Panel")]
    public CanvasGroup inventoryCanvasGroup; // Kéo Inventory_Panel vào đây

    private bool isOpen = false;

    void Start()
    {
        // Khởi đầu: Tắt túi đồ
        CloseInventory();
    }

    // Hàm này dùng để gọi từ Nút bấm (Button) trên màn hình Mobile
    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryCanvasGroup.alpha = 1f;          // Hiện hình ảnh
        inventoryCanvasGroup.blocksRaycasts = true;   // Cho phép bấm vào các ô
        inventoryCanvasGroup.interactable = true;     // Cho phép tương tác

        // Tùy chọn: Dừng thời gian game khi mở túi (nếu là game Single Player)
        Time.timeScale = 0f; 
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryCanvasGroup.alpha = 0f;          // Ẩn hình ảnh
        inventoryCanvasGroup.blocksRaycasts = false;  // Không cho bấm xuyên qua
        inventoryCanvasGroup.interactable = false;    // Khóa tương tác

        Time.timeScale = 1f; 
    }
}