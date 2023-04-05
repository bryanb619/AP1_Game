using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndGameTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement _player = other.GetComponent<PlayerMovement>();

        if (_player!= null)
        {
            print("scene changed"); 
            SceneManager.LoadScene("EndScene");
        }

    }
}
