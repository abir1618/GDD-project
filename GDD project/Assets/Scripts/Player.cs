using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth = 5;

    public static Player instance;
    public bool isDead { get; private set; }

    // Components
    private Animator animator;
    public Rigidbody2D rb;

    // Movement Settings
    public float jumpHeight = 7f;
    public float moveSpeed = 5f;

    // Player States
    private float movement;
    private bool isGround;
    private bool facingRight;

    // Ground Check
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    public LayerMask whatIsWater;

    // Checkpoint
    public Transform checkpoint;

    // Prevent multiple water hits
    private bool touchingWater = false;

    // Arrow
    public GameObject arrowPrefab;
    public Transform spawnPosition;
    public float arrowSpeed = 7f;

    // Orange
    public int currentOranges { get; private set; }


    void Start()
    {
        // Initial States
        isGround = true;
        facingRight = true;

        // Get Animator
        animator = GetComponent<Animator>();

        // Player Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        currentOranges = 0;
    }


    void Update()
    {
        // Normal death
        if (maxHealth <= 0 && !isDead)
        {
            Die();
        }

        // Don't control player when dead
        if (isDead)
        {
            return;
        }

        // Horizontal Input
        movement = Input.GetAxis("Horizontal");

        // Check Ground + Water
        CheckGround();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Player Direction
        Flip();

        // Run Animation
        PlayRunAnimation();

        // Fire Animation
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Fire");
        }
    }


    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        MovePlayer();
    }


    // PLAYER MOVEMENT
    void MovePlayer()
    {
        Vector2 velocity = rb.velocity;

        velocity.x = movement * moveSpeed;

        rb.velocity = velocity;
    }


    // CHECK GROUND + WATER
    void CheckGround()
    {
        // Ground Check
        Collider2D collInfo = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            whatIsGround
        );

        if (collInfo != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }


        // Water Check
        Collider2D waterInfo = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            whatIsWater
        );

        if (waterInfo != null && !touchingWater)
        {
            touchingWater = true;

            WaterHit();
        }

        // Reset once checker leaves water
        if (waterInfo == null)
        {
            touchingWater = false;
        }
    }


    // WATER HIT
    void WaterHit()
    {
        if (isDead)
        {
            return;
        }

        // Remove 1 health
        maxHealth--;

        Debug.Log("Touched water! Health remaining: " + maxHealth);


        // Health reached zero
        if (maxHealth <= 0)
        {
            Die();
            return;
        }


        // Still has health
        RespawnAtCheckpoint();
    }


    // RESPAWN
    void RespawnAtCheckpoint()
    {
        if (checkpoint == null)
        {
            Debug.LogWarning("Checkpoint is not assigned!");
            return;
        }

        // Stop movement
        rb.velocity = Vector2.zero;

        // Move player to checkpoint
        transform.position = checkpoint.position;

        // Reset animations
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Fire");
        animator.ResetTrigger("Hurt");

        animator.SetBool("Jump", false);
        animator.SetFloat("Run", 0f);

        // Reset movement
        movement = 0f;

        // Prevent another water hit immediately
        touchingWater = true;

        // Respawn sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRespawn();
        }

        Debug.Log("Player respawned at checkpoint!");
    }


    // JUMP
    void Jump()
    {
        if (isGround)
        {
            Vector2 velocity = rb.velocity;

            velocity.y = jumpHeight;

            rb.velocity = velocity;

            animator.SetBool("Jump", true);
        }
    }


    // FIRE ARROW
    public void FireArrow()
    {
        if (arrowPrefab == null || spawnPosition == null)
        {
            return;
        }

        GameObject tempArrowPrefab = Instantiate(
            arrowPrefab,
            spawnPosition.position,
            spawnPosition.rotation
        );

        Rigidbody2D arrowRb =
            tempArrowPrefab.GetComponent<Rigidbody2D>();

        if (arrowRb != null)
        {
            arrowRb.velocity =
                spawnPosition.right * arrowSpeed;
        }
    }


    // RUN ANIMATION
    void PlayRunAnimation()
    {
        if (Mathf.Abs(movement) > 0f)
        {
            animator.SetFloat("Run", 1f);
        }
        else
        {
            animator.SetFloat("Run", 0f);
        }
    }


    // FLIP PLAYER
    void Flip()
    {
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles =
                new Vector3(0f, -180f, 0f);

            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles =
                new Vector3(0f, 0f, 0f);

            facingRight = true;
        }
    }


    // TAKE DAMAGE
    public void TakeDamage(int damageAmount)
    {
        if (maxHealth <= 0 || isDead)
        {
            return;
        }

        maxHealth -= damageAmount;


        // Hurt animation only if still alive
        if (maxHealth > 0)
        {
            animator.SetTrigger("Hurt");

            // Hurt sound
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayPlayerHurt();
            }

            // Camera shake
            if (CameraShake.instance != null)
            {
                CameraShake.instance.Shake(
                    2f,
                    0.12f
                );
            }
        }
    }


    // COLLISION DETECTION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("Jump", false);
        }
    }


    private void OnTriggerEnter2D(Collider2D coll)
    {
        // Enemy Arrow
        if (coll.gameObject.CompareTag("EnemyArrow"))
        {
            TakeDamage(1);

            Destroy(coll.gameObject);
        }


        // Orange
        if (coll.gameObject.CompareTag("Orange"))
        {
            currentOranges++;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayOrangeCollect();
            }

            Destroy(coll.gameObject);
        }
    }


    // GROUND CHECK GIZMO
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            groundCheckPoint.position,
            groundCheckRadius
        );
    }


    // DEATH
    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log(gameObject.name + " Died!");


        // Stop horizontal movement
        // Keep gravity active
        rb.velocity = new Vector2(
            0f,
            rb.velocity.y
        );


        // Stop other animations
        animator.SetBool("Jump", false);
        animator.SetFloat("Run", 0f);

        animator.ResetTrigger("Fire");
        animator.ResetTrigger("Hurt");


        // Play Death animation
        animator.SetTrigger("Death");


        // Camera shake
        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake(
                4f,
                0.18f
            );
        }


        // Death sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayPlayerDeath();
        }


        // Start death sequence
        StartCoroutine(DeathSequence());
    }


    // DEATH SEQUENCE
    IEnumerator DeathSequence()
    {
        // Wait 3 seconds for death animation
        yield return new WaitForSeconds(3f);


        // Show Game Over
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowGameOver();
        }


        // Destroy player
        Destroy(gameObject);
    }
}