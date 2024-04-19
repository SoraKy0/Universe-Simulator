using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Declare GameIsPaused as a public static boolean to keep track of the game's pause state.
    public static bool GameIsPaused = false;

    // Declare a GameObject for the pause menu UI.
    public GameObject pauseMenuUI;

    // Declare a TMP_Dropdown for the resolution dropdown menu.
    public TMP_Dropdown resolutionDropdown;

    // Start method runs at the beginning.
    void Start()
    {
        // Fetch available resolutions.
        Resolution[] resolutions = Screen.resolutions;

        // Clear existing options in the resolution dropdown.
        resolutionDropdown.ClearOptions();

        // Create a list to store resolution options.
        List<string> options = new List<string>();

        // Find the current resolution index.
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height} @{resolutions[i].refreshRate}Hz";
            options.Add(option);

            // Check if this resolution matches the current screen resolution.
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        // Add resolution options to the dropdown.
        resolutionDropdown.AddOptions(options);

        // Set the value of the dropdown to the current resolution index.
        resolutionDropdown.value = currentResolutionIndex;

        // Refresh the displayed value of the dropdown.
        resolutionDropdown.RefreshShownValue();
    }

    // Update method runs once per frame.
    void Update()
    {
        // Toggle pause when the Escape key is pressed.
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

    // Method to resume the game.
    public void Resume()
    {
        // Hide the pause menu.
        pauseMenuUI.SetActive(false);
        // Lock and hide the mouse cursor.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Unpause the game.
        GameIsPaused = false;
    }

    // Method to pause the game.
    void Pause()
    {
        // Show the pause menu.
        pauseMenuUI.SetActive(true);
        // Unlock and show the mouse cursor.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Pause the game.
        GameIsPaused = true;
    }

    // Method to set windowed mode.
    public void SetWindowedMode(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
    }

    // Method to quit the game.
    public void QuitGame()
    {
        Application.Quit();
    }
}
