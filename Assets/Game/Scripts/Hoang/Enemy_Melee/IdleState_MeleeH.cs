using UnityEngine;

public class IdleState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;

    public IdleState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();


        if (stateTimer < 0)
            stateMachine_Hoang.ChangeState_Hoang(enemy.moveState);
    }
}
