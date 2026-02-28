using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}
public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class Enemy_MeleeH : Enemy_Hoang
{
    #region States
    public IdleState_Hoang idleState { get; private set; }
    public MoveState_Hoang moveState { get; private set; }
    public RecoveryState_Hoang recoveryState { get; private set; }
    public ChaseState_Hoang chaseState { get; private set; }
    public AttackState_Hoang attackState { get; private set; }
    public DeadState_Hoang deadState { get; private set; }
    public AbilityState_Hoang abilityState { get; private set; }

    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrown;
    public Transform axeStartPoint;

    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Hoang(this, stateMachine_Hoang, "Idle");
        moveState = new MoveState_Hoang(this, stateMachine_Hoang, "Move");
        recoveryState = new RecoveryState_Hoang(this, stateMachine_Hoang, "Recovery");
        chaseState = new ChaseState_Hoang(this, stateMachine_Hoang, "Chase");
        attackState = new AttackState_Hoang(this, stateMachine_Hoang, "Attack");
        deadState = new DeadState_Hoang(this, stateMachine_Hoang, "Idle"); // Idle anim is just a place holder,we use ragdoll
        abilityState = new AbilityState_Hoang(this, stateMachine_Hoang, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine_Hoang.Initialize(idleState);

        InitializeSpeciality();

    }

    protected override void Update()
    {
        base.Update();
        stateMachine_Hoang.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode(); 
            
    }

    public override void EnterBattleMode() { if (inBattleMode) return; base.EnterBattleMode(); stateMachine_Hoang.ChangeState_Hoang(recoveryState); }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed = moveSpeed * .6f;
        pulledWeapon.gameObject.SetActive(false);
    }

    private void InitializeSpeciality()
    {
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints.currentHp <= 0)
            stateMachine_Hoang.ChangeState_Hoang(deadState);
    }

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }


    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange; 


    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        if (stateMachine_Hoang.currentState != chaseState)
            return;

        if (Vector3.Distance(transform.position, player.position) < 2f)
            return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge roll");

        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }

        return false;
    }


    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.Log(clipName + "animation not found!");
        return 0;
    }
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);

    }
    

}
