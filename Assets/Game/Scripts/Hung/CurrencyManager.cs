using UnityEngine;
using System;

/// <summary>
/// Manages player currency (Gold)
/// Singleton pattern for easy access
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    [Header("Starting Currency")]
    [SerializeField] private int startingGold = 100;

    [Header("Current Currency (Runtime)")]
    [SerializeField] private int currentGold;

    // Events
    public event Action<int> OnGoldChanged; // (newAmount)

    // Singleton
    public static CurrencyManager Instance { get; private set; }

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentGold = startingGold;
        Debug.Log($"[CurrencyManager] Initialized with {currentGold} gold");
    }

    // ============================================
    // CURRENCY OPERATIONS
    // ============================================

    /// <summary>
    /// Add gold to player
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[CurrencyManager] Cannot add negative or zero gold!");
            return;
        }

        currentGold += amount;
        Debug.Log($"[CurrencyManager] +{amount} gold. Total: {currentGold}");

        OnGoldChanged?.Invoke(currentGold);
    }

    /// <summary>
    /// Remove gold from player
    /// Returns true if successful
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[CurrencyManager] Cannot spend negative or zero gold!");
            return false;
        }

        if (!HasEnoughGold(amount))
        {
            Debug.LogWarning($"[CurrencyManager] Not enough gold! Need {amount}, have {currentGold}");
            return false;
        }

        currentGold -= amount;
        Debug.Log($"[CurrencyManager] -{amount} gold. Remaining: {currentGold}");

        OnGoldChanged?.Invoke(currentGold);

        return true;
    }

    /// <summary>
    /// Check if player has enough gold
    /// </summary>
    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }

    /// <summary>
    /// Get current gold amount
    /// </summary>
    public int GetGold()
    {
        return currentGold;
    }

    /// <summary>
    /// Set gold directly (for save/load or cheats)
    /// </summary>
    public void SetGold(int amount)
    {
        currentGold = Mathf.Max(0, amount);
        Debug.Log($"[CurrencyManager] Gold set to {currentGold}");

        OnGoldChanged?.Invoke(currentGold);
    }

    // ============================================
    // DEBUG
    // ============================================

    [ContextMenu("Add 100 Gold")]
    private void Debug_Add100Gold()
    {
        AddGold(100);
    }

    [ContextMenu("Add 1000 Gold")]
    private void Debug_Add1000Gold()
    {
        AddGold(1000);
    }
}