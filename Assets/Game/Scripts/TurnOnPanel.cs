using UnityEngine;

public class TurnOnPanel : MonoBehaviour
{
    [Header("UI Panel")]
    public CanvasGroup Panel; // Kéo Inventory_Panel vào đây

    private bool isOpen = false;

    void Start()
    {
        // Khởi đầu: Tắt túi đồ
        ClosePanel();
    }

    // Hàm này dùng để gọi từ Nút bấm (Button) trên màn hình Mobile
    public void TogglePanel()
    {
        if (isOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        isOpen = true;
        Panel.alpha = 1f;          // Hiện hình ảnh
        Panel.blocksRaycasts = true;   // Cho phép bấm vào các ô
        Panel.interactable = true;     // Cho phép tương tác

        // Tùy chọn: Dừng thời gian game khi mở túi (nếu là game Single Player)
        Time.timeScale = 0f; 
    }

    public void ClosePanel()
    {
        isOpen = false;
        Panel.alpha = 0f;          // Ẩn hình ảnh
        Panel.blocksRaycasts = false;  // Không cho bấm xuyên qua
        Panel.interactable = false;    // Khóa tương tác

        Time.timeScale = 1f; 
    }
}
