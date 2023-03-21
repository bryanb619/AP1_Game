using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Interactive interactive;
    private int levelToLoad;

    internal void FadeToLevel(int levelIndex)
    {
        animator.SetTrigger("FadeIn");
    }

    private void OnFadeComplete()
    {
        SceneManager.LoadScene(interactive.levelChosen, LoadSceneMode.Single);
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement _player = GetComponent<PlayerMovement>();    
        if(_player != null)
        {
            SceneManager.LoadScene("EndScene");
        }
        
    }
}
