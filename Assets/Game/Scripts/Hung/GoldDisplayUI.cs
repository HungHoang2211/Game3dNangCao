using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays current gold in HUD with custom coin icon
/// </summary>
public class GoldDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image coinIcon; // ← ICON IMAGE
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Formatting")]
    [SerializeField] private string prefix = ""; // No prefix, icon replaces it
    [SerializeField] private bool showCommas = true; // Format: 1,000 instead of 1000

    private void Start()
    {
        // Subscribe to currency changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged += UpdateGoldDisplay;

            // Initial display
            UpdateGoldDisplay(CurrencyManager.Instance.GetGold());
        }
        else
        {
            Debug.LogError("[GoldDisplayUI] CurrencyManager not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
        }
    }

    private void UpdateGoldDisplay(int goldAmount)
    {
        if (goldText != null)
        {
            if (showCommas)
            {
                // Format with commas: 1,000
                goldText.text = $"{prefix}{goldAmount:N0}";
            }
            else
            {
                // Format without commas: 1000
                goldText.text = $"{prefix}{goldAmount}";
            }
        }
    }
}