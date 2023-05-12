using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;
using UnityEngine.Serialization;

public class OptionMenu : MonoBehaviour
{
    #region Variables
    //[SerializeField] private Audio audio ;
    [FormerlySerializedAs("_resDropdown")] [SerializeField] private Dropdown resDropdown;
    Resolution[] _resolutions;

    [FormerlySerializedAs("_resDropdown2")] [SerializeField] private TMP_Dropdown resDropdown2;

    [SerializeField] private Slider musicSlider;
    //[SerializeField] private StudioEventEmitter[] eventEmitters;  

    private MusicManager _musicManager; 

    #endregion

    #region Start
    private void Start()
    {
        ResolutionSetup();
        Screen.fullScreen = true;
        
        musicSlider.onValueChanged.AddListener(MusicVolume);

        _musicManager = FindObjectOfType<MusicManager>(); 

    }
    #endregion

    #region Resolution & Quality 

    #region Resoltion 
    private void ResolutionSetup()
    {
        // get resolution
        _resolutions = Screen.resolutions; 

        // clear
        resDropdown2.ClearOptions(); 

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
        resDropdown2.AddOptions(resOptions);
        resDropdown2.value = currentResolutionIndex;
        resDropdown2.RefreshShownValue();
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

    #region Sliders

    private void MusicVolume(float newVolume)
    {
        print(newVolume);
        _musicManager.MusicVolume(newVolume);
        
    }
    
    #endregion
}
