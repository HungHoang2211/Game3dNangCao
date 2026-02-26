using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Kéo thả Player vào đây hoặc để code tự tìm
    public Animator animator;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f; // Thời gian diễn ra animation đánh

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // Tự động tìm Player nếu chưa gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Xử lý thời gian chờ sau khi đánh
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
            return; // Khi đang đánh thì không thực hiện các logic di chuyển bên dưới
        }

        // Logic chuyển đổi trạng thái
        if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }

        // Thực thi trạng thái
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        // Đồng bộ animation chạy/đi bộ
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
            return;
        }

        if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, 1))
        {
            patrolPoint = hit.position;
            agent.SetDestination(patrolPoint);
            isPatrolling = true;
            isIdle = false;
        }
    }

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;
        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);
    }

    void Attack()
    {
        isAttacking = true;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        agent.ResetPath(); // Dừng di chuyển khi đánh

        // Quay mặt về phía Player
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);

        // Kích hoạt animation đánh
        animator.ResetTrigger("AttackVu"); // Đảm bảo trigger được reset trước khi kích hoạt
        animator.SetTrigger("AttackVu");
    }

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}