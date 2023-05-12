using UnityEngine;
using UnityEngine.Serialization;

public class Tester : MonoBehaviour
{
    [FormerlySerializedAs("TimeBoost")]
    [Header("Time Scale at 100 times")]

    [SerializeField] private bool timeBoost;
    [FormerlySerializedAs("LightActive")] [SerializeField] private bool lightActive;

    [FormerlySerializedAs("GameTestLight")] [SerializeField] private GameObject gameTestLight;
                     private GameObject _player;

    [SerializeField] private bool killCheat = false;

    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private int damageAmount = 30;

    private bool _show; 




    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player") ;
        //KillEnemy = false;
        timeBoost = false;
        lightActive = false;
    }

    // Start is called before the first frame update

    void Update()
    {
        CheckBools();
        DamageAllInRange();
    }


    private void CheckBools()
    {
        if (timeBoost)
        {
            AiGameTest();
        }
        else if (timeBoost == false)
        {
            ResetTimeScale();
        }

        if (lightActive)
        {
            TestLight();
        }
        else if (lightActive == false)
        {
            DisabletestLight();
        }
    }

    private void AiGameTest()
    {
        Time.timeScale = 100;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1;
    }


    private void TestLight()
    {
        gameTestLight.SetActive(true);
    }

    private void DisabletestLight()
    {
        gameTestLight.SetActive(false);
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
            Collider[] hitColliders = Physics.OverlapSphere(_player.transform.position, damageRadius);
            
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Deal damage to the enemy
                    
                    /*
                     if(hitCollider.GetComponent<EnemyBehaviour>() == true)
                        hitCollider.GetComponent<EnemyBehaviour>().TakeDamage(damageAmount, WeaponType.Normal);
                    */
                    if(hitCollider.GetComponent<EnemyChaseBehaviour>() == true)
                        hitCollider.GetComponent<EnemyChaseBehaviour>().TakeDamage(damageAmount, WeaponType.Normal);
                }
            }
        }
    }
}
