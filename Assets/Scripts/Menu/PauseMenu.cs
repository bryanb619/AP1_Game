using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    // game paused bool
    [HideInInspector]
    public bool _Paused = false;

    //  Pause Menu Canvas
    [SerializeField]
    public GameObject PauseCanvas, _pauseMenu, _OtptionsMenu;

    // Event Sy




    private void Start()
    {
        PauseCanvas.SetActive(true);
        _pauseMenu.SetActive(false);
        _OtptionsMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        KeyDetect();

    }
    
    void FixedUpdate()
    {
        if (!_Paused)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (_Paused == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

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
    public void Resume()
    {

        _pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        _Paused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        //Cursor.lockState = CursorLockMode.Locked;



    }
    // method pause
    public void Pause()
    {


        _pauseMenu.SetActive(true);

        Time.timeScale = 0f;

        _Paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // buttons 

    // Pause Menu
    public void ResumeButton()
    {

        _pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        _Paused = false;

    }

    // OPTIONS
    public void OptionsButton()
    {

        _OtptionsMenu.SetActive(true);
        _pauseMenu.SetActive(false);
    }


    // EXIT Tom Main Menu
    public void ExitButton()
    {
        //Application.Quit();
        Debug.Log("Star Menu Loaded");
        SceneManager.LoadScene("_StartMenu");
    }


    public void GameQuit()
    {
        Application.Quit();
    }

    // Options Menu

    // BACK BUTTON

    public void BakcButton()
    {
        _pauseMenu.SetActive(true);
        _OtptionsMenu.SetActive(false);

    }



}
