using UnityEngine;

public class Tester : MonoBehaviour
{
    [Header("Time Scale at 100 times")]

    [SerializeField] private bool TimeBoost;
    [SerializeField] private bool LightActive;

    [SerializeField] private GameObject GameTestLight;

    [SerializeField] private EnemyBehaviour Enemy1;
    [SerializeField] private EnemyChaseBehaviour Enemy2;

    public bool KillEnemy;

    private bool _show; 




    private void Start()
    {
        KillEnemy = false;
        TimeBoost = false;
        LightActive = false;

        //Enemy1 = FindObjectOfType<EnemyBehaviour>();
        //Enemy2 = FindObjectOfType<EnemyChaseBehaviour>();


    }

    // Start is called before the first frame update

    void Update()
    {
        CheckBools();
        KillAll();

        if (Input.GetKeyDown(KeyCode.M))
        {
            
            if(!_show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _show = true;
            }
            else if(_show) 
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;

                _show = false;
            }
            
        }
    }


    private void CheckBools()
    {
        if (TimeBoost)
        {
            AIGameTest();
        }
        else if (TimeBoost == false)
        {
            ResetTimeScale();
        }

        if (LightActive)
        {
            TestLight();
        }
        else if (LightActive == false)
        {
            DisabletestLight();
        }
    }

    private void AIGameTest()
    {
        Time.timeScale = 100;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1;
    }


    private void TestLight()
    {
        GameTestLight.SetActive(true);
    }

    private void DisabletestLight()
    {
        GameTestLight.SetActive(false);
    }

    private void KillAll()
    {
        if (Input.GetKeyDown(KeyCode.T) || KillEnemy == true)
        {
            //Die();
            //Enemy1.TakeDamage(100);
            //Enemy2.TakeDamage(100); 
        }
        if (Input.GetKeyDown(KeyCode.Y)) //|| KillEnemy == true)
        {
            //Die();
            //Enemy1.TakeDamage(25);
            //Enemy2.TakeDamage(25);
        }
    }





}
