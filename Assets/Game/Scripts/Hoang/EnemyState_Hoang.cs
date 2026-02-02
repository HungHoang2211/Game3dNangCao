using UnityEngine;
using UnityEngine.AI;

public class EnemyState_Hoang
{
    protected Enemy_Hoang enemyBase;
    protected EnemyStateMachine_Hoang stateMachine_Hoang;

    protected string animBoolName;
    protected float stateTimer;

    protected bool triggerCalled;
    public EnemyState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine_Hoang = stateMachine_Hoang;
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

    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
            return agent.destination;
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
            {
                return path.corners[i + 1];
            }
        }
        return agent.destination;
    }
}
