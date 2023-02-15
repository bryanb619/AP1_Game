using UnityEngine;

public class PauseMenu : MonoBehaviour
{ 
    private GameManager     _manager; 
 
    // game paused bool
    private bool            _Paused;

    //  Pause Menu Canvas
    [SerializeField]
    private GameObject      PauseCanvas, _pauseMenu, _OptionsMenu;


    private void Start()
    {
        _manager = FindObjectOfType<GameManager>();


        PauseCanvas.SetActive(true);
        _pauseMenu.SetActive(false);
        _OptionsMenu.SetActive(false);
       

    }

    // Update is called once per frame
    void Update()
    {

        KeyDetect();

    }
    
    private void KeyDetect()
    {
        if (Input.GetButtonDown("Pause"))
        {
            
            if (_Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
            
        }
    }

    // method Resume
    private void Resume()
    {
    
        _pauseMenu.SetActive(false);
        _Paused = false;

        _manager.UpdateGameState(GameState.Gameplay); 

    }
    // method pause
    private void Pause()
    {
        _pauseMenu.SetActive(true);
        _Paused = true;

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

        _OptionsMenu.SetActive(true);
        _pauseMenu.SetActive(false);
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
        _pauseMenu.SetActive(true);
        _OptionsMenu.SetActive(false);

    }


}




