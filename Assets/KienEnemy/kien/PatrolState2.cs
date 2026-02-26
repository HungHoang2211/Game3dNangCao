using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState2 : StateMachineBehaviour
{
    float timer;
    NavMeshAgent agent;
    List<Transform> wayPoints = new List<Transform>();
    Transform player;
    float chaseRange = 10f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        agent = animator.GetComponent<NavMeshAgent>();
        Transform wayPointsContainer = GameObject.FindGameObjectWithTag("WayPoints2").transform;
        foreach (Transform t in wayPointsContainer)
        {
            wayPoints.Add(t);
        }
        agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            animator.SetBool("isIdle", true);
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
        }
        float distance = Vector3.Distance(player.position, agent.transform.position);
        if (distance < chaseRange)
        {
            animator.SetBool("isChasing", true);
        }

    }




    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
