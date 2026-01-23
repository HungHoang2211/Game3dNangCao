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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
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

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * walkSpeed);
        }
        animator.SetFloat("Speed", movementDirection.magnitude);
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
