using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class OptionMenu : MonoBehaviour
{
    #region Variables
    //[SerializeField] private Audio audio ;
    [SerializeField] private Dropdown _resDropdown;
    Resolution[] _resolutions;

    [SerializeField] private TMP_Dropdown _resDropdown2;

    #endregion

    #region Start
    private void Start()
    {
        ResolutionSetup();
        Screen.fullScreen = true;
    }
    #endregion

    #region Resolution & Quality 

    #region Resoltion 
    private void ResolutionSetup()
    {
        // get resolution
        _resolutions = Screen.resolutions; 

        // clear
        _resDropdown2.ClearOptions(); 

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
        _resDropdown2.AddOptions(resOptions);
        _resDropdown2.value = currentResolutionIndex;
        _resDropdown2.RefreshShownValue();
    }

    #endregion
    public void SetVolume(float volume)
    {
        //audio.SetFloat("Master", volume);
    }

    #region Quality
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    #endregion

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

    #region Buttons
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    #endregion
}
