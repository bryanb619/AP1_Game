using System;
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
   
    [SerializeField]    private TMP_Dropdown        resDropdown;
                        private Resolution[]        _resolutions;

    [SerializeField]    private GameObject          warningBox; 
                        
    [SerializeField]    private TMP_Dropdown        qualityDropdown;
                        //private string[]            _qualityOptions;
    
    [SerializeField]    private Slider               musicSlider;
  

                        private MusicManager        _musicManager;

                        private PlayerControl       _playerControl;

    #endregion

    private void Awake()
    {
        DataCheck();
        
        _playerControl = FindObjectOfType<PlayerControl>();
    }
    
    #region DataCheck

    private void DataCheck()
    {
        QualityCheck(); // Quality 
        ResCheck(); // Resolution
        VolumeCheck(); // Volume
    }

    #region Checks
    private void ResCheck()
    {
        ResolutionSetup();
    }
    
    private void QualityCheck()
    {
        if(!PlayerPrefs.HasKey("Quality"))
        {
            PlayerPrefs.SetInt("Quality", 3);
            QualitySetup(PlayerPrefs.GetInt("Quality"));

#if UNITY_EDITOR
            Debug.Log("Set Current Quality: " + QualitySettings.GetQualityLevel());
#endif
        }
        else
        {
            QualitySetup(PlayerPrefs.GetInt("Quality"));
            
#if UNITY_EDITOR
            
            Debug.Log("Current Quality: " + QualitySettings.GetQualityLevel());
#endif
            
        }
        
    }

    private void VolumeCheck()
    {
        
    }
    #endregion
        

    #endregion

    #region Start
    private void Start()
    {
        
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

    #region Quality

    private void QualitySetup(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityDropdown.RefreshShownValue();
        
#if UNITY_EDITOR
        
        Debug.Log("Current Quality: " + QualitySettings.GetQualityLevel());
#endif
    }
    public void Quality(int qualityIndex)
    {
        // Set Game graphical quality 
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        
          
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
    
    public  void ChangeResolution(int index)
    {
        Resolution resolution;
        
        resolution = _resolutions[index];
        
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    #endregion
    
    public void SetVolume(float volume)
    {
        //audio.SetFloat("Master", volume);
    }


    #region Controls

    private void ControlSetCheck()
    {
        
        
    }

    #endregion

    #region Player Input

    public void PlayerInputType(int newInputType)
    {

        PlayerInput newControl; 
        
        switch (newInputType)
        {
            case 0:
            {
#if UNITY_EDITOR
                Debug.Log("mouse");
#endif
                // set control to Mouse
                newControl = PlayerInput.Mouse;
                _playerControl.UpdatePlayerControl(newControl);
                //SetNewControlSelection(newControl);
                break;
            }
            case 1:
            {
                
 #if UNITY_EDITOR
                Debug.Log("Keyboard");
#endif
                // set control to keyboard
                newControl = PlayerInput.Keyboard;
                _playerControl.UpdatePlayerControl(newControl);
                //SetNewControlSelection(newControl);
                break;
            }
        }
        
    }
    
    private void SetNewControlSelection(PlayerInput newControl)
    {

        switch (newControl)
        {
            case PlayerInput.Keyboard:
            {
                // call Player control to update for Keyboard
                _playerControl.UpdatePlayerControl(newControl);
                
                break;
            }
            case PlayerInput.Mouse:
            {
                // Call Player control to update for Mouse
                _playerControl.UpdatePlayerControl(newControl);
                break;
            }

        }
        //newControl.playerInput = (PlayerInput) newInputType;
    }
    
    #endregion

    #region Buttons
    public void BackToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
    }
    
    public void ClearOptions()
    {
        PlayerPrefs.DeleteAll();

#if UNITY_EDITOR
        Debug.Log("Player Prefs Cleared");
        
#endif
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
