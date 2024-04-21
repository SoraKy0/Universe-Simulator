using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI; //Reference  to the pause menu UI

    public TMP_Dropdown resolutionDropdown; //Reference to the dropdown for the resulutions

    // Start is called before the first frame update.
    void Start()
    {
        Resolution[] resolutions = Screen.resolutions; //Stores a list of all avaliable screen resolutions

        resolutionDropdown.ClearOptions();

        //Create a list to store resolution options to be put in the dropdown
        List<string> options = new List<string>();

        //Find the current resolution of screen
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            //this is the way the resolutions with the width height and refresh rate will be added to the list
            RefreshRate refreshRate = resolutions[i].refreshRateRatio;
            string option = $"{resolutions[i].width} x {resolutions[i].height} -{resolutions[i].refreshRate}Hz"; options.Add(option);

            //Check resolution of screen matches what the dropbox says
            RefreshRate currentRefreshRate = Screen.currentResolution.refreshRateRatio;
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRateRatio.numerator == currentRefreshRate.numerator &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        //Adds the resolution options to the dropdown
        resolutionDropdown.AddOptions(options);

        //Sets the dropdown value to the current screen resolution index
        resolutionDropdown.value = currentResolutionIndex;

        // Refresh the displayed value of the dropdown
        resolutionDropdown.RefreshShownValue();
    }

    // Update method runs once per frame.
    void Update()
    {
        // when the escape key is pressed it will run the resume method
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Method to resume the game
    public void Resume()
    {
        //hides the pausemenu
        pauseMenuUI.SetActive(false);
        //locks and hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //unpauses the game
        GameIsPaused = false;
    }

    //Method to pause the game
    void Pause()
    {
        // Shows the pause menu
        pauseMenuUI.SetActive(true);
        // Unlock and show the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Pauses the game
        GameIsPaused = true;
    }

    //Method to toggle windowed/fullscreen mode
    public void SetWindowedMode(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
    }

    //Will quit the game when the button is pressed
    public void QuitGame()
    {
        Application.Quit();
    }
}
