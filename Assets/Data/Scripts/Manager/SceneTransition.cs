using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int levelToLoad;

    internal void FadeToLevel(int levelIndex)
    {
        animator.SetTrigger("FadeIn");
    }

    private void OnFadeComplete(int levelIndex)
    {
        Debug.Log("Entered and index is: " + levelIndex);
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
    }
}
