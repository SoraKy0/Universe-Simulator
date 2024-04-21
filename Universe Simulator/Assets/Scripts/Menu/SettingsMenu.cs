using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer; //references the audiomixer in unity that control the volume of the game
    public TMP_Dropdown resolutionsDropdown; //reference to the dropdown UI that has the differenct resolutions

    private void Start()
    {
        //Gets the resolutions of the screen
        Resolution[] resolutions = Screen.resolutions;

        //clears any existing resolutions from the dropdown
        resolutionsDropdown.ClearOptions();

        //list will store all the resolution options for the monitor
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            //this is the way the resolutions with the width height and refresh rate will be added to the list
            string option = $"{resolutions[i].width} x {resolutions[i].height} -{resolutions[i].refreshRate}Hz";
            options.Add(option);

            //this checks is the monitor resolution is the same as the game resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        //put the resolutions into the dropdown in the 
        resolutionsDropdown.AddOptions(options);
         
        // Set the value of the dropdown to the current resolution
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    //sets the screens resolution to the selected resolution from the dropdown
    public void SetResolution(int resolutionIndex)
    {
        //applies the resolution to game
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //uses the audio mixer thing to change the volume
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    //Method to toggle windowed/fullscreen mode
    public void SetWindowedMode(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
    }
}
