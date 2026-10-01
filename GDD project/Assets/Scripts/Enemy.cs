using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float walkSpeed = 1.5f;

    // Ground Check
    public Transform groundCheckPoint;
    public float distance = 0.3f;
    public LayerMask whatIsGround;
    public LayerMask whatIsWater;
    private bool facingLeft;

    // Water
    private bool touchingWater = false;

    // Player Detection
    public float attackRangeRadius = 4f;
    public LayerMask whatIsPlayer;
    public Transform player;

    // Chase Settings
    public float chaseSpeed = 2f;
    public float retrieveDistance = 3f;

    // Components
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    // Death
    private bool isDead = false;

    // Floating Damage Text
    public GameObject floatingTextPrefab;
    public Transform textSpawnPoint;

    // Attack
    public Transform attackPoint;
    public float attackRadius = 1f;


    void Start()
    {
        facingLeft = true;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        // Check normal health death
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

        // Don't continue if water killed the enemy
        if (isDead)
        {
            return;
        }


        // Check if player is alive
        bool playerAlive =
            Player.instance != null &&
            !Player.instance.isDead;


        Collider2D collInfo = null;

        if (playerAlive)
        {
            collInfo = Physics2D.OverlapCircle(
                transform.position,
                attackRangeRadius,
                whatIsPlayer
            );
        }


        // PLAYER DETECTED
        if (collInfo)
        {
            // Face Player
            if (player.position.x > transform.position.x && facingLeft)
            {
                transform.eulerAngles =
                    new Vector3(0f, -180f, 0f);

                facingLeft = false;
            }
            else if (player.position.x < transform.position.x && !facingLeft)
            {
                transform.eulerAngles =
                    new Vector3(0f, 0f, 0f);

                facingLeft = true;
            }


            Vector2 targetPos = new Vector2(
                player.position.x,
                transform.position.y
            );


            // Chase Player
            if (Vector2.Distance(transform.position, targetPos) > retrieveDistance)
            {
                animator.SetBool("Attack", false);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPos,
                    chaseSpeed * Time.deltaTime
                );
            }
            else
            {
                animator.SetBool("Attack", true);
            }
        }


        // PLAYER NOT DETECTED
        else
        {
            animator.SetBool("Attack", false);


            // Normal Walking
            transform.Translate(
                Vector2.left *
                Time.deltaTime *
                walkSpeed
            );


            // Ground Check
            RaycastHit2D hitInfo = Physics2D.Raycast(
                groundCheckPoint.position,
                Vector2.down,
                distance,
                whatIsGround
            );


            // Turn around if there is no ground
            if (hitInfo == false)
            {
                if (facingLeft)
                {
                    transform.eulerAngles =
                        new Vector3(0f, -180f, 0f);

                    facingLeft = false;
                }
                else
                {
                    transform.eulerAngles =
                        new Vector3(0f, 0f, 0f);

                    facingLeft = true;
                }
            }
        }
    }


    // ==========================================
    // WATER CHECK
    // ==========================================

    void CheckWater()
    {
        Collider2D waterInfo = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            distance,
            whatIsWater
        );


        if (waterInfo != null && !touchingWater)
        {
            touchingWater = true;

            // Enemy dies immediately
            Die();
        }


        // Reset once checker leaves water
        if (waterInfo == null)
        {
            touchingWater = false;
        }
    }


    // ==========================================
    // ATTACK
    // ==========================================

    public void Attack()
    {
        if (attackPoint == null)
            return;

        Collider2D collInfo =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                whatIsPlayer
            );


        if (collInfo)
        {
            if (collInfo.gameObject.GetComponent<Player>() != null)
            {
                Player.instance.TakeDamage(1);
            }
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


        // Hurt animation + sound
        // Only if enemy is still alive
        if (maxHealth > 0)
        {
            animator.SetTrigger("Hurt");


            // Hurt sound
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayEnemyHurt();
            }


            // Small camera shake
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
    // ARROW COLLISION
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


        // Stop movement
        rb.velocity = Vector2.zero;


        // Stop attack animation
        animator.SetBool("Attack", false);


        // Death animation
        animator.SetBool("Death", true);


        // Death sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayEnemyDeath();
        }


        // Stop physics movement
        rb.gravityScale = 0f;


        // Disable collider
        boxCollider2D.enabled = false;


        // Death animation event
        // will call ShakeCamera()


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
        // Attack Range
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRangeRadius
        );


        // Ground + Water Check
        if (groundCheckPoint == null)
        {
            return;
        }


        // Ground ray
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            groundCheckPoint.position,
            Vector2.down * distance
        );


        // Attack point
        if (attackPoint == null)
        {
            return;
        }


        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}