using UnityEngine;

/// <summary>
/// Debug tool to test equipment bonuses
/// Press keys to add/remove equipment
/// DELETE THIS SCRIPT when Inventory system is done
/// </summary>
public class EquipmentTester : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;
    
    [Header("Test Equipment Stats")]
    [SerializeField] private float testWeaponDamage = 20f;
    [SerializeField] private float testArmorDefense = 10f;
    [SerializeField] private float testAccessorySpeed = 2f;
    [SerializeField] private float testAccessoryCritRate = 10f;
    
    private bool weaponEquipped = false;
    private bool armorEquipped = false;
    private bool accessoryEquipped = false;
    
    void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
        
        if (playerStats == null)
        {
            Debug.LogError("[EquipmentTester] PlayerStats not found!");
        }
    }
    
    void Start()
    {
        Debug.Log("=== EQUIPMENT TESTER READY ===");
        Debug.Log("Press [1] to equip/unequip WEAPON (+20 DMG)");
        Debug.Log("Press [2] to equip/unequip ARMOR (+10 DEF)");
        Debug.Log("Press [3] to equip/unequip ACCESSORY (+2 SPD, +10% CRIT)");
        Debug.Log("Press [L] to LEVEL UP");
        Debug.Log("Press [H] to HEAL");
        Debug.Log("===============================");
        
        PrintStats();
    }
    
    void Update()
    {
        // Equip/Unequip Weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleWeapon();
        }
        
        // Equip/Unequip Armor
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleArmor();
        }
        
        // Equip/Unequip Accessory
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleAccessory();
        }
        
        // Level Up
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
        
        // Heal
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal();
        }
        
        // Print Stats
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintStats();
        }
    }
    
    void ToggleWeapon()
    {
        if (weaponEquipped)
        {
            // Unequip
            playerStats.RemoveEquipmentBonus(damage: testWeaponDamage);
            weaponEquipped = false;
            Debug.Log($"🗡️ WEAPON UNEQUIPPED (-{testWeaponDamage} DMG)");
        }
        else
        {
            // Equip
            playerStats.AddEquipmentBonus(damage: testWeaponDamage);
            weaponEquipped = true;
            Debug.Log($"🗡️ WEAPON EQUIPPED (+{testWeaponDamage} DMG)");
        }
        
        PrintStats();
    }
    
    void ToggleArmor()
    {
        if (armorEquipped)
        {
            // Unequip
            playerStats.RemoveEquipmentBonus(defense: testArmorDefense);
            armorEquipped = false;
            Debug.Log($"🛡️ ARMOR UNEQUIPPED (-{testArmorDefense} DEF)");
        }
        else
        {
            // Equip
            playerStats.AddEquipmentBonus(defense: testArmorDefense);
            armorEquipped = true;
            Debug.Log($"🛡️ ARMOR EQUIPPED (+{testArmorDefense} DEF)");
        }
        
        PrintStats();
    }
    
    void ToggleAccessory()
    {
        if (accessoryEquipped)
        {
            // Unequip
            playerStats.RemoveEquipmentBonus(
                speed: testAccessorySpeed, 
                critRate: testAccessoryCritRate
            );
            accessoryEquipped = false;
            Debug.Log($"💍 ACCESSORY UNEQUIPPED (-{testAccessorySpeed} SPD, -{testAccessoryCritRate}% CRIT)");
        }
        else
        {
            // Equip
            playerStats.AddEquipmentBonus(
                speed: testAccessorySpeed, 
                critRate: testAccessoryCritRate
            );
            accessoryEquipped = true;
            Debug.Log($"💍 ACCESSORY EQUIPPED (+{testAccessorySpeed} SPD, +{testAccessoryCritRate}% CRIT)");
        }
        
        PrintStats();
    }
    
    void LevelUp()
    {
        // Add enough EXP to level up
        int expNeeded = playerStats.GetExpToNextLevel();
        playerStats.AddExp(expNeeded);
        
        Debug.Log($"⬆️ LEVEL UP!");
        PrintStats();
    }
    
    void Heal()
    {
        float maxHP = playerStats.GetMaxHP();
        playerStats.Heal(maxHP);
        
        Debug.Log($"❤️ HEALED TO FULL!");
        PrintStats();
    }
    
    void PrintStats()
    {
        Debug.Log("=== CURRENT STATS ===");
        Debug.Log($"Level: {playerStats.GetCurrentLevel()}");
        Debug.Log($"EXP: {playerStats.GetCurrentExp()}/{playerStats.GetExpToNextLevel()}");
        Debug.Log($"HP: {playerStats.GetCurrentHP():F1}/{playerStats.GetMaxHP():F1}");
        Debug.Log($"Damage: {playerStats.GetTotalDamage():F1}");
        Debug.Log($"Defense: {playerStats.GetTotalDefense():F1}");
        Debug.Log($"Speed: {playerStats.GetTotalSpeed():F1}");
        Debug.Log($"Crit Rate: {playerStats.GetTotalCritRate():F1}%");
        Debug.Log($"Crit Damage: {playerStats.GetTotalCritDamage():F1}%");
        Debug.Log("=====================");
    }
}