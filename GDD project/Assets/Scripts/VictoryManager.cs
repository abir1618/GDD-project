using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager instance;

    public GameObject victoryPanel;

    // Victory Music
    public AudioSource musicSource;
    public AudioClip victoryMusic;

    // Button Sound
    public AudioSource buttonSource;
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

        victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);

        // Stop gameplay music
        if (AudioManager.instance != null &&
            AudioManager.instance.musicSource != null)
        {
            AudioManager.instance.musicSource.Stop();
        }

        // Play victory music once
        if (musicSource != null && victoryMusic != null)
        {
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        Time.timeScale = 0f;
    }

    public void PlayButtonSound()
    {
        if (buttonSource != null && buttonClickSound != null)
        {
            buttonSource.PlayOneShot(buttonClickSound);
        }
    }

    public void Replay()
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