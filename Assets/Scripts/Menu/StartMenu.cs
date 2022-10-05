using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject _Menu, _OptionsMenu;
    // Debug Color
    string DebugColor = "<size=14><color=green>";
    string closeColor = "</color></size>";


    // Start is called before the first frame update
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
        SceneManager.LoadScene("Game");
        Debug.Log(DebugColor + "Game Loaded!" + closeColor);

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
