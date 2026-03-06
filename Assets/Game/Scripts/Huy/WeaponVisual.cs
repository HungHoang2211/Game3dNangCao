using UnityEngine;

/// <summary>
/// Spawns weapon model on player's hand.
/// Shows default weapon when nothing equipped.
///
/// SETUP:
/// 1. Add to Player
/// 2. Assign rightHandBone (from skeleton: Armature > ... > Hand.R)
/// 3. Assign defaultWeaponPrefab (model vu khi mac dinh)
/// 4. Adjust offset until weapon fits hand
/// </summary>
public class WeaponVisual : MonoBehaviour
{
    [Header("Attach Point")]
    [Tooltip("Bone tay phai cua nhan vat")]
    [SerializeField] private Transform rightHandBone;

    [Header("Default Weapon")]
    [Tooltip("Vu khi mac dinh khi khong equip gi")]
    [SerializeField] private GameObject defaultWeaponPrefab;

    [Header("Offset")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private Vector3 scaleOverride = Vector3.one;

    [Header("References")]
    [SerializeField] private EquipmentManager equipmentManager;

    private GameObject currentWeaponInstance;
    private bool isDefaultWeapon = false;

    void Start()
    {
        if (equipmentManager == null)
            equipmentManager = GetComponent<EquipmentManager>();

        if (equipmentManager != null)
            equipmentManager.OnEquipmentChanged += HandleEquipmentChanged;

        // Check if already has weapon equipped
        ItemObject weapon = equipmentManager != null ? equipmentManager.GetWeapon() : null;

        if (weapon != null && weapon.equipPrefab != null)
        {
            SpawnWeapon(weapon.equipPrefab, false);
        }
        else
        {
            SpawnDefaultWeapon();
        }
    }

    void OnDestroy()
    {
        if (equipmentManager != null)
            equipmentManager.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged(EquipmentSlot slot, ItemObject item)
    {
        if (slot != EquipmentSlot.Weapon) return;

        if (item != null && item.equipPrefab != null)
        {
            // Equip weapon -> show its model
            SpawnWeapon(item.equipPrefab, false);
        }
        else
        {
            // Unequip -> show default weapon
            SpawnDefaultWeapon();
        }
    }

    private void SpawnDefaultWeapon()
    {
        if (defaultWeaponPrefab == null)
        {
            DestroyCurrentWeapon();
            return;
        }

        SpawnWeapon(defaultWeaponPrefab, true);
    }

    private void SpawnWeapon(GameObject prefab, bool isDefault)
    {
        DestroyCurrentWeapon();

        if (prefab == null || rightHandBone == null) return;

        currentWeaponInstance = Instantiate(prefab, rightHandBone);
        // Prefab giữ nguyên local transform đã chỉnh sẵn bên trong nó

        isDefaultWeapon = isDefault;

        Debug.Log($"[WeaponVisual] Spawned {(isDefault ? "default" : "equipped")} weapon");
    }

    private void DestroyCurrentWeapon()
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
    }
}
