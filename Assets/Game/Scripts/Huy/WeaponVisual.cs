using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [Header("Attach Points")]
    [Tooltip("Bone tay phải của nhân vật (kéo từ Armature > Hand.R)")]
    [SerializeField] private Transform rightHandBone;

    [Header("Offset (tuỳ chỉnh cho khớp tay)")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private Vector3 scaleOverride = Vector3.one;

    [Header("References")]
    [SerializeField] private EquipmentManager equipmentManager;

    private GameObject currentWeaponInstance;

    void Start()
    {
        if (equipmentManager == null)
            equipmentManager = GetComponent<EquipmentManager>();

        // Lắng nghe khi equipment thay đổi
        equipmentManager.OnEquipmentChanged += HandleEquipmentChanged;

        // Nếu đã có weapon equipped sẵn (load save), spawn luôn
        ItemObject weapon = equipmentManager.GetWeapon();
        if (weapon != null)
            SpawnWeapon(weapon);
    }

    void OnDestroy()
    {
        if (equipmentManager != null)
            equipmentManager.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged(EquipmentSlot slot, ItemObject item)
    {
        // Chỉ xử lý slot Weapon
        if (slot != EquipmentSlot.Weapon) return;

        if (item != null)
            SpawnWeapon(item);
        else
            DestroyCurrentWeapon();
    }

    private void SpawnWeapon(ItemObject item)
    {
        // Xoá vũ khí cũ trước
        DestroyCurrentWeapon();

        if (item.equipPrefab == null)
        {
            Debug.LogWarning($"[WeaponVisual] {item.itemName} không có equipPrefab!");
            return;
        }

        if (rightHandBone == null)
        {
            Debug.LogError("[WeaponVisual] Chưa gán rightHandBone!");
            return;
        }

        // Instantiate vũ khí làm con của bone tay
        currentWeaponInstance = Instantiate(item.equipPrefab, rightHandBone);

        // Áp dụng offset
        currentWeaponInstance.transform.localPosition = positionOffset;
        currentWeaponInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
        currentWeaponInstance.transform.localScale = scaleOverride;

        Debug.Log($"[WeaponVisual] Spawned {item.itemName} on hand");
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