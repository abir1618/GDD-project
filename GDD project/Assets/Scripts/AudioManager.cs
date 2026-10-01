using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Music
    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    // Sound Effects
    public AudioSource audioSource;

    public AudioClip respawnSound;
    public AudioClip checkpointSound;
    public AudioClip orangeCollectSound;

    public AudioClip playerHurtSound;
    public AudioClip playerDeathSound;

    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Background music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }


    public void PlayRespawn()
    {
        if (respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }
    }


    public void PlayCheckpoint()
    {
        if (checkpointSound != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }
    }


    public void PlayOrangeCollect()
    {
        if (orangeCollectSound != null)
        {
            audioSource.PlayOneShot(orangeCollectSound);
        }
    }


    public void PlayPlayerHurt()
    {
        if (playerHurtSound != null)
        {
            audioSource.PlayOneShot(playerHurtSound);
        }
    }


    public void PlayPlayerDeath()
    {
        if (playerDeathSound != null)
        {
            audioSource.PlayOneShot(playerDeathSound);
        }
    }


    public void PlayEnemyHurt()
    {
        if (enemyHurtSound != null)
        {
            audioSource.PlayOneShot(enemyHurtSound);
        }
    }


    public void PlayEnemyDeath()
    {
        if (enemyDeathSound != null)
        {
            audioSource.PlayOneShot(enemyDeathSound);
        }
    }
}