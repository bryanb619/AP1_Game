using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField]    private GameObject menu, optionsMenu, startText;
    [SerializeField]    private string loadMenu;
    
    [SerializeField]    private GameObject startButton, optionsButton, exitButton;
    
    [Header("Sounds")]
    [SerializeField]    private EventReference hover, click, music;

                        // Debug Color
                        private string _debugColor = "<size=14><color=green>";
                        private string _closeColor = "</color></size>";
                        
                        private bool _run;
    
    private void Awake()
    {
        
        // DETECT Game control type like if console or PC Build
        /*
#if UNITY_EDITOR
        
        devPopUp.SetActive(true);

        switch (_buildUnit)
        {
            case BuildUnit.PC:
            {
                Debug.Log("PC Build");
                
                break;
            }
            case BuildUnit.Console:
            {
                
                
#if UNITY_EDITOR
                Debug.Log("Console Build");
#endif
                
                break;
                
            }
            default:
            {
                
#if UNITY_EDITOR
                Debug.LogError(_debugColor + "Build Unit not set!" + _closeColor);
#endif
                
                break;
            }
        }
        
#endif
              */              
    }
    

    private void Start()
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
            if (!Input.anyKeyDown) return;
            
            _run = true; 
            startText.SetActive(false);
            EventSystem.current.SetSelectedGameObject(startButton);
            
                
            menu.SetActive(true);
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
