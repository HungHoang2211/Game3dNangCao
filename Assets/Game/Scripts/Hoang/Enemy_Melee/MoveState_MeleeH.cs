using UnityEngine;
using UnityEngine.AI;

public class MoveState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    private Vector3 destination;


    public MoveState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.moveSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());


        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine_Hoang.ChangeState_Hoang(enemy.idleState);
    }


}

