using System;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;

public class PauseMenu : MonoBehaviour
{ 
                    private GameManager                 _manager;
                    private GameState                   _gameState; 
 
                    // game paused bool
                    private bool                        _paused;

    //  Pause Menu Canvas
    [SerializeField] private GameObject                 pauseCanvas, pauseMenu, optionsMenu;

    [SerializeField] private EventReference             click; 

    [SerializeField] private Slider                     _sliderAudio;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        _manager                                        = FindObjectOfType<GameManager>();

        pauseCanvas.SetActive(true);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
       

    }

    // Update is called once per frame
    void Update()
    {
        KeyDetect();
    }
    
    private void KeyDetect()
    {
        if (_gameState != GameState.Death)
        {
            if(Input.GetButtonDown("Pause"))
            {
            
                if (_paused && !optionsMenu)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            
            }

            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _manager.UpdateGameState(GameState.Death);
        }
    }

    // method Resume
    private void Resume()
    {
    
        pauseMenu.SetActive(false);
        _paused = false;

        _manager.UpdateGameState(GameState.Gameplay); 

    }
    // method pause
    private void Pause()
    {
        pauseMenu.SetActive(true);
        _paused = true;

        _manager.UpdateGameState(GameState.Paused);
    }
    
    // Pause Menu

    // Resume button
    public void ResumeButton()
    {
        Resume();

    }

    // OPTIONS Button
    public void OptionsButton()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }


    // EXIT Tom Main Menu Button
    public void ExitButton()
    {
        _manager.ExitToMainMenu();
    }

    // Quit Game Button
    public void GameQuit()
    {
        Application.Quit();
    }



    // Options Menu

    // BACK BUTTON

    public void BackButton()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);

    }

    public void ClickSound()
    {
        RuntimeManager.PlayOneShot(click);
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
        {
            case GameState.Gameplay:
            {
                _gameState = GameState.Gameplay;
                break;
            }
            case GameState.Paused:
            {
                _gameState = GameState.Paused;
                break;
            }
            case GameState.Death:
            {
                _gameState = GameState.Death; 
                break; 
            }
           
        }
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}




