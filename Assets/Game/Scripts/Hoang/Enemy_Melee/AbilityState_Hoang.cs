using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Hoang : EnemyState_Hoang
{

    private Enemy_MeleeH enemy;
    private Vector3 movementDirection;

    private const float MAX_MOVEMENT_DISTANCE = 20;

    private float moveSpeed;


    public AbilityState_Hoang(Enemy_Hoang enemyBase, EnemyStateMachine_Hoang stateMachine_Hoang, string animBoolName) : base(enemyBase, stateMachine_Hoang, animBoolName)
    {
        enemy = enemyBase as Enemy_MeleeH;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.PullWeapon();

        moveSpeed = enemy.moveSpeed;
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.moveSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.moveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
            stateMachine_Hoang.ChangeState_Hoang(enemy.recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject newAxe = ObjectPoolH.instance.GetObject(enemy.axePrefab);

        // SET ĐÚNG VỊ TRÍ SPAWN TRƯỚC
        newAxe.transform.position = enemy.axeStartPoint.position;
        newAxe.transform.rotation = enemy.axeStartPoint.rotation;

        // RESET RIGIDBODY
        Rigidbody rb = newAxe.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // SETUP BAY
        newAxe.GetComponent<EnemyAxe_H>()
            .AxeSetup(enemy.axeFlySpeed, enemy.player, enemy.axeAimTimer);
    }
}
