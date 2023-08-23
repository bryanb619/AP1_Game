using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.Serialization;



public class GameManager : MonoBehaviour
{
    #region Variables

    // GAME STATE   ------------------------------------------------------------------------------------------->
    [FormerlySerializedAs("State")] [Header("Game state")]

                        public GameState                        state;
                        //private GameState                       _state;
                        public static GameManager               Instance;

                        public static event Action<GameState>   OnGameStateChanged;

    [SerializeField]    private GameObject outOfFocus, pauseMenu;

    
    // Build Platform ----------------------------------------------------------------------------------------->

    public BuildUnit platform;
    
    
    

    // Scenes       ------------------------------------------------------------------------------------------->

    [SerializeField]    private string                          startMenu;
    [SerializeField]    private string                          restartMenu; 
    [SerializeField]    private string                          endMenu;

    [SerializeField]    private float                           restartTime; 


    // GAME SOUND   ------------------------------------------------------------------------------------------->
    [SerializeField]    private StudioEventEmitter[]            ambientSounds;

    [SerializeField]    private StudioEventEmitter[]            sfxSounds;

    [SerializeField]    private StudioEventEmitter[]            musicSounds;

    //[SerializeField]
                        // VOLUME
                        private float                           _ambientVolume;
                        private float                           _sfxVolume;
                        private float                           _musicVolume;

                        // AUDIO STATE
                        private bool                            _audioState;
                        
                        

                 

    //public class SFXSound : UnityEvent<float> {}
    //public static SFXSound OnSFXValueChange = new SFXSound();
    //[SerializeField] private Slider _ambientSlider;
    
    #endregion

    #region Awake & Start
    private void Awake()
    {
        //DontDestroyOnLoad(this);

        Instance = this;

        GameManager[] otherManagers = FindObjectsOfType<GameManager>();

        for (int i = 1; i < otherManagers.Length; i++)
        {
            Destroy(otherManagers[i].gameObject);
        }
        
        // Check game Build
        
        
        switch (platform)
        {
            // IF CONSOLE
            case BuildUnit.Console:
            {
                // Apply console settings  
                
                
#if UNITY_EDITOR

            // DEV ------------------------------------------------------------------------------------------>
            const string debugColor = "<size=14><color=red>";
            const string closeColor = "</color></size>";
            //Debug.Log(debugColor + "Close attack" + closeColor);
#endif
                
                break;
            }
            // ELSE IF PC
            case BuildUnit.PC:
            {
                
                // Apply console settings  
                
                
#if UNITY_EDITOR

            // DEV ------------------------------------------------------------------------------------------>
            const string debugColor = "<size=14><color=red>";
            const string closeColor = "</color></size>";
            //Debug.Log(debugColor + "Close attack" + closeColor);
#endif
                
                break;
            }
            
            default:
            {
                Debug.LogError("No Build set");
                break;
            }
            
        }
        
         

    }

    private void Start()
    {
        UpdateGameState(GameState.Gameplay);
        //outOfFocus.SetActive(false);
        
    }
    #endregion

    #region Game State

    public void UpdateGameState(GameState newGamestate)
    {
        state = newGamestate;

        switch (newGamestate)
        {
            case GameState.Gameplay:
                {
                    HandleGameplay(); 
                    break;
                }
            case GameState.Paused:
                {
                    HandlePaused(); 
                    break;
                }
            case GameState.Unfocussed:
                {
                    print("UNFOCUSSED");
                    HandlePaused();
                    break; 
                }
            case GameState.Death:
            {
                HandleDeath();
                break; 
            }

            default: { throw new ArgumentOutOfRangeException(nameof(newGamestate), newGamestate, null); }
        }
        OnGameStateChanged?.Invoke(newGamestate);
    }


    private void HandleGameplay()
    {
        // set cursor settings to play mode
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        Time.timeScale = 1f;
        _audioState = false;

        //_state = GameState.Gameplay;
        HandleEventEmitterState();
    }


    private void HandleUnfocussedApp(bool focused)
    {
        //_state = GameState.Paused;
        switch(focused)
        {
            case true:
                {
                    outOfFocus.SetActive(false);
                    UpdateGameState(GameState.Gameplay);
                    break; 
                }
            case false:
                {
                    outOfFocus.SetActive(true);
                    UpdateGameState(GameState.Paused);
                    break; 
                }
        }
    }


    private void HandlePaused()
    {
        // set cursor to pause mode
        Cursor.visible      = true;
        Cursor.lockState    = CursorLockMode.None;

        _audioState         = true;

        Time.timeScale      = 0f;
        //_state = GameState.Paused;

        HandleEventEmitterState();

    }

    private void HandleMenus()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        Time.timeScale = 1f;

    }

    private void HandleDeath()
    {
        // set cursor to pause mode
        Cursor.visible      = true;
        Cursor.lockState    = CursorLockMode.None;

        StartCoroutine(TimerToRestart()); 

    }

    private IEnumerator TimerToRestart()
    {
        bool isRunning = false;

        if (!isRunning)
        {
            isRunning = true; 
            
            yield return new WaitForSeconds(restartTime);
            SceneManager.LoadScene(restartMenu);

            isRunning = false; 

        }
           
    }


    private void CheckAppFocus()
    {

        if (!Application.isFocused)
        {
          
            //HandleUnfocussedApp(_inApp);
            return;
        }
        else 
        {
        
        }
    }

    public void RestartGame()
    {
        

    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(startMenu);
    }


    public void ApplyBuildPlatform(BuildUnit unit)
    {
        switch (unit)
        {
            case BuildUnit.Console:
            {
                
#if UNITY_EDITOR
                Debug.Log("Console");
#endif
                break;
            }
            case BuildUnit.PC:
            {
                
#if UNITY_EDITOR
                Debug.Log("PC");
#endif
                break;
            }
            
        }
    }

    #region FMOD Sounds

    /// <summary>
    /// Handle of FMOD emitters sound states such play, pause and volume
    /// </summary>
    /// 
    private void HandleEventEmitterState()
    {
        foreach (FMODUnity.StudioEventEmitter emitter in ambientSounds)
        {
            // Set according to bool value
            emitter.EventInstance.setPaused(_audioState);
        }
        foreach (FMODUnity.StudioEventEmitter emitter in musicSounds)
        {
            // Set according to bool value
            emitter.EventInstance.setPaused(_audioState);
        }
        return;
    }
    #region Volume change

    public void HandleMusicVolume(float newVolume)
    {
        _musicVolume = newVolume;

        print("ambient " + _ambientVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in musicSounds)
        {
            // 
            emitter.SetParameter("Volume", _musicVolume);

        }
    }

    public void HandleAmbientVolume(float newVolume)
    {
        _ambientVolume = newVolume;
        
        //print("ambient " + ambientVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in ambientSounds)
        {
            emitter.SetParameter("Volume", _ambientVolume);
        }
    }

    public void HandleSfxVolume(float newVolume)
    {
        _sfxVolume = newVolume;

        //print("ambient " + sfxVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in sfxSounds)
        {
            //emitter.SetParameter("Volume", sfxVolume);
        }
    }
    #endregion

    #endregion
    #endregion
    
    #region Builds

    private void ConsoleBuild()
    {
        // Disable NAVMESH 
        
    }

    private void PCBuild()
    {
        // LOAD SET INPUT
        
        
        // Apply input config
        
    }

    #endregion
}