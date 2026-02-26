using UnityEngine;

public class CharacterControllerVu : MonoBehaviour
{
    public Rigidbody rb;
    private bool isGrounded = false;

    public Animator animator;
    void Start()
    {
        
    }

 
    void Update()
    {
        HandleMovement();
    }
    public void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
             
            
        }
        if (movement == Vector3.zero)
        {
            animator.SetBool("isWalkingVu", false);
        }
        else 
        {
            animator.SetBool("isWalkingVu", true); 
        }

        rb.MovePosition(transform.position + movement * 5 * Time.fixedDeltaTime);

        HandleRotation(movement);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public void HandleRotation(Vector3 enemyVuMovementInput)
    {
        Vector3 lookDirection = enemyVuMovementInput;
        lookDirection.y = 0; 

        if(lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }
}
