using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementHung : MonoBehaviour
{
    public Transform Player;
    public float UpdateRate = 0.1f;
    private NavMeshAgent Agent;

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    public void StartChasing()
    {
        if (FollowCoroutine == null)
        {
            FollowCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);
        while (enabled)
        {
            Agent.SetDestination(Player.transform.position);
            yield return wait;
        }
    }
}
