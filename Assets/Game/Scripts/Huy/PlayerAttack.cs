using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon")]
    public ItemStatus currentWeapon;
    public ItemStatus defaultWeapon;
    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    [Header("Weapon Holder")]
    public Transform weaponHolder;

    public GameObject currentWeaponObject;

    [Header("Player stat")]
    public PlayerStat playerDamage;

    private float nextAttackTime;

    Animator animator;

    public EquipmentUI equipmentUI;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (currentWeapon == null)
        {
            EquipWeapon(defaultWeapon);
        }         
        else
            EquipWeapon(currentWeapon);

        UpdateRangeVisual();

       
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnAttackHold();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnAttackRelease();
        }
    }


    public void OnAttackHold()
    {
        if (currentWeapon == null) return;
        ShowRange(true);

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + 1f / currentWeapon.attackSpeed;

        DoAttack();
    }

    public void OnAttackRelease()
    {
        ShowRange(false);
    }

    void DoAttack()
    {
        if (currentWeapon == null) return;
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
        Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.attackRange);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(currentWeapon.damage + (playerDamage.damage / 2));
            }
        }
        Debug.Log("Slash");
    }
    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }

    }

    public void EquipWeapon(ItemStatus newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogWarning("EquipWeapon: newWeapon null!");
            return;
        }

        Debug.Log("EquipWeapon: Trang bị " + newWeapon.itemName);

        currentWeapon = newWeapon;

        if (currentWeaponObject != null)
        {
            Debug.Log("EquipWeapon: Hủy vũ khí cũ " + currentWeaponObject.name);
            Destroy(currentWeaponObject);
        }

        if (newWeapon.itemPrefab != null)
        {
            currentWeaponObject = Instantiate(newWeapon.itemPrefab, weaponHolder);
            currentWeaponObject.transform.localPosition = Vector3.zero;
            currentWeaponObject.transform.localRotation = Quaternion.identity;
            Debug.Log("EquipWeapon: Đã instantiate prefab " + newWeapon.itemPrefab.name);
        }

        if (equipmentUI != null)
        {
            Debug.Log("EquipWeapon: Cập nhật UI với " + newWeapon.itemName);
            equipmentUI.UpdateWeapon(newWeapon);
        }
        else
        {
            Debug.LogWarning("EquipWeapon: equipmentUI chưa được gán trong Inspector!");
        }
    }


    void UpdateRangeVisual()
    {
        if (attackRangeCircle != null && currentWeapon != null)
        {
            float diameter = currentWeapon.attackRange * 2f;
            attackRangeCircle.transform.localScale =
                new Vector3(diameter, 0.05f, diameter);
        }
    }

   
}
