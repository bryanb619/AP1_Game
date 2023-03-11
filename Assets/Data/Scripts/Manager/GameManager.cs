using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
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

    // Testing New Game Manager
    public static GameManager                   Instance;

    //[HideInInspector]
    public GameState                            State;

    public static event Action<GameState>       OnGameStateChanged;


    public BattleStates                         _battleState;

    public static event Action<BattleStates>    OnBattleStateChanged;


    public class SFXSound : UnityEvent<float> {}

    public static SFXSound OnSFXValueChange = new SFXSound();


    //[SerializeField] private Slider _ambientSlider;


    #endregion

    #region Awake & Start
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        UpdateGameState(GameState.Gameplay);

       // HandleAmbientVolume(_slider.value);

        //_slider.onValueChanged.AddListener(HandleAmbientVolume);
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

        //Time.timeScale = 1f;
        _audioState = false;

        HandleEventEmitterState();
    }


    private void HandlePaused()
    {
        // set cursor to pause mode
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _audioState = true;
        
        //Time.timeScale = 0f;

        HandleEventEmitterState();

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
        
        print("ambient " + ambientVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in ambientSounds)
        {
            emitter.SetParameter("Volume", ambientVolume);
        }
    }

    public void HandleSFXVolume(float newVolume)
    {
        sfxVolume = newVolume;

        print("ambient " + sfxVolume);

        foreach (FMODUnity.StudioEventEmitter emitter in sfxSounds)
        {
            emitter.SetParameter("Volume", sfxVolume);
        }
    }
    #endregion

    #endregion
    /*
    public void AudioState()
    {
        foreach (var emitter in studioEventEmitters)
        {
            var instance = emitter.EventInstance;
            instance.setPaused(_audioState);
        }
    }
    */

    #endregion

    #region Battle states
    public void FightState(BattleStates newBattleState)
    {
        _battleState = newBattleState;

        switch(newBattleState)
        {
            case BattleStates._NONE:
                {
                    print("No Battle active");
                    break;
                }

            case BattleStates._PLAYERTURM: 
                {
                    HandlePlayerTurn();
                    break;
                }
            case BattleStates._ENEMYTURN: 
                {
                    HandleEnemyTurn();
                    break;
                }
            case BattleStates._VICTORY:
                {
                    HandleVictory();
                    break;
                }
             case BattleStates._DEFEAT: 
                {
                    HandleDefeat();
                    break;
                }
            default:{throw new ArgumentOutOfRangeException(nameof(newBattleState), newBattleState, null);}
        }

        OnBattleStateChanged?.Invoke(newBattleState);
    }


    private void HandlePlayerTurn()
    {

    }

    private void HandleEnemyTurn()
    {
       
    }

    private void HandleVictory()
    {

    }

    private void HandleDefeat() 
    {

    }

    #endregion

    /*
    public void Pause(bool state)
    {
        foreach (var emitter in studioEventEmitters)
        {
            var instance = emitter.EventInstance;
            instance.setPaused(state);
        }
    }
    */

    /*
    public void ExitGame()
    {
        Application.Quit();
    }
    */


}