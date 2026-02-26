using UnityEngine;

public class DeadState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    private EnemyRagdoll_Hoang ragdoll;

    private bool interactionDisabled;

    public DeadState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
        ragdoll = enemy.GetComponent<EnemyRagdoll_Hoang>();
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
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
            ragdoll.RagdollActive(false);
            ragdoll.CollidersActive(false);
        }
    }
}