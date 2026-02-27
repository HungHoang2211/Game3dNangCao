using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon")]

    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    [Header("Weapon Holder")]
    public Transform weaponHolder;

    public GameObject currentWeaponObject;

    [Header("Player stat")]
    

    private float nextAttackTime;

    Animator animator;



    public Transform spawnPoint;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        transform.position = spawnPoint.position;

       

        //UpdateRangeVisual();

       
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
       
        ShowRange(true);

        if (Time.time < nextAttackTime)
            return;

        //nextAttackTime = Time.time + 1f / currentWeapon.attackSpeed;

        DoAttack();
    }

    public void OnAttackRelease()
    {
        ShowRange(false);
    }

    void DoAttack()
    {
       
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
        //Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.attackRange);

        //foreach (Collider hit in hits)
        //{
        //    EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
        //    if (enemy != null)
        //    {
        //        enemy.TakeDamage(currentWeapon.damage + (playerDamage.damage / 2));
        //    }
        //}
        Debug.Log("Slash");
    }
    void ShowRange(bool show)
    {
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(show);
        }

    }

   


    //void UpdateRangeVisual()
    //{
    //    if (attackRangeCircle != null && currentWeapon != null)
    //    {
    //        float diameter = currentWeapon.attackRange * 2f;
    //        attackRangeCircle.transform.localScale =
    //            new Vector3(diameter, 0.05f, diameter);
    //    }
    //}

   
}
