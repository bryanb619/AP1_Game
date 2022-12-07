using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    // game paused bool
    [HideInInspector]
    public bool _Paused = false;

    //  Pause Menu Canvas
    [SerializeField]
    public GameObject PauseCanvas, _pauseMenu, _OptionsMenu, GamePlay;

    // Event Sy




    private void Start()
    {
        GamePlay = GameObject.Find("--- GamePlay ---");

        PauseCanvas.SetActive(true);
        _pauseMenu.SetActive(false);
        _OptionsMenu.SetActive(false);
        //GamePlay.SetActive(true);

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
        GamePlay.SetActive(true);

        Time.timeScale = 1f;

        _Paused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }
    // method pause
    public void Pause()
    {


        _pauseMenu.SetActive(true);
        GamePlay.SetActive(false);

        Time.timeScale = 0f;

        _Paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
       
    }

    // buttons 

    // Pause Menu
    public void ResumeButton()
    {
        Resume();

    }

    // OPTIONS
    public void OptionsButton()
    {

        _OptionsMenu.SetActive(true);
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

    public void BackButton()
    {
        _pauseMenu.SetActive(true);
        _OptionsMenu.SetActive(false);

    }



}
