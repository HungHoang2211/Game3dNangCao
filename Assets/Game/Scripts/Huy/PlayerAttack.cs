using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponStatus currentWeapon;
    public WeaponStatus defaultWeapon;
    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    [Header("Weapon Holder")]
    public Transform weaponHolder;

    public GameObject currentWeaponObject;

    [Header("Player stat")]
    public PlayerStat playerDamage;

    private float nextAttackTime;

    Animator animator;

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

        //foreach (Collider hit in hits)
        //{
        //    EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
        //    if (enemy != null)
        //    {
        //        enemy.TakeDamage(currentWeapon.damage + (playerDamage.damage/2));
        //    }
        //}
        //Debug.Log("Slash");
    }
    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }

    }

    public void EquipWeapon(WeaponStatus newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;

        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        if (newWeapon.weaponPrefab != null)
        {
            currentWeaponObject = Instantiate(
                newWeapon.weaponPrefab,
                weaponHolder
            );

            currentWeaponObject.transform.localPosition = Vector3.zero;
            currentWeaponObject.transform.localRotation = Quaternion.identity;
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

    /*/kien attack 
    void Attack()
    { RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit,attackRange))

        
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyKien enemy = hit.collider.GetComponent<EnemyKien>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
    /*/
}
