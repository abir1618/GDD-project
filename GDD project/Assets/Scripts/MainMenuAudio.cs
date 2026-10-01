using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource buttonSource;

    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    void Start()
    {
        // Main menu music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayButtonSound()
    {
        if (buttonSource != null && buttonClickSound != null)
        {
            buttonSource.PlayOneShot(buttonClickSound);
        }
    }
}