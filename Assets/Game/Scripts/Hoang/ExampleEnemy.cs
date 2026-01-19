using UnityEngine;
using UnityEngine.AI;

public class ExampleEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    private void Update()
    {
        agent.destination = target.position;
    }
}
