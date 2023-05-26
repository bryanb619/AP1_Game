using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class StartMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField]    private GameObject menu, optionsMenu, startText;
    [SerializeField]    private string loadMenu;
    
    [Header("Sounds")]
    [SerializeField]    private EventReference hover, click, music;

                        // Debug Color
                        private string _debugColor = "<size=14><color=green>";
                        private string _closeColor = "</color></size>";
                        
                        private bool _run;


    private void Awake()
    {
        Screen.fullScreen = true;
    }

    void Start()
    {
        // Cursor Lock state
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        startText.SetActive(true);
        
        menu.SetActive(false);
        optionsMenu.SetActive(false);
    }


    private void Update()
    {
        
        if (startText && !_run)
        {
            if (Input.anyKeyDown )
            {
                _run = true; 
                startText.SetActive(false);
                
                menu.SetActive(true);

               
            } 
        }
        
    }


    #region Start Menu

 
    // Game Load Button
    public void LoadGame()
    {
        SceneManager.LoadScene(loadMenu);
        Debug.Log(_debugColor + "Game Loading!" + _closeColor);

    }
    public void Options()
    {
        menu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Game Exit Button
    public void ExitGame()
    {

        Application.Quit();
        Debug.Log(_debugColor + "Game quit!" + _closeColor);
    }



    // Sounds ------------------------------------------------------------>
    public void HoverButton()
    {
        //PlayOneShot
        RuntimeManager.PlayOneShot(hover);
        
        
    }

    public void ClickButton() 
    {
        RuntimeManager.PlayOneShot(click);

    }
    #endregion

    #region Options Menu

    // Back to main Menu Button
    public void BackButton()
    {
        optionsMenu.SetActive(false);
        menu.SetActive(true);
    }

    #endregion



}
