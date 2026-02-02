using UnityEngine;

public class EnemyRagdoll_Hoang : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        RagdollActivate(false);
    }
    public void RagdollActivate(bool active)
    {
       foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
    }
    public void CollidersActivate(bool active)
    {
        foreach (Collider cl in ragdollColliders)
        {
            cl.enabled = active;
        }
    }
}
