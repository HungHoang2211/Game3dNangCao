using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform camera;

    void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
