using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieScript : MonoBehaviour
{
    public GameObject player;
    public CharacterScript cs;
    public float speed = 3f;
    public float avoidDistance = 1.5f; // Distance to check for obstacles
    public float rotationSpeed = 5f;
    public float attackDistance = 1.0f;
    public float damage = 1.0f;
    public float health;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cs = player.GetComponent<CharacterScript>();
        animator = GetComponent<Animator>();
        health = 2f;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
    }
    
    public void setHealth(float h) {
        this.health = h;
    }
    void checkHealth() {
        if (health <= 0)
        {
            Debug.Log("Health is less than 0, destroying myself");
            
            // Notify the gms that a zombie has died
            GameManagerScript gms = FindObjectOfType<GameManagerScript>();
            gms.zombieDead(this.gameObject);
            
            // zombie needs to go away
            GameObject.Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
   void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            AttackPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            cs.takeDamage(damage);
        }
        
        else if(other.gameObject.CompareTag("bullet")) {
            Bullet bullet = other.GetComponent<Bullet>();
            
            if (bullet != null)
            {
                float damage = bullet.getDamage();  // Ensure Bullet class has getDamage method
                health -= damage;
                Debug.Log("Zombie hit by bullet, remaining health: " + health);
                checkHealth();
            }
            else
            {
                Debug.LogWarning("Bullet script missing on object with 'bullet' tag!");
            }
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Check for obstacles directly in front of the zombie
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, avoidDistance))
        {
            // If there's an obstacle, rotate to avoid it
            Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            direction = Vector3.Lerp(direction, avoidDirection, rotationSpeed * Time.deltaTime);
        }

        // Move towards the player or avoid the obstacle
        transform.position += direction * speed * Time.deltaTime;

        // Rotate to face the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // Set animation to running
        animator.SetBool("isMoving", true);
        animator.SetBool("isAttacking", false);
    }

        void AttackPlayer()
    {
        // Set animation to attacking
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);

        // Implement attack logic (e.g., reduce player health)
        //Debug.Log("Zombie is attacking the player!");
    }

    void FixedUpdate() {

    }
}
