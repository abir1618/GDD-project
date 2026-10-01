using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public int maxHealth = 3;

    // Player Detection
    public float attackRangeRadius = 8f;
    public LayerMask whatIsPlayer;

    // Facing
    // rotation 0 = facing left
    // rotation 180 = facing right
    private bool facingRight = false;

    // Arrow
    public GameObject arrowPrefab;
    public Transform spawnPosition;
    public float fireRate = 1.5f;
    private float nextFireTime = 0f;

    // Ground / Water Check
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsWater;

    private bool touchingWater = false;

    // Components
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;

    // Death
    private bool isDead = false;

    // Floating damage text
    public GameObject floatingTextPrefab;
    public Transform textSpawnPoint;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }


    void Update()
    {
        // Normal health death
        if (maxHealth <= 0 && !isDead)
        {
            Die();
        }

        // Don't do anything while dead
        if (isDead)
        {
            return;
        }

        // Check water
        CheckWater();

        // Stop if water killed enemy
        if (isDead)
        {
            return;
        }


        // Check player
        if (Player.instance == null || Player.instance.isDead)
        {
            animator.SetBool("Attack", false);
            return;
        }


        Collider2D collInfo = Physics2D.OverlapCircle(
            transform.position,
            attackRangeRadius,
            whatIsPlayer
        );


        if (collInfo)
        {
            FacePlayer();


            // Only play attack animation
            // when fire cooldown is ready
            if (Time.time >= nextFireTime)
            {
                animator.SetBool("Attack", true);
            }
            else
            {
                animator.SetBool("Attack", false);
            }
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }


    // ==========================================
    // WATER CHECK
    // ==========================================

    void CheckWater()
    {
        if (groundCheckPoint == null)
        {
            return;
        }


        Collider2D waterInfo = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            whatIsWater
        );


        if (waterInfo != null && !touchingWater)
        {
            touchingWater = true;

            // Enemy dies immediately
            Die();
        }


        // Reset when checker leaves water
        if (waterInfo == null)
        {
            touchingWater = false;
        }
    }


    // ==========================================
    // FACE PLAYER
    // ==========================================

    void FacePlayer()
    {
        float playerX =
            Player.instance.transform.position.x;


        if (playerX > transform.position.x && !facingRight)
        {
            transform.eulerAngles =
                new Vector3(0f, -180f, 0f);

            facingRight = true;
        }
        else if (playerX < transform.position.x && facingRight)
        {
            transform.eulerAngles =
                new Vector3(0f, 0f, 0f);

            facingRight = false;
        }
    }


    // ==========================================
    // FIRE ARROW
    // ==========================================

    // Called by Attack animation event
    public void FireArrow()
    {
        if (isDead)
        {
            return;
        }


        if (Time.time < nextFireTime)
        {
            return;
        }


        nextFireTime =
            Time.time + fireRate;


        Vector2 dir =
            facingRight
                ? Vector2.right
                : Vector2.left;


        Quaternion rot =
            facingRight
                ? Quaternion.identity
                : Quaternion.Euler(0f, 180f, 0f);


        if (arrowPrefab == null || spawnPosition == null)
        {
            return;
        }


        GameObject tempArrow = Instantiate(
            arrowPrefab,
            spawnPosition.position,
            rot
        );


        EnemyArrow enemyArrow =
            tempArrow.GetComponent<EnemyArrow>();


        if (enemyArrow != null)
        {
            enemyArrow.SetDirection(dir);
        }
    }


    // ==========================================
    // TAKE DAMAGE
    // ==========================================

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
                AudioManager.instance.PlayEnemyHurt();
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


        // Floating damage text
        if (floatingTextPrefab != null &&
            textSpawnPoint != null)
        {
            Instantiate(
                floatingTextPrefab,
                textSpawnPoint.position,
                Quaternion.identity
            );
        }
    }


    // ==========================================
    // PLAYER ARROW COLLISION
    // ==========================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Arrow"))
        {
            TakeDamage(1);

            Destroy(other.gameObject);
        }
    }


    // ==========================================
    // DEATH
    // ==========================================

    void Die()
    {
        if (isDead)
        {
            return;
        }


        isDead = true;


        Debug.Log(gameObject.name + " Died!");


        // Stop attack animation
        animator.SetBool("Attack", false);


        // Death animation
        animator.SetBool("Death", true);


        // Death sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayEnemyDeath();
        }


        // Stop movement
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;


        // Disable collider
        col.enabled = false;


        // Death animation event
        // calls ShakeCamera()


        // Destroy after death animation
        Destroy(gameObject, 5f);
    }


    // ==========================================
    // DEATH ANIMATION EVENT
    // ==========================================

    public void ShakeCamera()
    {
        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake(
                4f,
                0.18f
            );
        }
    }


    // ==========================================
    // GIZMOS
    // ==========================================

    private void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRangeRadius
        );


        // Water checker
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                groundCheckPoint.position,
                groundCheckRadius
            );
        }
    }
}