using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndGameUI : MonoBehaviour
{
   private void Start()
   {
        Time.timeScale = 1.0f;
   }

   public void loadMainMenu()
   {
        SceneManager.LoadScene("StartMenu"); 
   }

   public void ExitGame()
   {
        Application.Quit();
   }
}
