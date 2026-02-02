using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Phat : EnemyState_Phat
{
    private Enemy_BossP enemy;

    public IdleState_Phat(Enemy_Phat enemyBase, EnemyStateMachine_Phat stateMachine_Phat, string animBoolName) : base(enemyBase, stateMachine_Phat, animBoolName)
    {
        enemy = enemyBase as Enemy_BossP;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
            stateMachine_Phat.ChangeState(enemy.attackState);

        if (stateTimer < 0)
            stateMachine_Phat.ChangeState(enemy.moveState);
    }
}
