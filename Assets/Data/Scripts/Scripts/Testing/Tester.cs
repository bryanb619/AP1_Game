using System;
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


    [SerializeField] private GameObject cam;
    private bool _isCamActive; 

    private bool _show;


    private void Awake()
    {
        _player         = GameObject.FindGameObjectWithTag("Player") ;
        //KillEnemy = false;
      

    }

    private void Start()
    {
        timeBoost       = false;
        lightActive     = false;
        _isCamActive    = false; 
     
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
            DisableTestLight();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            switch (_isCamActive)
            {
                case true:
                {
                    _isCamActive = false; 
                    
                    ActionOnCamera(_isCamActive);
                    break; 
                }
                case false:
                {
                    _isCamActive = true; 
                    ActionOnCamera(_isCamActive);
                    break; 
                }
            }
        }
    }

    private static void AiGameTest()
    {
        Time.timeScale = 100;
    }

    private static void ResetTimeScale()
    {
        Time.timeScale = 1;
    }


    private void TestLight()
    {
        gameTestLight.SetActive(true);
    }

    private void DisableTestLight()
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
                    
                    if(hitCollider.GetComponent<EnemyRangedBehaviour>() == true)
                        hitCollider.GetComponent<EnemyRangedBehaviour>().TakeDamage(damageAmount, WeaponType.Normal,0);
                    
                    if(hitCollider.GetComponent<RangedBossBehaviour>() == true)
                        hitCollider.GetComponent<RangedBossBehaviour>().TakeDamage(damageAmount, WeaponType.Normal,0);
                    
                        
                }
            }
        }
    }

    private void ActionOnCamera(bool activate)
    {

        switch (activate)
        {
            case true:
            {
                cam.SetActive(false);
                break;
            }
            case false:
            {
                cam.SetActive(true);
                break; 
            }
            
        }
        
    }
}
