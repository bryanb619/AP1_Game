using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    public static SceneLoading Instance;

    [SerializeField] private Slider loadingBar;
    //[SerializeField] private GameObject LoadPanel;

    private float target; 


    private void Awake()
    {
        /*
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }

    private void Start()
    {
        LoadScene(); 
    }

    private void Update()
    {
        loadingBar.value = Mathf.MoveTowards(loadingBar.value, target, 3 * Time.deltaTime);
    }

    public async void LoadScene()
    {
        target = 0;
        loadingBar.value = 0f; 
        var scene = SceneManager.LoadSceneAsync("Game");
        scene.allowSceneActivation = false;

        do
        {
            await Task.Delay(2500);
            target = scene.progress;    
            //loadingBar.fillAmount = scene.progress;

        } while (scene.progress < 0.9f);

        await Task.Delay(2000);

        scene.allowSceneActivation = true;
    }
    /*
   // Start is called before the first frame update
   void Start()
   {
       //StartCoroutine(LoadScene());
   }

   private IEnumerator LoadScene() 
   {



       AsyncOperation operation = SceneManager.LoadSceneAsync("Game_TOPDOWN_TEST");



       while (!operation.isDone) 
       {

           float progress = Mathf.Clamp01(operation.progress / .9f);

           loadingBar.value = progress;


           if (progress * 100 % 10 == 0)
           {
               yield return new WaitForSeconds(3f);
           }

           if (progress >= 1f)
           {
               operation.allowSceneActivation = true;
           }


           //Debug.Log(operation.progress);
           yield return null;
       }
   }

   */

}
