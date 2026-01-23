using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Stats")]
    public int damage = 10;
    public float attackRange = 3f;
    public float attackSpeed = 1.5f; 

    [Header("Range Visual")]
    public GameObject attackRangeCircle;

    private float nextAttackTime;
    
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
        if (attackRangeCircle != null)
        {
            attackRangeCircle.SetActive(false);

            float diameter = attackRange * 2f;
            attackRangeCircle.transform.localScale = new Vector3(diameter, 0.05f, diameter);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnAttackHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnAttackRelease();
        }
    }

    public void OnAttackHold()
    {
        ShowRange(true);

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + 1f / attackSpeed;

        DoAttack();
    }

    public void OnAttackRelease()
    {
        ShowRange(false);
    }

    void DoAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        //Animation chém ko biết nhét đúng ko
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    void ShowRange(bool show)
    {
        if (attackRangeCircle != null) 
        { 
            attackRangeCircle.SetActive(show); 
        }
           
    }
}
