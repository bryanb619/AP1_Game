using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    #region Variables
    
    [SerializeField]    private TMP_Dropdown        resDropdown;
                        private Resolution[]        _resolutions;

    [SerializeField]    private GameObject          warningBox; 
                        
    [SerializeField]    private TMP_Dropdown        qualityDropdown;
                        //private string[]            _qualityOptions;
    
    [SerializeField]    private Slider               musicSlider;
    
    
    // Managers
    
                        // Music Manager
                        private MusicManager        _musicManager;
                        
                        // Player control
                        private PlayerControl           _playerControl;
                        
                        // Graphic Manager
                        private GraphicManager         _graphicManager;
    
    #endregion

    #region Data Loading

    private void Awake()
    {
        // Find scripts
        GetManagers(); 
        
        // Check Saved Data
        DataCheck();
    }

    private void GetManagers()
    {
        // TODO - Find a better way to find scripts
        _musicManager   = FindObjectOfType<MusicManager>();
        _graphicManager = FindObjectOfType<GraphicManager>();
        _playerControl  = FindObjectOfType<PlayerControl>();
    }
    
    
    #region DataCheck

    private void DataCheck()
    {
        
        QualityCheck(); // Quality 
        ResCheck(); // Resolution
        VolumeCheck(); // Volume
    }


    #endregion
    
    
    #region Checks
    private void ResCheck()
    {
        if (!PlayerPrefs.HasKey("Resolution"))
        {
            PlayerPrefs.SetInt("Resolution", 20);
            //resDropdown.value = PlayerPrefs.GetInt("Resolution");
            _graphicManager.ResolutionSetup();

#if UNITY_EDITOR
            
            Debug.Log("Set Current Resolution: " + resDropdown.value);
#endif
            
        }
        else
        {
            //ResolutionSetup();
            
            resDropdown.value = PlayerPrefs.GetInt("Resolution");
            _graphicManager.ResolutionSetup();

#if UNITY_EDITOR
                
                Debug.Log("No Saved data, Current Resolution is: 1920x1080");
#endif
            
        }
        
    }
    
    private void QualityCheck()
    {
        if(!PlayerPrefs.HasKey("Quality"))
        {
            PlayerPrefs.SetInt("Quality", 3);

#if UNITY_EDITOR
            Debug.Log("Set Current Quality: " + QualitySettings.GetQualityLevel());
#endif
        }
        else
        {
            // Set Game graphical quality
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

    #region UI 
    
    #region Buttons
    
    // Resolution Dropdown
    public void ResDropDown()
    {
        
    }

    #region Quality
    
    /// <summary>
    /// Quality Dropdown
    /// </summary>
    /// <param name="qualityLevel"></param>
    public void QualitySetup(int qualityLevel)
    {
        // Refresh Dropdown show value
        qualityDropdown.RefreshShownValue();
        
        // Set Game graphical quality
        _graphicManager.SetQualityLevel(qualityLevel);
        
        // Save Quality
        PlayerPrefs.SetInt("Quality", qualityLevel);
    }

    #endregion

    #region Fullscreen
    
    /// <summary>
    /// Full Screen Toggle
    /// </summary>
    /// <param name="isFullscreen"></param>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
#if UNITY_EDITOR

       // Debug.Log("Fullscreen: " + isFullscreen);
#endif
        
    }

    #endregion
    
    // Player Dropdown (MOVEMENT)

    public void PlayerMovement(int movementIndex)
    {
        // Set Player Movement
       // _playerControl.SetMovement(movementIndex);

        // Save Player Movement
        //PlayerPrefs.SetInt("Movement", movementIndex);
        
#if UNITY_EDITOR
        Debug.Log("Player Movement: " + movementIndex);
#endif
    }

    #region Clear Saved Options

    public void ClearOptions()
    {
        PlayerPrefs.DeleteAll();

#if UNITY_EDITOR

        Debug.Log("Player Prefs Cleared");
        
#endif
    }

    #endregion
    

    public void BackToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
    }
    // sound Slider
    
    public void SetVolume(float volume)
    {
        //audio.SetFloat("Master", volume);
        musicSlider.value = volume;
    }
    
    #endregion
    
    #region Sliders

    private void MusicVolume(float newVolume)
    {
        print(newVolume);
        _musicManager.MusicVolume(newVolume);
    }
    #endregion
    
    
    #endregion
    
    private void OpenWarningRes()
    {
        warningBox.SetActive(true);
        
        
#if UNITY_EDITOR
        print("runing warning box");
#endif
    }
    
    #region Controls

    private void ControlSetCheck()
    {
        
        
    }
    
    
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

    #endregion

    

   
}
