using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 15f; // Tăng tốc độ xoay để bắt kịp Player nhanh hơn
    public float attackDuration = 1.0f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private bool isAttacking;
    private bool isIdle;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        if (animator == null) animator = GetComponentInChildren<Animator>();

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

        // Giảm cooldown liên tục
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // XỬ LÝ KHI ĐANG TRONG TRẠNG THÁI TẤN CÔNG
        if (isAttacking)
        {
            agent.velocity = Vector3.zero;
            attackTimer -= Time.deltaTime;

            // Xoay nhanh về phía player khi đang đánh để đòn đánh không bị lệch
            SmoothLookAt(player.position);

            if (attackTimer <= 0f)
            {
                isAttacking = false;
                agent.isStopped = false;
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // KIỂM TRA ĐIỀU KIỆN ĐÁNH NGAY LẬP TỨC
        if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
        {
            Attack(); // Gọi trực tiếp để giảm delay do switch case
            return;
        }

        // CHUYỂN TRẠNG THÁI DI CHUYỂN
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }

        ExecuteMovementState();

        // ĐỒNG BỘ ANIMATION
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            SmoothLookAt(transform.position + agent.velocity);
        }
    }

    void ExecuteMovementState()
    {
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
        }
    }

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                isIdle = false;
                SetNewPatrolPoint();
            }
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            isIdle = true;
            idleTimer = 0f;
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
            agent.isStopped = false;
        }
    }

    void ChasePlayer()
    {
        isIdle = false;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        // Reset Trigger trước khi Set để đảm bảo không bị dồn lệnh (nếu có lỗi logic khác)
        animator.ResetTrigger("AttackVu");
        animator.SetTrigger("AttackVu");
    }

    void SmoothLookAt(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}