using UnityEngine;

public class Tester : MonoBehaviour
{
    [Header("Time Scale at 100 times")]

    [SerializeField] private bool TimeBoost;
    [SerializeField] private bool LightActive;

    [SerializeField] private GameObject GameTestLight;
    [SerializeField] private GameObject Player;

    [SerializeField] private bool killCheat = false;

    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private int damageAmount = 30;

    private bool _show; 




    private void Start()
    {
        //KillEnemy = false;
        TimeBoost = false;
        LightActive = false;
    }

    // Start is called before the first frame update

    void Update()
    {
        CheckBools();
        DamageAllInRange();
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

    /*private void KillAll()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //Die();
            Enemy1.TakeDamage(10, WeaponType.Normal);
            Enemy2.TakeDamage(10, WeaponType.Normal);
        }
    }
    */

    private void DamageAllInRange()
    {
        if(Input.GetKeyDown(KeyCode.Y) && killCheat == true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(Player.transform.position, damageRadius);
            
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Deal damage to the enemy
                    if(hitCollider.GetComponent<EnemyBehaviour>() == true)
                        hitCollider.GetComponent<EnemyBehaviour>().TakeDamage(damageAmount, WeaponType.Normal);

                    else if(hitCollider.GetComponent<EnemyChaseBehaviour>() == true)
                        hitCollider.GetComponent<EnemyChaseBehaviour>().TakeDamage(damageAmount, WeaponType.Normal);
                }
            }
        }
    }
}
