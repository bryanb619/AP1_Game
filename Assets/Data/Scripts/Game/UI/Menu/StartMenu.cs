using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]    private GameObject _Menu, _OptionsMenu;
    [SerializeField]    private string loadMenu; 

                        // Debug Color
                        private string DebugColor = "<size=14><color=green>";
                        private string closeColor = "</color></size>";


    void Start()
    {
        // Cursor Lock state
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _Menu.SetActive(true);
        _OptionsMenu.SetActive(false);
    }
    #region Start Menu

 
    // Game Load Button
    public void LoadGame()
    {
        SceneManager.LoadScene(loadMenu);
        Debug.Log(DebugColor + "Game Loading!" + closeColor);

    }
    public void Options()
    {
        _Menu.SetActive(false);
        _OptionsMenu.SetActive(true);
    }

    // Game Exit Button
    public void ExitGame()
    {

        Application.Quit();
        Debug.Log(DebugColor + "Game quit!" + closeColor);
    }
    #endregion

    #region Options Menu

    // Back to main Menu Button
    public void BackButton()
    {
        _OptionsMenu.SetActive(false);
        _Menu.SetActive(true);
    }

    #endregion



}
