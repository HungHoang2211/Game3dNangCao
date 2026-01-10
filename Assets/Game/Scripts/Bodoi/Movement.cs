using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove3D : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -20f;

    private Vector2 moveInput;
    private float verticalVelocity; 
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        
        controller.Move(move * speed * Time.deltaTime);

        
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; 
        else
            verticalVelocity += gravity * Time.deltaTime;

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
