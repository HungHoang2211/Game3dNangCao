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
    public float rotationSpeed = 15f;
    public float attackDuration = 1.0f;

    [Header("Path Update")]
    [Tooltip("How often to recalculate path during chase (seconds)")]
    public float chaseUpdateInterval = 0.3f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;
    private float chaseUpdateTimer;

    private bool isAttacking;
    private bool isIdle;

    public int Damage = 10;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        currentState = State.Patrol;
        SetNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // Handle attack state separately
        if (isAttacking)
        {
            HandleAttacking();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check attack first
        if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
        {
            Attack();
            return;
        }

        // State transitions
        if (distanceToPlayer <= detectionRadius)
        {
            if (currentState != State.Chase)
            {
                currentState = State.Chase;
                isIdle = false;
                chaseUpdateTimer = 0f; // Force immediate path update
            }
            ChasePlayer();
        }
        else
        {
            if (currentState != State.Patrol)
            {
                currentState = State.Patrol;
                SetNewPatrolPoint();
            }
            Patrol();
        }

        // Animation
        bool isMoving = agent.hasPath && agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);

        // Rotate toward movement direction
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            SmoothLookAt(transform.position + agent.velocity);
        }
    }

    // ============================================
    // ATTACK
    // ============================================

    void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        cooldownTimer = attackCooldown;

        // Stop movement completely
        agent.isStopped = true;
        agent.ResetPath();

        // Deal damage
        Player playerHealth = player.GetComponent<Player>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(Damage);
        }

        // Play animation
        animator.ResetTrigger("AttackVu");
        animator.SetTrigger("AttackVu");
    }

    void HandleAttacking()
    {
        // Face player during attack
        SmoothLookAt(player.position);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            isAttacking = false;
            agent.isStopped = false;
        }
    }

    // ============================================
    // PATROL (fixed jitter)
    // ============================================

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

        // Check if arrived (with safe threshold)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.3f)
        {
            // Stop agent to prevent micro-jitter
            agent.isStopped = true;
            agent.ResetPath();

            isIdle = true;
            idleTimer = 0f;
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    // ============================================
    // CHASE (fixed: don't recalc every frame)
    // ============================================

    void ChasePlayer()
    {
        agent.isStopped = false;

        // Only update path every X seconds (not every frame)
        chaseUpdateTimer -= Time.deltaTime;

        if (chaseUpdateTimer <= 0f)
        {
            agent.SetDestination(player.position);
            chaseUpdateTimer = chaseUpdateInterval;
        }
    }

    // ============================================
    // ROTATION
    // ============================================

    void SmoothLookAt(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
