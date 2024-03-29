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

    private void Start()
    {
        // Fetch available resolutions
        Resolution[] resolutions = Screen.resolutions;

        // Clear existing options
        resolutionDropdown.ClearOptions();

        // Create a list to store resolution options
        List<string> options = new List<string>();

        // Find the current resolution index
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height} @{resolutions[i].refreshRate}Hz";
            options.Add(option);

            // Check if this resolution matches the current screen resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        // Add resolution options to the dropdown
        resolutionDropdown.AddOptions(options);

        // Set the value of the dropdown to the current resolution index
        resolutionDropdown.value = currentResolutionIndex;

        // Refresh the shown value of the dropdown
        resolutionDropdown.RefreshShownValue();
    }

    // Method to set the resolution
    public void SetResolution(int resolutionIndex)
    {
        // Fetch the selected resolution from the resolutions array
        Resolution resolution = Screen.resolutions[resolutionIndex];

        // Apply the selected resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Update is called once per frame
    void Update()
    {
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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }
    public void SetWindowedMode(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
