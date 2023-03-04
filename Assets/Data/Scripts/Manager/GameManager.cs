using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    #region Variables
    //private
    [SerializeField]
    private StudioEventEmitter[]                studioEventEmitters;

    private bool                                _audioState; 

    // Testing New Game Manager
    public static GameManager                   Instance;

    //[HideInInspector]
    public GameState                            State;

    public static event Action<GameState>       OnGameStateChanged;


    public BattleStates                         _battleState;

    public static event Action<BattleStates>    OnBattleStateChanged;

    #endregion

    #region Awake & Start
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        UpdateGameState(GameState.Gameplay);

        Cursor.visible = false;
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        Time.timeScale = 1f;
        _audioState = true;
        
    }

    private void HandlePaused()
    {
        // set cursor to pause mode
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _audioState = false;
        Time.timeScale = 0f;

        StopAllCoroutines();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void AudioState()
    {
        foreach (var emitter in studioEventEmitters)
        {
            var instance = emitter.EventInstance;
            instance.setPaused(_audioState);
        }
    }

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