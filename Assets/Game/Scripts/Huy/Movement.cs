using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove3D : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.8f;
    public float rotateSpeed = 10f;

    private Vector2 moveInput;
    private Vector3 velocity;
    private CharacterController controller;

    Animator animator;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove =
            move * speed +
            Vector3.up * velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        if (animator != null)
        {
            Vector3 localMove = transform.InverseTransformDirection(move);
            animator.SetFloat("InputX", localMove.x);
            animator.SetFloat("InputY", localMove.z);
            animator.SetFloat("Speed", move.magnitude);
        }
    }

}
