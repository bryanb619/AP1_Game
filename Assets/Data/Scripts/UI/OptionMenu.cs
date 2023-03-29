using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    //[SerializeField] private Audio audio ;
    [SerializeField] private Dropdown _resDropdown;
    Resolution[] _resolutions;

    private void Start()
    {
        ResolutionSetup();
        Screen.fullScreen = true;
    }

    #region Resolution & Quality 
    private void ResolutionSetup()
    {
        // get resolution
        _resolutions = Screen.resolutions; 

        // clear
        _resDropdown.ClearOptions(); 

        // create list of resolution
        List<string> resOptions = new List<string>();  

        int currentResolutionIndex = 0; 

        for (int x = 0; x < _resolutions.Length; x++)
        {
            string option = _resolutions[x].width + " x " + _resolutions[x].height;

            resOptions.Add(option);

            if (_resolutions[x].width == Screen.currentResolution.width && _resolutions[x].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = x;
            }
        }
        _resDropdown.AddOptions(resOptions);
        _resDropdown.value = currentResolutionIndex;
        _resDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        //audio.SetFloat("Master", volume);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = _resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    #endregion

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
