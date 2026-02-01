using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    Transform player;
    float attackRange = 2;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").transform;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 1. Tính toán hướng nhìn (chỉ lấy hướng X và Z, bỏ qua Y để tránh Enemy bị nghiêng)
        Vector3 direction = player.position - animator.transform.position;
        direction.y = 0;

        // 2. Kiểm tra nếu hướng không bị rỗng (tránh lỗi khi đứng trùng vị trí)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Xoay mượt với tốc độ 5f (bạn có thể tăng số này nếu muốn xoay nhanh hơn)
            animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // 3. Kiểm tra khoảng cách để chuyển State
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > attackRange)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
        
   }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
