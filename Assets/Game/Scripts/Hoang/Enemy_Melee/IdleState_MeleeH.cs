using UnityEngine;

public class IdleState_MeleeH : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    public IdleState_MeleeH(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
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

        if (enemy.PlayerInAggresionRange())
        {
            stateMachine_Hoang.ChangeState(enemy.recoveryState);
            return;
        }

        if (stateTimer < 0)
            stateMachine_Hoang.ChangeState(enemy.moveState);
    }
}
