using UnityEngine;

public class CameraControllerVu : MonoBehaviour
{
    [Header("Atributes Camera")]
    public Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.7f, -4f);
    private Quaternion rotation;

    private float x;
    private float y;
    [SerializeField] private float xSpeed = 4f;
    [SerializeField] private float ySpeed = 5f;

    [SerializeField] private float xMinRotation = -360f;
    [SerializeField] private float xMaxRotation = 360f;
    [SerializeField] private float yMinRotation = 10f;
    [SerializeField] private float yMaxRotation = 80f;
    void Start()
    {
        Vector3 angles = this.transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        CameraMove();
        rotation = Quaternion.Euler(y, x, 0);

        Vector3 desiredPosition = offset;
        Vector3 position = rotation * desiredPosition + target.position;
        transform.position = position;
        transform.rotation = rotation;
    }
    public void CameraMove()
    {
        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;
        
        x = ClampAngle(x, xMinRotation, xMaxRotation);
        y = ClampAngle(y, yMinRotation, yMaxRotation);
    }
    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
