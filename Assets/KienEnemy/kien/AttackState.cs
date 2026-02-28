using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    Transform player;
    float attackRange = 2f;


    float attackRate = 1.5f;
    float nextAttackTime;
    public int damage = 10;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").transform;
        nextAttackTime = Time.time;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Vector3 direction = player.position - animator.transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, Time.deltaTime * 5f);
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
}