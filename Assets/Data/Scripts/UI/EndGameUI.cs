using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private GameObject panel1, panel2; 

   private void Start()
   {
        Time.timeScale = 1.0f;
        panel1.SetActive(true);
   }

   public void NextButton()
   {
        panel1.SetActive(false);
        panel2.SetActive(true);
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
