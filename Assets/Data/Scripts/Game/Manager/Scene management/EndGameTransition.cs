using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndGameTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player!= null)
        {
            print("scene changed"); 
            SceneManager.LoadScene("EndScene");
        }

    }
}
