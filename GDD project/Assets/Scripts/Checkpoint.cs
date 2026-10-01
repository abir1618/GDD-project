using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool alreadyActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null && !alreadyActivated)
        {
            alreadyActivated = true;

            // Set this as the new respawn point
            player.checkpoint = transform;

            // Restore health
            player.maxHealth = 5;

            if (AudioManager.instance != null)
{
    AudioManager.instance.PlayCheckpoint();
}

            Debug.Log("Checkpoint activated: " + gameObject.name);
            Debug.Log("Health restored!");
        }
    }
}