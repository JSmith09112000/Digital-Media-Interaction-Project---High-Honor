using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    [Header("Screen Panel")]
    public GameObject deathScreenPanel;

    //Disables the death screen at the start of the game and ensures the game is not paused
    private void Start()
    {
        deathScreenPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    //Runs when the player dies
    public void PlayerDied()
    {
        deathScreenPanel.SetActive(true);

        //Freezes game time
        Time.timeScale = 0f;

    }

    //Loads the Main Menu, bringng the player back to the begining
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    //Leaves the game
    public void ExitGame()
    {
        Debug.Log("Leaving the Game");

        Application.Quit();
    }

}
