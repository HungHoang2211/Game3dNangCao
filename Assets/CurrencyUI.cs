using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public CurrencyData goldData; // Kéo file ScriptableObject bạn vừa tạo vào đây
    public TextMeshProUGUI goldText;

    void Update()
    {
        // Cập nhật liên tục số tiền lên màn hình
        if (goldData != null && goldText != null)
        {
            goldText.text = goldData.amount.ToString("N0");
        }
    }
}