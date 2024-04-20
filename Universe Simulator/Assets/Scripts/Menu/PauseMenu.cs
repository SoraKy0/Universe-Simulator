using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public TMP_Dropdown resolutionDropdown;

    // Start is called before the first frame update.
    void Start()
    {
        Resolution[] resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        // Create a list to store resolution options.
        List<string> options = new List<string>();

        // Find the current resolution index.
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Use refreshRateRatio instead of refreshRate.
            RefreshRate refreshRate = resolutions[i].refreshRateRatio;
            string option = $"{resolutions[i].width} x {resolutions[i].height} @{refreshRate.numerator}/{refreshRate.denominator}Hz";
            options.Add(option);

            // Check if this resolution matches the current screen resolution.
            RefreshRate currentRefreshRate = Screen.currentResolution.refreshRateRatio;
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRateRatio.numerator == currentRefreshRate.numerator &&
                resolutions[i].refreshRateRatio.denominator == currentRefreshRate.denominator)
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

    // Method to toggle windowed/fullscreen mode.
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
