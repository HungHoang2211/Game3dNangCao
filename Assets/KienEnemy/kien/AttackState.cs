using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    Transform player;
    float attackRange = 2f;

    float attackRate = 1.5f;
    float nextAttackTime;
    public int damage = 10;

    GameObject effectObj;     // effect object
    Transform effectPoint;    // vị trí spawn effect

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").transform;
        nextAttackTime = Time.time;

        // Tìm effect point trong enemy
        effectPoint = animator.transform.Find("EffectPoint"); 
        if (effectPoint != null)
        {
            effectObj = effectPoint.gameObject;
            effectObj.SetActive(true); // bật effect khi bắt đầu attack
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 direction = player.position - animator.transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            animator.transform.rotation = Quaternion.Slerp(
                animator.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Player pHealth = player.GetComponent<Player>();
                if (pHealth != null)
                {
                    pHealth.TakeDamage(damage);
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (effectObj != null)
        {
            effectObj.SetActive(false); // tắt effect khi thoát attack
        }
    }
}