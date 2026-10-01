using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private Animator animator;
    private bool levelCompleted = false;

    public Transform playerStopPoint;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted)
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            levelCompleted = true;

            // Get Player components
            Animator playerAnimator = player.GetComponent<Animator>();
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            // Stop player completely
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }

            // Move player to the middle of the door
            if (playerStopPoint != null)
            {
                player.transform.position = playerStopPoint.position;
            }

            // Stop running and jumping
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Run", 0f);
                playerAnimator.SetBool("Jump", false);
                playerAnimator.ResetTrigger("Fire");
                playerAnimator.ResetTrigger("Hurt");
            }

            // Stop player control
            player.enabled = false;

            // Open door
            animator.SetTrigger("Open");

            // Complete level after door opens
            Invoke(nameof(CompleteLevel), 2f);
        }
    }

    void CompleteLevel()
{
    Debug.Log("Level Complete!");

    if (VictoryManager.instance != null)
    {
        VictoryManager.instance.ShowVictory();
    }
}
}