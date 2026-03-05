using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD button for using health potions from inventory.
/// Shows potion count, click to heal, auto-disables when empty or full HP.
///
/// SETUP:
/// 1. Create UI Button on HUD Canvas
/// 2. Add this script to the button
/// 3. Assign potionItem (the Health Potion ItemObject from database)
/// 4. (Optional) Assign UI references for count text and cooldown
/// </summary>
public class PotionButton : MonoBehaviour
{
    [Header("Potion Item")]
    [Tooltip("Drag the Health Potion ItemObject here")]
    [SerializeField] private ItemObject potionItem;

    [Header("Cooldown")]
    [Tooltip("Seconds between each potion use")]
    [SerializeField] private float cooldown = 1.5f;

    [Header("UI References")]
    [Tooltip("Text showing how many potions left")]
    [SerializeField] private TextMeshProUGUI countText;

    [Tooltip("Cooldown overlay image (Fill type = Filled)")]
    [SerializeField] private Image cooldownOverlay;

    [Tooltip("Potion icon image")]
    [SerializeField] private Image potionIcon;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);

    private Button button;
    private InventoryManager inventory;
    private PlayerStats playerStats;

    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(UsePotion);
        }

        inventory = FindAnyObjectByType<InventoryManager>();
        playerStats = FindAnyObjectByType<PlayerStats>();

        // Listen for inventory changes to update count
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateUI;
        }

        // Listen for HP changes to enable/disable button
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += OnHealthChanged;
        }

        // Set icon from potionItem
        if (potionIcon != null && potionItem != null && potionItem.icon != null)
        {
            potionIcon.sprite = potionItem.icon;
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateUI;

        if (playerStats != null)
            playerStats.OnHealthChanged -= OnHealthChanged;
    }

    // ============================================
    // UPDATE COOLDOWN
    // ============================================

    private void Update()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            cooldownTimer = 0f;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;

            UpdateUI();
        }
        else
        {
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldown;
        }
    }

    // ============================================
    // USE POTION
    // ============================================

    private void UsePotion()
    {
        if (isOnCooldown) return;
        if (potionItem == null) return;
        if (inventory == null || playerStats == null) return;

        // Check if has potions
        int count = inventory.GetItemCount(potionItem);
        if (count <= 0)
        {
            Debug.Log("[PotionButton] No potions left!");
            inventory.ShowMessage("No potions!");
            return;
        }

        // Check if HP is already full
        if (playerStats.GetCurrentHP() >= playerStats.GetMaxHP())
        {
            Debug.Log("[PotionButton] HP already full!");
            inventory.ShowMessage("HP is full!");
            return;
        }

        // Use potion: heal + remove from inventory
        playerStats.Heal(potionItem.hpRestore);
        inventory.RemoveItem(potionItem, 1);

        Debug.Log($"[PotionButton] Used {potionItem.itemName}, restored {potionItem.hpRestore} HP");
        inventory.ShowMessage($"+{potionItem.hpRestore} HP");

        // Start cooldown
        isOnCooldown = true;
        cooldownTimer = cooldown;

        UpdateUI();
    }

    // ============================================
    // UPDATE UI
    // ============================================

    private void UpdateUI()
    {
        if (potionItem == null || inventory == null) return;

        int count = inventory.GetItemCount(potionItem);

        // Update count text
        if (countText != null)
        {
            countText.text = count.ToString();
        }

        // Update icon opacity
        if (potionIcon != null)
        {
            potionIcon.color = count > 0 ? normalColor : emptyColor;
        }

        // Update button interactable
        if (button != null)
        {
            bool canUse = count > 0 && !isOnCooldown;

            // Also check if HP not full
            if (playerStats != null)
            {
                canUse = canUse && playerStats.GetCurrentHP() < playerStats.GetMaxHP();
            }

            button.interactable = canUse;
        }
    }

    private void OnHealthChanged(float currentHP, float maxHP)
    {
        UpdateUI();
    }
}
