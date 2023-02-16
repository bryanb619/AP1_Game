using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] ProjectileData                             _projectileData;

    [Header("Impact effect")]
    [SerializeField] private GameObject                         impactEffect;

    [SerializeField] private bool                               _impactEffet; 


    [Header("Speed")]
    [Range(1f, 30f)][SerializeField] private int                speed;

    [Header("Magic")]
    [Tooltip("set Magic Type")]
    [SerializeField] private WeaponType                         _type;

    private GameState                                           _state;

    //[Header("Ranged enemy damage value")]
    //[Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    //[SerializeField] 
    private int                                enemyDamage;

    //[Header("Ranged enemy damage value")]
    //[Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    //[SerializeField] 
    private int                                enemyChaseDamage;


    private float                                               elapsed;

    private bool                                                _gamePlay;




    private Rigidbody                                           _rb;

    [Header("Physics")]
    [Tooltip("If set to true configure gravity and speed")]
    [SerializeField] private bool                              _useRBPhysics; 



    


    private void Awake()
    {
        
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

    }
    private void GameManager_OnGameStateChanged(GameState state)
    {
        
        switch (state)
        {
            case GameState.Gameplay:
                {

                    _gamePlay = true;

                    print("gameplay");
                    break;
                }
            case GameState.Paused:
                {
                    _gamePlay = false;

                    print("paused");
                    break;
                }
        }
    }

    private void Start()
    {
        

        if(_rb != null ) 
        {
            _rb = GetComponent<Rigidbody>();
        }

        // auto updating damage values on START
        switch (_state)
        { 
            case GameState.Gameplay:
                {
                    _gamePlay = true;
                    break; 
                }
            case GameState.Paused:
                {
                    _gamePlay = false;
                    break; 
                }
        }

        switch (_type)
        {
            case WeaponType.None:
                {
                    enemyChaseDamage = 0;
                    enemyDamage = 0;

                    //  determine sound types
                    break;
                }
            case WeaponType.Normal:
                {
                    enemyChaseDamage = 10;
                    enemyDamage = 10;

                    //  determine sound types
                    break;
                }
            case WeaponType.Ice:
                {
                    enemyChaseDamage = 10;
                    enemyDamage = 20;

                    //  determine sound types
                    break;
                }
            case WeaponType.Fire:
                {
                    speed = 15;
                    enemyChaseDamage = 10;
                    enemyDamage = 40;

                    //  determine sound types
                    break;
                }

            case WeaponType.Thunder:
                {
                    enemyChaseDamage = 30;
                    enemyDamage = 40;

                    //  determine sound types
                    break;
                }

            default: { break; }
        }

    }

    private void FixedUpdate()
    {
        PhsysicsMovement();
    }

    // Update is called once per frame
    private void Update()
    {
        
        ProjectileMovement();
        UpdatePhysicstTime();

    }


    private void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        EnemyChaseBehaviour ChaseEnemy = hitInfo.GetComponent<EnemyChaseBehaviour>();
        PlayerMovement player = hitInfo.GetComponent<PlayerMovement>();

        if (enemy != null)
        {
            enemy.TakeDamage(enemyDamage, WeaponType.Normal);
            DestroyBullet();
            //Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            ChaseEnemy.TakeDamage(enemyChaseDamage, WeaponType.Normal);
            DestroyBullet();
            Debug.Log("HIT");

        }
        else if (player != null)
        {
            DestroyBullet();
        }
        else if (hitInfo.tag == "Wall" || hitInfo.tag == "Default")
        {
            DestroyBullet();
        }

        //Instantiate(impactEffect, transform.position, transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Default" || collision.gameObject.tag == "Wall")
        {
            
            DestroyBullet();
        }
    }


    private void PhsysicsMovement()
    {
        if (_useRBPhysics)
        {
            if (_gamePlay)
            {
                _rb.constraints = RigidbodyConstraints.None;
            }
            else if (!_gamePlay && _useRBPhysics)
            {
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private void UpdatePhysicstTime()
    {
        if(_gamePlay && _useRBPhysics) 
        {
            elapsed += Time.deltaTime;

            Debug.Log(elapsed);
            if (elapsed >= 6.5f)
            {
                DestroyOnDistance();
            }
        }
    }

    private void ProjectileMovement()
    {
        if(!_useRBPhysics)
        {
            if (_gamePlay)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                elapsed += Time.deltaTime;

                Debug.Log(elapsed);
                if (elapsed >= 6.5f)
                {
                    DestroyOnDistance();
                }

                //Debug.Log(speed + " gameplay ");
            }
        }
        
        
    }

    private void ImpactEffect()
    {
        Instantiate(impactEffect, transform.position, Quaternion.identity);
    }


    private void DestroyBullet()
    {
        if(_impactEffet)
        {
            ImpactEffect();
        }
        
        Destroy(this.gameObject);
    }





    private void DestroyOnDistance()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
