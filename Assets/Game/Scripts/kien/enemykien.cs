using UnityEngine;
using UnityEngine.AI;

public class EnemyKien : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Range")]
    public float detectRange = 10f;
    public float attackRange = 1.8f;
    public float homeRange = 15f;

    private Vector3 homePos;

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        homePos = transform.position;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromHome = Vector3.Distance(transform.position, homePos);

        // ===== QUAY VỀ NHÀ =====
        if (distanceFromHome > homeRange)
        {
            ReturnHome();
            return;
        }

        // ===== PHÁT HIỆN PLAYER =====
        if (distanceToPlayer <= detectRange)
        {
            // TẤN CÔNG
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        anim.SetBool("isRun", false);
        anim.SetBool("isAttack", false);
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        anim.SetBool("isRun", true);
        anim.SetBool("isAttack", false);
    }

    void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        anim.SetBool("isRun", false);
        anim.SetBool("isAttack", true);
    }

    void ReturnHome()
    {
        agent.isStopped = false;
        agent.SetDestination(homePos);

        anim.SetBool("isRun", true);
        anim.SetBool("isAttack", false);

        if (Vector3.Distance(transform.position, homePos) < 0.5f)
        {
            Idle();
        }
    }
}
