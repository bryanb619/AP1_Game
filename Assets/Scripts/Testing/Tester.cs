using UnityEngine;

public class Tester : MonoBehaviour
{
    [Header("Time Scale at 100 times")]

    [SerializeField] private bool TimeBoost;
    [SerializeField] private bool LightActive;

    [SerializeField] private GameObject GameTestLight;



    private void Start()
    {
        TimeBoost = false;
        LightActive = false;
    }

    // Start is called before the first frame update

    void Update()
    {
        CheckBools();
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





}
