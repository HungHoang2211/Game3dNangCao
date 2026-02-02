using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy_Hoang : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 20;

    [Header("Attack data")]
    public float attackRange;
    public float attackMoveSpeed;
    public float attackCooldown = 1.2f;
    protected float lastAttackTime;

    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;
    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine_Hoang stateMachine_Hoang { get; private set; }
    protected virtual void Awake()
    {
        stateMachine_Hoang = new EnemyStateMachine_Hoang();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }
    protected virtual void Start()
    {
       InitializePatrolPoints();
    }
    protected virtual void Update()
    {

    }
    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void RecordAttackTime()
    {
        lastAttackTime = Time.time;
    }
    public virtual void GetHit()
    {
        healthPoints--;
    }
    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }
    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;
    public void AnimationTrigger() => stateMachine_Hoang.currentState.AnimationTrigger();
    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;
        if(currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }
        return destination;
    }
    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }
    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currenEulerAngels = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currenEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        return Quaternion.Euler(currenEulerAngels.x, yRotation, currenEulerAngels.z);
    }
}
