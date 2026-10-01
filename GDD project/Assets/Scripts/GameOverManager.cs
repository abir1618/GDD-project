using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    public GameObject gameOverPanel;

    // Audio
    public AudioSource musicSource;
    public AudioSource buttonSource;

    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

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

        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Stop gameplay music
        if (AudioManager.instance != null &&
            AudioManager.instance.musicSource != null)
        {
            AudioManager.instance.musicSource.Stop();
        }

        // Play Game Over music ONCE
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        Time.timeScale = 0f;
    }

    public void PlayButtonSound()
    {
        if (buttonSource != null && buttonClickSound != null)
            buttonSource.PlayOneShot(buttonClickSound);
    }

    public void RestartGame()
    {
        PlayButtonSound();

        Time.timeScale = 1f;

        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        PlayButtonSound();

        Time.timeScale = 1f;

        SceneManager.LoadScene("StartMenu");
    }
}