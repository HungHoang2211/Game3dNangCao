using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public PlayerControls controls;
    private CharacterController characterController;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    private Vector3 movementDirection;
    private Vector3 lookingDirection;

    [Header("Aim Info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector2 MoveInput;
    private Vector2 AimInput;

    Animator animator;

    [Header("Gravity")]
    [SerializeField] public float gravity = -20f;
    private float yVelocity;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Lấy Animator ở object con (thường là Model nhân vật)
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Character.Movement.performed += context => MoveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => MoveInput = Vector2.zero;

        controls.Character.Aim.performed += context => AimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => AimInput = Vector2.zero;
    }

    void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
    }

    private void AimTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(AimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hitInfo.point - transform.position;
            lookingDirection.y = 0f;
            lookingDirection.Normalize();

            transform.forward = lookingDirection;

            aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(MoveInput.x, 0, MoveInput.y);

        if (characterController.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 move = movementDirection * walkSpeed + Vector3.up * yVelocity;

        characterController.Move(move * Time.deltaTime);

        //if (movementDirection.magnitude > 0)
        //{
        //    characterController.Move(movementDirection * Time.deltaTime * walkSpeed);
        //}

        // CẬP NHẬT ANIMATOR TẠI ĐÂY
        if (animator != null)
        {
            // hướng di chuyển theo LOCAL của nhân vật
            Vector3 localMove = transform.InverseTransformDirection(movementDirection);

            animator.SetFloat("InputX", localMove.x);
            animator.SetFloat("InputY", localMove.z);
            animator.SetFloat("Speed", movementDirection.magnitude);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}