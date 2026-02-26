using UnityEngine;
using UnityEngine.AI;
public class AIControllerVu : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float StartWaitTime = 4f;
    public float timeRotation = 2f;
    public float speedWalk = 6f;
    public float speedRun = 9f;

    public float viewRadius = 15f;
    public float viewAngle = 90f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeResolveIterations = 4;
    public float edgeDistance = 0.5f;

    public Transform[] waypoints;
    int m_currentWaypointIndex;

    Vector3 playersLastPosition = Vector3.zero;
    Vector3 m_playerPosition;

    float m_waitTime;
    float m_timeRotation;
    bool m_playerInSight;
    bool m_playerInrange;
    bool m_playerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;



    void Start()
    {
        m_playerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_playerInrange = false;
        m_waitTime = StartWaitTime;
        m_timeRotation = timeRotation;

        m_currentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
    }

    
    void Update()
    {
        EnviromentView();
        if(!m_CaughtPlayer)
        {
            if (!m_IsPatrol)
            {
                Chasing();
            }
            else
            {
                Patroling();
            }
        }
    }
    private void Chasing()
    {
        m_playerNear = false;
        playersLastPosition = Vector3.zero;
        if(!m_CaughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_playerPosition);
            
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if(m_waitTime <= 0  && !m_CaughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Enemy").transform.position) >= 6f)
            {
                
                m_IsPatrol = true;
                m_playerNear = false;
                Move(speedWalk);
                m_timeRotation = timeRotation;
                m_waitTime = StartWaitTime;
                navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Enemy").transform.position) >= 2.5f)
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
                
            }
        }
    }    
    private void Patroling()
    {
        if(m_playerNear)
        {
            if(m_timeRotation <= 0)
            {
                
                Move(speedWalk);
                lookingPlayer(playersLastPosition);
            }
            else
            {
                Stop();
                m_timeRotation -= Time.deltaTime;
            }
        }
        else
        {
            m_playerNear = false;
            playersLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_waitTime = StartWaitTime;
                }
                else
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
            }
        }
    }
    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }
    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }
    public void NextPoint()
    {
        m_currentWaypointIndex = (m_currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
    }
    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }
    void lookingPlayer(Vector3 Player)
    {
        navMeshAgent.SetDestination(Player);
        if(Vector3.Distance(transform.position,Player) <= 0.3f)
        {
            if (m_waitTime <= 0)
            {
                m_playerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
                m_waitTime = StartWaitTime;
                m_timeRotation = timeRotation;

            }
            else
            {
                Stop();
                m_waitTime -= Time.deltaTime;
            }
           
        }
        else
        {
            m_playerNear = false;
            playersLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_currentWaypointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_waitTime = StartWaitTime;
                }
                else
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
            }
        }
    }
    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float distToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    m_playerInrange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_playerInrange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_playerInrange = false;
            }
            if (m_playerInrange)
            {
                m_playerPosition = player.transform.position;
            }
        }
    }
}
