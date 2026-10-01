using UnityEngine;

public class Saw : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 0.5f;

    private float nextDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null)
            return;

        if (player.isDead)
            return;

        if (Time.time >= nextDamageTime)
        {
            player.TakeDamage(damage);

            nextDamageTime = Time.time + damageInterval;

            Debug.Log("Saw damaged player!");
        }
    }
}