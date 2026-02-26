using UnityEngine;

public class ChaseState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    private float lastTimeUpdatedDistanation;

    public ChaseState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();


        enemy.agent.speed = enemy.chaseSpeed;
        enemy.agent.isStopped = false;

    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
            stateMachine_Hoang.ChangeState_Hoang(enemy.attackState);

        enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }


    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDistanation + .25f)
        {
            lastTimeUpdatedDistanation = Time.time;
            return true;
        }

        return false;
    }
}
