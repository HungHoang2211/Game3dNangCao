using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    
    Transform player;
    NavMeshAgent agent;
    float attackRange = 2;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        agent.speed = 2;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector3.Distance(animator.transform.position, player.position);

        // Chỉ yêu cầu Agent di chuyển nếu còn ở xa hơn khoảng cách tấn công
        if (distance > attackRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Khi đã đến gần, dừng Agent lại để tránh va chạm gây giật
            agent.SetDestination(animator.transform.position);
            animator.SetBool("isAttacking", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     agent.speed= 1;
        agent.SetDestination(agent.transform.position);


    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
