using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using TMPro;
public class OptionMenu : MonoBehaviour
{
    #region Variables
    //[SerializeField] private Audio audio ;
   
    [SerializeField]    private Dropdown             resDropdown;
                        private Resolution[]        _resolutions;

    [SerializeField]    private GameObject            warningBox; 
                        
    
    [SerializeField]    private Slider musicSlider;
    //[SerializeField] private StudioEventEmitter[] eventEmitters;  

    private MusicManager _musicManager; 

    #endregion

    #region Start
    private void Start()
    {
        ResolutionSetup();

        
        //musicSlider.onValueChanged.AddListener(MusicVolume);

        //_musicManager = FindObjectOfType<MusicManager>(); 

    }
    #endregion

    #region Resolution & Quality 

    #region Resoltion 
    private void ResolutionSetup()
    {
        // get resolution
        _resolutions = Screen.resolutions; 

        // clear
        resDropdown.ClearOptions(); 

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
        resDropdown.AddOptions(resOptions);
        resDropdown.value = currentResolutionIndex;
        resDropdown.RefreshShownValue();
    }

    #endregion
    public void SetVolume(float volume)
    {
        //audio.SetFloat("Master", volume);
    }

    #region Quality
    
    public void Quality(int qualityIndex)
    {
        // Set Game graphical quality 
        QualitySettings.SetQualityLevel(qualityIndex);
        
          
#if UNITY_EDITOR
        Debug.Log("Current Quality: " + QualitySettings.GetQualityLevel());
#endif
        
    }
    #endregion 
    public void SetFullscreen(bool isFullscreen)
    {
        
        Screen.fullScreen = isFullscreen;
        
#if UNITY_EDITOR
        Debug.Log("Fullscreen: " + isFullscreen);
#endif
        
    }
    
    public void OpenWarningRes()
    {
        warningBox.SetActive(true);
        
    #if UNITY_EDITOR
        print("run");
    #endif
    }


    public void SetNewRes(bool change)
    {
        if (change)
        {
            //warningBox.SetActive(true);
            
            int resIndex = resDropdown.value;
            Resolution resolution = _resolutions[resIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            warningBox.SetActive(false);
            
#if UNITY_EDITOR
            Debug.Log("Resolution: " + resolution.width + " x " + resolution.height);
#endif
        }
        
        else
        {
            warningBox.SetActive(false);
        }
        
    }
    
    private  void ChangeResolution(int index)
    {
        Resolution resolution = _resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        

    }

    #endregion
    
    private void SetNewControlSelection()
    {
        
    }

    #region Buttons
    public void BackToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
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
