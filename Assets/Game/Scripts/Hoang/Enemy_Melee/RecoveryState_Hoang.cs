using UnityEngine;

public class RecoveryState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    public RecoveryState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);

        if(triggerCalled)
        {
            stateMachine_Hoang.ChangeState(enemy.chaseState);
        } 
    }
}
