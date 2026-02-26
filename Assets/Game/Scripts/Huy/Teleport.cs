using UnityEngine;

public class Teleport : MonoBehaviour
{
    
    public Transform targetPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetPoint.position;
        }
    }
}
