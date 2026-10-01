using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public int damage = 1;
    public float arrowSpeed = 7f;
    public float lifeTime = 4f;

    private Rigidbody2D rb;
    private Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void FixedUpdate()
    {
        rb.velocity = direction * arrowSpeed;
    }

}