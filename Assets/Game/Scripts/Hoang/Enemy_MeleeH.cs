using UnityEngine;

public enum EnemyMelee_Type {Shield}

public class Enemy_MeleeH : Enemy_Hoang
{
    public IdleState_MeleeH idleState { get; private set; }
    public MoveState_MeleeH moveState { get; private set; }
    public RecoveryState_Hoang recoveryState { get; private set; }
    public ChaseState_Hoang chaseState { get; private set; }
    public AttackState_Hoang attackState { get; private set; }
    public DeadState_Hoang deadState { get; private set; }
    public EnemyMelee_Type meleeType;

    public Transform shieldTransform;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;
    protected override void Awake()
    {
        base.Awake();       

        idleState = new IdleState_MeleeH(this, stateMachine_Hoang, "Idle");
        moveState = new MoveState_MeleeH(this, stateMachine_Hoang, "Move");
        recoveryState = new RecoveryState_Hoang(this, stateMachine_Hoang, "Recovery");
        chaseState = new ChaseState_Hoang(this, stateMachine_Hoang, "Chase");
        attackState = new AttackState_Hoang(this, stateMachine_Hoang, "Attack");
        deadState = new DeadState_Hoang(this, stateMachine_Hoang, "Idle");
    }
    protected override void Start()
    {
        base.Start();
        stateMachine_Hoang.Initialize(idleState);
        InitializeSpciality();
    }
    protected override void Update()
    {
        base.Update();
        stateMachine_Hoang.currentState.Update();
    }
    private void InitializeSpciality()
    {
        if(meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }    
    }
    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
            stateMachine_Hoang.ChangeState(deadState);
    }
    public void PullWeapon()
    { 
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }
}
