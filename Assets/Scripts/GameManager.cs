using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    private AudioSource audioSource;
    public AudioClip gameOverSound;

    public TMP_Text score;

    double startTime = 0.0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startTime = AudioSettings.dspTime;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioListener.pause = false;
    }

    public void OnGameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
        score.text = "You survived: " + (AudioSettings.dspTime - startTime).ToString("F2") + " seconds";
        gameOverPanel.SetActive(true);
        Debug.Log("Player caught!");
        // wait for the sound to finish before pausing the game
        Invoke("MuteAudio", gameOverSound.length);
    }

    private void MuteAudio()
    {
        AudioListener.pause = true;
    }
}
