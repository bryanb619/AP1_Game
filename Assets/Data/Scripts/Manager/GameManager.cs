using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    //private
    [SerializeField]
    private StudioEventEmitter[] studioEventEmitters;

    private bool _audioState; 

    // Testing New Game Manager
    public static GameManager Instance;

    //[HideInInspector]
    public GameState State;


    public static event Action<GameState> OnGameStateChanged;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Gameplay);
    }
    public void UpdateGameState(GameState newstate)
    {
        State = newstate;

        switch (newstate)
        {
            case GameState.Gameplay:
                {
                    // set cursor settings to play mode
                    Cursor.visible = false; 
                    Cursor.lockState = CursorLockMode.Confined;

                    Time.timeScale = 1f;
                    _audioState= true;
                    break; 
                }
            case GameState.Paused:
                {

                    // set cursor to pause mode
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    _audioState = false;
                    Time.timeScale = 0f;
                    break;
                }
            default: 
                {
                    print("Invalid/State not added to Game Manager"); 
                    throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
                }


        }

        //AudioState();
        OnGameStateChanged?.Invoke(newstate);
        
    }

    private void HandleNewState()
    {
        


    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("_StartMenu");
    }


    public void AudioState()
    {
        foreach (var emitter in studioEventEmitters)
        {
            var instance = emitter.EventInstance;
            instance.setPaused(_audioState);
        }
    }


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






