using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Options Menu")]
    public GameObject optionsPanel;  

    [Header("Player Gameplay Inputs")]
    public Behaviour[] gameplayInputComponents;

    private bool isPaused = false;

    private bool[] previousInputStates;

    private void Start()
    {
        // Make sure the game starts unpaused
        Time.timeScale = 1f;

        pausePanel.SetActive(false);

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        previousInputStates =
            new bool[gameplayInputComponents.Length];
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                if (optionsPanel != null &&
                    optionsPanel.activeSelf)
                {
                    BackToPauseMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        // Freeze game time
        Time.timeScale = 0f;

        // Disable player gameplay inputs
        for (int i = 0; i < gameplayInputComponents.Length; i++)
        {
            if (gameplayInputComponents[i] != null)
            {
                previousInputStates[i] =
                    gameplayInputComponents[i].enabled;

                gameplayInputComponents[i].enabled = false;
            }
        }

        // Show pause menu
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        // Restore game time
        Time.timeScale = 1f;

        // Restore player gameplay inputs
        for (int i = 0; i < gameplayInputComponents.Length; i++)
        {
            if (gameplayInputComponents[i] != null)
            {
                gameplayInputComponents[i].enabled =
                    previousInputStates[i];
            }
        }

        // Hide pause menu
        pausePanel.SetActive(false);

        if (optionsPanel != null)
         {
             optionsPanel.SetActive(false);
         } 
    }

    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            pausePanel.SetActive(false);
            optionsPanel.SetActive(true);
        }
    }

    public void BackToPauseMenu()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        pausePanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        // Prevent the next scene from being paused
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        // Safety reset when leaving the gameplay scene
        Time.timeScale = 1f;
    }
}