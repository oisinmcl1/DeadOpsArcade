using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    private Rigidbody rb; // Reference to the Rigidbody
    private Animator animator; // Reference to the Animator
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    private float health;
    public Shooter s;
    Vector3 spherePos;
    Vector3 velocity;
    Vector3 direction;

    void Start()
    {
        // transform.position = new Vector3(0f, 0f, 0f);
        transform.position = new Vector3(2.97f, 6.38f, 3.52f);
        // transform.position = new Vector3(-497.59f, 77.07f, -511.17f);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = 5f;
        s = GetComponentInChildren<Shooter>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
    }

    void HandleMouseRotation()
    {
        // Calculate the mouse position in the world space
        Vector3 mouseScreenPosition =
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // Calculate the direction vector from the character to the mouse position
        direction = (mouseWorldPosition - transform.position).normalized;

        direction.y = 0;

        // If the mouse is significantly far enough, rotate toward the mouse
        if (direction.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, 720 * Time.fixedDeltaTime));
        }
    }

    void HandleMovement()
    {
        // Calculate forward and right directions based on the player's current facing direction
        Vector3 forward = direction;
        Vector3 right = Vector3.Cross(transform.up, forward).normalized;

        // Get input for movement (horizontal and vertical)
        float moveRight = Input.GetAxis("Horizontal");
        float moveForward = Input.GetAxis("Vertical");

        // Calculate the movement vector in world space relative to the player's facing direction
        Vector3 movement = ((forward * moveForward) + (right * moveRight)).normalized;
        //Debug.Log("movement: " + movement);

        // Determine the relative movement direction
        float forwardDot = Vector3.Dot(movement, forward); // Positive for forward, negative for backward
        float rightDot = Vector3.Dot(movement, right); // Positive for right, negative for left
        //Debug.Log("forward dot: " + forwardDot);
        //Debug.Log("right dot: " + rightDot);
        // Set animator parameters based on the direction
        if (animator != null)
        {
            animator.SetBool("hasPistol", s.getGunType() == Shooter.GunType.Pistol);
            animator.SetBool("hisRifle", s.getGunType() == Shooter.GunType.Rifle);

            animator.SetBool("isMovingForward", forwardDot > 0.1f);
            animator.SetBool("isMovingBackward", forwardDot < -0.1f);
            animator.SetBool("isMovingRight", rightDot > 0.1f);
            animator.SetBool("isMovingLeft", rightDot < -0.1f);

            animator.SetBool("isMovingSideways", rightDot > 0.1 || rightDot < -0.1);
        }

        // Apply movement if there is any
        if (movement.magnitude > 0.1f)
        {
            Vector3 moveDirection = movement * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveDirection);
        }
    }

    void FixedUpdate()
    {
        HandleMouseRotation();
        HandleMovement();
    }


    bool IsGrounded()
    {
        // Calculate position for ground check sphere
        Vector3 spherePos =
            new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(spherePos, 0.3f, groundMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 spherePos = transform.position - new Vector3(transform.position.x, groundYOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePos, 0.2f);
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("zombie"))
        {
            Debug.Log("Player taking damage!");
        }
    }
}