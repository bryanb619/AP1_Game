using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.Events;
//using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;

    //[HideInInspector]
    [Header("Game state")]
    public GameState State;

    //private
    [SerializeField]
    private StudioEventEmitter[]               ambientSounds;

    [SerializeField]
    private StudioEventEmitter[]               sfxSounds;

    [SerializeField]
    private StudioEventEmitter[]               musicSounds;

    //[SerializeField]
    private float                               ambientVolume;
    private float                               sfxVolume;
    private float                               musicVolume;

    private bool                                _audioState; 


    //public class SFXSound : UnityEvent<float> {}

    //public static SFXSound OnSFXValueChange = new SFXSound();


    //[SerializeField] private Slider _ambientSlider;


    #endregion

    #region Awake & Start
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        
    }

    private void Start()
    {
        UpdateGameState(GameState.Gameplay);
    }
    #endregion

    #region Game State

    public void UpdateGameState(GameState newGamestate)
    {
        State = newGamestate;

        switch (newGamestate)
        {
            case GameState.Gameplay:
                {
                    HandleGameplay(); // set Gameplay
                    break; 
                }
            case GameState.Paused:
                {
                    HandlePaused(); // set paused
                    break;
                }
            default: {throw new ArgumentOutOfRangeException(nameof(newGamestate), newGamestate, null);}
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

        HandleEventEmitterState();
    }


    private void HandlePaused()
    {
        // set cursor to pause mode
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _audioState = true;
        
        Time.timeScale = 0f;

        HandleEventEmitterState();

    }

    private void HandleMenus()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        Time.timeScale = 1f;
    
    }


    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
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
    }
    #region Volume change

    public void HandleMusicVolume(float newVolume)
    {
        musicVolume = newVolume;

        print("ambient " + ambientVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in musicSounds)
        {
            // 
            emitter.SetParameter("Volume", musicVolume);

        }
    }

    public void HandleAmbientVolume(float newVolume)
    {
        ambientVolume = newVolume;
        
        //print("ambient " + ambientVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in ambientSounds)
        {
            emitter.SetParameter("Volume", ambientVolume);
        }
    }

    public void HandleSFXVolume(float newVolume)
    {
        sfxVolume = newVolume;

        //print("ambient " + sfxVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in sfxSounds)
        {
            emitter.SetParameter("Volume", sfxVolume);
        }
    }
    #endregion

    #endregion
    #endregion

}