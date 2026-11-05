using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    [Header("Audio")]
    public Slider volumeSlider;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }

    // Start the game
    public void StartGame()
    {
        SceneManager.LoadScene("TestGameScene");// Load scene of game (Current Scene of game is TestGameScene)
    }

    // Open options
    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Return to main menu
    public void BackToMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Adjust volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat("Volume", volume);
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting Game");

        Application.Quit();
    }
}