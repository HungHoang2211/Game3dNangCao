using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Persists player data across scene loads.
///
/// FIX: Level and EXP saved from ExperienceManager (not PlayerStats).
/// </summary>
public class GameDataPersist : MonoBehaviour
{
    public static GameDataPersist Instance { get; private set; }

    // ============================================
    // SAVED DATA
    // ============================================

    [System.Serializable]
    public class SlotData
    {
        public int itemId;
        public int quantity;
    }

    // Inventory
    private List<SlotData> savedInventory = new List<SlotData>();

    // Equipment
    private int savedWeaponId = -1;
    private int savedArmorId = -1;
    private int savedAccessoryId = -1;

    // Level and EXP (from ExperienceManager)
    private int savedLevel = 0;
    private int savedTotalExp = 0;

    // HP (from PlayerStats)
    private float savedHP = -1f;

    // Gold
    private int savedGold = 0;

    private bool hasData = false;

    // ============================================
    // SINGLETON
    // ============================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ============================================
    // PUBLIC
    // ============================================

    public void LoadSceneWithData(string sceneName)
    {
        SaveAllData();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithData(int sceneIndex)
    {
        SaveAllData();
        SceneManager.LoadScene(sceneIndex);
    }

    // ============================================
    // SAVE
    // ============================================

    private void SaveAllData()
    {
        SaveInventory();
        SaveEquipment();
        SaveLevelAndExp();
        SaveHP();
        SaveGold();

        hasData = true;
        Debug.Log("[GameDataPersist] === ALL DATA SAVED ===");
    }

    private void SaveInventory()
    {
        savedInventory.Clear();

        InventoryManager inventory = FindAnyObjectByType<InventoryManager>();
        if (inventory == null) return;

        InventorySlot[] slots = inventory.GetAllSlots();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                savedInventory.Add(new SlotData { itemId = -1, quantity = 0 });
            }
            else
            {
                savedInventory.Add(new SlotData
                {
                    itemId = slots[i].item.id,
                    quantity = slots[i].quantity
                });
            }
        }

        Debug.Log($"[GameDataPersist] Inventory saved: {savedInventory.Count} slots");
    }

    private void SaveEquipment()
    {
        EquipmentManager equip = FindAnyObjectByType<EquipmentManager>();
        if (equip == null) return;

        savedWeaponId = equip.GetWeapon() != null ? equip.GetWeapon().id : -1;
        savedArmorId = equip.GetArmor() != null ? equip.GetArmor().id : -1;
        savedAccessoryId = equip.GetAccessory() != null ? equip.GetAccessory().id : -1;

        Debug.Log($"[GameDataPersist] Equipment saved: W={savedWeaponId}, A={savedArmorId}, Acc={savedAccessoryId}");
    }

    private void SaveLevelAndExp()
    {
        // Get from ExperienceManager (source of truth for level/exp)
        ExperienceManager expMgr = FindAnyObjectByType<ExperienceManager>();

        if (expMgr != null)
        {
            savedLevel = expMgr.GetCurrentLevel();
            savedTotalExp = expMgr.GetTotalExperience();
            Debug.Log($"[GameDataPersist] Level/EXP saved: Lv{savedLevel}, TotalEXP={savedTotalExp}");
        }
        else
        {
            // Fallback to PlayerStats
            PlayerStats stats = FindAnyObjectByType<PlayerStats>();
            if (stats != null)
            {
                savedLevel = stats.GetCurrentLevel();
                savedTotalExp = stats.GetCurrentExp();
                Debug.Log($"[GameDataPersist] Level/EXP saved from PlayerStats: Lv{savedLevel}");
            }
        }
    }

    private void SaveHP()
    {
        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats != null)
        {
            savedHP = stats.GetCurrentHP();
            Debug.Log($"[GameDataPersist] HP saved: {savedHP:F0}");
        }
    }

    private void SaveGold()
    {
        if (CurrencyManager.Instance != null)
        {
            savedGold = CurrencyManager.Instance.GetGold();
            Debug.Log($"[GameDataPersist] Gold saved: {savedGold}");
        }
    }

    // ============================================
    // RESTORE
    // ============================================

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasData) return;

        Debug.Log($"[GameDataPersist] Scene '{scene.name}' loaded, restoring data...");
        Invoke(nameof(RestoreAllData), 0.2f);
    }

    private void RestoreAllData()
    {
        InventoryManager inventory = FindAnyObjectByType<InventoryManager>();

        if (inventory == null)
        {
            Debug.LogError("[GameDataPersist] InventoryManager not found!");
            return;
        }

        ItemDatabase database = inventory.GetItemDatabase();

        if (database == null)
        {
            Debug.LogError("[GameDataPersist] ItemDatabase not found!");
            return;
        }

        database.Initialize();

        RestoreInventory(inventory, database);
        RestoreEquipment(database);
        RestoreLevelAndExp();
        RestoreHP();
        RestoreGold();

        Debug.Log("[GameDataPersist] === ALL DATA RESTORED ===");
    }

    private void RestoreInventory(InventoryManager inventory, ItemDatabase database)
    {
        InventorySlot[] slots = inventory.GetAllSlots();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Clear();
        }

        int restoredCount = 0;

        for (int i = 0; i < savedInventory.Count && i < slots.Length; i++)
        {
            SlotData data = savedInventory[i];
            if (data.itemId < 0) continue;

            ItemObject item = database.GetItemByID(data.itemId);
            if (item != null)
            {
                slots[i].item = item;
                slots[i].quantity = data.quantity;
                restoredCount++;
            }
        }

        Debug.Log($"[GameDataPersist] Inventory restored: {restoredCount} items");
    }

    private void RestoreEquipment(ItemDatabase database)
    {
        EquipmentManager equip = FindAnyObjectByType<EquipmentManager>();
        if (equip == null) return;

        if (savedWeaponId >= 0)
        {
            ItemObject item = database.GetItemByID(savedWeaponId);
            if (item != null) equip.EquipItemDirectly(EquipmentSlot.Weapon, item);
        }

        if (savedArmorId >= 0)
        {
            ItemObject item = database.GetItemByID(savedArmorId);
            if (item != null) equip.EquipItemDirectly(EquipmentSlot.Armor, item);
        }

        if (savedAccessoryId >= 0)
        {
            ItemObject item = database.GetItemByID(savedAccessoryId);
            if (item != null) equip.EquipItemDirectly(EquipmentSlot.Accessory, item);
        }

        Debug.Log("[GameDataPersist] Equipment restored");
    }

    private void RestoreLevelAndExp()
    {
        // Restore to ExperienceManager first (source of truth)
        ExperienceManager expMgr = FindAnyObjectByType<ExperienceManager>();

        if (expMgr != null)
        {
            expMgr.RestoreState(savedLevel, savedTotalExp);
            Debug.Log($"[GameDataPersist] Level/EXP restored via ExperienceManager: Lv{savedLevel}, EXP={savedTotalExp}");
        }
        else
        {
            // Fallback: restore directly to PlayerStats
            PlayerStats stats = FindAnyObjectByType<PlayerStats>();
            if (stats != null)
            {
                stats.RestoreState(savedLevel, savedTotalExp, savedHP);
                Debug.Log($"[GameDataPersist] Level restored via PlayerStats: Lv{savedLevel}");
            }
        }
    }

    private void RestoreHP()
    {
        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats == null) return;

        if (savedHP > 0)
        {
            float diff = savedHP - stats.GetCurrentHP();
            if (diff > 0)
                stats.Heal(diff);
        }

        Debug.Log($"[GameDataPersist] HP restored: {savedHP:F0}");
    }

    private void RestoreGold()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.SetGold(savedGold);
            Debug.Log($"[GameDataPersist] Gold restored: {savedGold}");
        }
    }
}
