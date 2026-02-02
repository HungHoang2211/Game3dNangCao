using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target_Hoang : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}