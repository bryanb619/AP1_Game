using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject _PauseMenu, _OptionsMenu;

    private bool _isPaused;

    // Start is called before the first frame update
    void Start()
    {
        _PauseMenu.SetActive(false);
        _OptionsMenu.SetActive(false);

        _isPaused = false;
        
    }


    private void Update()
    {
        DetectPlayerInput();
    }

    private void DetectPlayerInput()
    {

        
        if(Input.GetKeyDown(KeyCode.Escape) && _isPaused)
        {
            Resume();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && _isPaused == false)
        {
            Pause();    
        }

    /*
        if(Input.GetButtonDown(KeyCode.Escape) && _isPaused) 
        {
            Resume();
        }
        else if (Input.GetButtonDown("Pause") && _isPaused == false)
        {
            Pause();
        }
    */

    }


    #region Pause Menu
    public void Resume()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        _PauseMenu.SetActive(true);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _isPaused = true;
        _PauseMenu?.SetActive(false);

    }

    public void OptionsMenu()
    {
        _PauseMenu.SetActive(false);
        _OptionsMenu.SetActive(true);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void GameExit()
    {
        Application.Quit();
    }
    #endregion


    #region Options Menu

    public void BackButton()
    {
        _OptionsMenu.SetActive(false);
        _PauseMenu?.SetActive(true);
    }
    #endregion


}
