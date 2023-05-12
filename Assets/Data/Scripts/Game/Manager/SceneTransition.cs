using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Interactive interactive;
    private int _levelToLoad;

    internal void FadeToLevel(int levelIndex)
    {
        animator.SetTrigger("FadeIn");
    }

    private void OnFadeComplete()
    {
        SceneManager.LoadScene(interactive.levelChosen, LoadSceneMode.Single);
    }
}
