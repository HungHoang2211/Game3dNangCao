using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Phat : EnemyState_Phat
{
    private Enemy_BossP enemy;
    private bool interactionDisabled;

    public DeadState_Phat(Enemy_Phat enemyBase, EnemyStateMachine_Phat stateMachine_Phat, string animBoolName) : base(enemyBase, stateMachine_Phat, animBoolName)
    {
        enemy = enemyBase as Enemy_BossP;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.abilityState.DisableFlamethrower();

        interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        //enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();
        // uncommnet if you want to disale interaction 
        //DisableInteractionIfShould();
    }

    private void DisableInteractionIfShould()
    {
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            //enemy.ragdoll.RagdollActive(false);
            //enemy.ragdoll.CollidersActive(false);
        }
    }
}
