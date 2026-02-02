using UnityEngine;

public class ChaseState_Hoang : EnemyState_Hoang
{
    private Enemy_MeleeH enemy;
    private float lastTimeUpdateDestination;
    public ChaseState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();
        CheckChaseAnimation();

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

        if (enemy.PlayerInAttackRange() && enemy.CanAttack())
        {
            enemy.RecordAttackTime();
            stateMachine_Hoang.ChangeState(enemy.attackState);
        }

        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }
    private bool CanUpdateDestination()
    {
        if(Time.time > lastTimeUpdateDestination + .25f)
        {
            lastTimeUpdateDestination = Time.time;
            return true;
        }
        return false;
    }
    private void CheckChaseAnimation()
    {
        if(enemy.meleeType == EnemyMelee_Type.Shield && enemy.shieldTransform == null)
        {
            enemy.anim.SetFloat("ChaseIndex", 0);
        }
    }
}
