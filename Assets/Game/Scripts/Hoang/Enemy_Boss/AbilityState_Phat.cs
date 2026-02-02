using UnityEngine;

public class AbilityState_Phat : EnemyState_Phat
{
    private Enemy_BossP enemy;

    public AbilityState_Phat(Enemy_Phat enemyBase, EnemyStateMachine_Phat stateMachine_Phat, string animBoolName) : base(enemyBase, stateMachine_Phat, animBoolName)
    {
        enemy = enemyBase as Enemy_BossP;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.flamethrowDuration;

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.bossVisuals.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (ShouldDisableFlamethrower())
            DisableFlamethrower();

        if (triggerCalled)
            stateMachine_Phat.ChangeState(enemy.moveState);
    }

    private bool ShouldDisableFlamethrower() => stateTimer < 0;

    public void DisableFlamethrower()
    {
        if (enemy.bossWeaponType != BossWeaponType.Flamethrower)
            return;

        if (enemy.flamethrowActive == false)
            return;

        enemy.ActivateFlamethrower(false);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisuals.DischargeBatteries();
            enemy.bossVisuals.EnableWeaponTrail(false);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetAbilityOnCooldown();
        enemy.bossVisuals.ResetBatteries();
    }
}
