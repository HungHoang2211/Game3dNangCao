using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState_Phat 
{
    protected Enemy_Phat enemyBase;
    protected EnemyStateMachine_Phat stateMachine_Phat;

    protected string animBoolName;
    protected float stateTimer;

    protected bool triggerCalled;

    public EnemyState_Phat(Enemy_Phat enemyBase, EnemyStateMachine_Phat stateMachine_Phat, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine_Phat = stateMachine_Phat;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        enemyBase.anim.SetBool(animBoolName, true);

        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {

        enemyBase.anim.SetBool(animBoolName, false);
    }

    public void AnimationTrigger() => triggerCalled = true;

    public virtual void AbilityTrigger()
    {

    }

    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
            return agent.destination;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
                return path.corners[i + 1];
        }

        return agent.destination;
    }
}
