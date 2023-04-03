using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }


    // buttons
    public void RetartButton()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("StartMenu");
    }


}
