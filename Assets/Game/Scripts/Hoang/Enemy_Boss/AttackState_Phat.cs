using UnityEngine;

public class AttackState_Phat : EnemyState_Phat
{
    private Enemy_BossP enemy;
    public float lastTimeAttacked { get; private set; }
    public AttackState_Phat(Enemy_Phat enemyBase, EnemyStateMachine_Phat stateMachine_Phat, string animBoolName) : base(enemyBase, stateMachine_Phat, animBoolName)
    {
        enemy = enemyBase as Enemy_BossP;
    }

    public override void Enter()
    {
        base.Enter();

        //enemy.anim.SetFloat("AttackAnimIndex", Random.Range(0, 2));
        enemy.agent.isStopped = true;

        stateTimer = 1f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            enemy.FaceTarget(enemy.player.position, 20);

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine_Phat.ChangeState(enemy.idleState);
            else
                stateMachine_Phat.ChangeState(enemy.moveState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAttacked = Time.time;
        //enemy.bossVisuals.EnableWeaponTrail(false);
    }
}
