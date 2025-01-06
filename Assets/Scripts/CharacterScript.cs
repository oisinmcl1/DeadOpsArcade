using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    private Rigidbody rb; // Reference to the Rigidbody
    private Animator animator; // Reference to the Animator
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    private float health;
    private bool isAlive;
    public Shooter s;
    // public GameManager gm;
    public GameManagerScript gms;
    Vector3 spherePos;
    Vector3 velocity;
    Vector3 direction;
    private bool isFrozen;
    public bool onIsland;

    void Start()
    {
        // transform.position = new Vector3(0f, 0f, 0f);
        transform.position = new Vector3(2.97f, 6.38f, 3.52f);
        // transform.position = new Vector3(-497.59f, 77.07f, -511.17f);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = 5f;
        isAlive = true;
        s = GetComponentInChildren<Shooter>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        gms = FindObjectOfType<GameManagerScript>();
        isFrozen = false;
        onIsland = true;
    }

    void Update()
    {
    }

    /*void HandleMouseRotation()
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
    }*/
    void HandleAiming()
    {
        bool isUsingController = false;

        // if gamepad is connected, use the right stick for aiming
        if (Gamepad.current != null)
        {
            Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
            if (rightStick.magnitude > 0.1f)
            {
                isUsingController = true;

                // Convert 2D stick input into a 3D direction
                direction = new Vector3(rightStick.x, 0f, rightStick.y).normalized;

                // Rotate toward that direction
                if (direction.magnitude > 0.1f)
                {
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, 720 * Time.fixedDeltaTime));
                }

                // exit to prevent mouse from overriding
                return;
            }
        }

        // fallback to mouse if controller is not being used
        if (!isUsingController)
        {
            Vector3 mouseScreenPosition = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.y
            );

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            // Calculate direction from player to mouse position
            direction = (mouseWorldPosition - transform.position).normalized;
            direction.y = 0;

            if (direction.magnitude > 0.1f)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, 720 * Time.fixedDeltaTime));
            }
        }
    }
    
    void HandleMovement()
    {
        if (isFrozen)
        {
            return;
        }
        
        // Determine the forward direction based on the magnitude of the direction vector
        // Vector3 forward = direction;
        Vector3 forward;
        if (direction.magnitude > 0.1f)
        {
            // If the direction vector's magnitude is greater than 0.1, use the direction vector
            forward = direction;
        }
        else
        {
            // Otherwise, use the transform's forward direction
            forward = transform.forward;
        }
        Vector3 right = Vector3.Cross(transform.up, forward).normalized;

        // Get input for movement (horizontal and vertical) from both keyboard and controller
        // Keyboard inputs
        float moveRight = Input.GetAxis("Horizontal");
        float moveForward = Input.GetAxis("Vertical");

        // Check if a controller is connected
        if (Gamepad.current != null) 
        {
            moveRight = Gamepad.current.leftStick.x.ReadValue();
            moveForward = Gamepad.current.leftStick.y.ReadValue();
        }

        // Calculate the movement vector in world space relative to the player's facing direction
        Vector3 movement = ((forward * moveForward) + (right * moveRight)).normalized;

        // Figure out the movement direction
        float forwardDot = Vector3.Dot(movement, forward);
        float rightDot = Vector3.Dot(movement, right);

        // Set animator parameters based on the direction
        if (animator != null)
        {
            animator.SetBool("hasPistol", 
                s.getGunType() == Shooter.GunType.Pistol || 
                s.getGunType() == Shooter.GunType.Revolver);
            
            animator.SetBool("hisRifle", s.getGunType() == Shooter.GunType.Rifle);

            animator.SetBool("isMovingForward", forwardDot > 0.1f);
            animator.SetBool("isMovingBackward", forwardDot < -0.1f);
            animator.SetBool("isMovingRight", rightDot > 0.1f);
            animator.SetBool("isMovingLeft", rightDot < -0.1f);

            animator.SetBool("isMovingSideways", rightDot > 0.1f || rightDot < -0.1f);
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
        if (!isAlive)
        {
            return;
        }
        
        // Debug.Log(onIsland);
        
        // HandleMouseRotation();
        HandleAiming();
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

    /*void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("zombie"))
        {
            Debug.Log("Player taking damage!\n" +
                      "Health is now: " + health);
            
            health -= 1f;

            if (health <= 0)
            {
                // Kill player if health is 0
                Debug.Log("Player died!");
                Destroy(gameObject);
                
                // Using gamemanager call gameOver func
                GameManagerScript gm = FindObjectOfType<GameManagerScript>();
                gm.gameOver();
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        // player will die if teleported off island without disabling this trigger
        if (!onIsland)
        {
            return;
        }
        
        if (other.gameObject.CompareTag("island"))
        {
            Debug.Log("Player left island");
            // transform.position = new Vector3(2.97f, 6.38f, 3.52f);
            takeDamage(999f);
        }
    }
    
    IEnumerator die()
    {
        Debug.Log("Player dying");
        isAlive = false;
        isFrozen = true;
        
        Debug.Log("Playing death animation");
        animator.SetBool("isDead", true);
        
        // stop player rotation because user can just start spinning them in the death animation lol
        // rb.constraints = RigidbodyConstraints.FreezeAll;
        
        gms.afterPlayerDies();
        
        // Wait for the animation to finish
        yield return new WaitForSeconds(6f);
        
        Debug.Log("This is the player and i am destroying myself why did you let me die");
        Destroy(gameObject);
    }

    public void takeDamage(float damage) {
        if(!isAlive) {
            return;
        }
        health -= damage;
        Debug.Log("Player taking damage!, health: " + health);
        
        if(health <= 0) {
            StartCoroutine(die());
        }
    }
    
    // due to state machine for animations, need to freeze player for a moment to reset animations so it doesnt bug out
    public void freezePlayer()
    {
        Debug.Log("Freezing Player");
        if (!isFrozen)
        {
            StartCoroutine(freeze());
        }
    }
    IEnumerator freeze()
    {
        isFrozen = true;

        // reset the animations to idle
        if (animator != null)
        {
            animator.SetBool("isMovingForward", false);
            animator.SetBool("isMovingBackward", false);
            animator.SetBool("isMovingRight", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingSideways", false);
        }

        // wait a small bit to ensure animations are reset
        yield return new WaitForSeconds(0.1f);

        isFrozen = false;
    }
}