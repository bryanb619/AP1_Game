using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile speed")]
    [Range(1f, 30f)][SerializeField] private int                speed;

    [Tooltip("Set weapon type")]
    [SerializeField] private WeaponType                         _type;

    private GameState                                           _state;

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int                                enemyDamage;

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int                                enemyChaseDamage;


    private float                                               elapsed;

    private bool                                                _gamePlay;


    //private Rigidbody                                           _rb;

    private void Awake()
    {
        
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        //_rb = GetComponent<Rigidbody>();

        
       
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

    // Update is called once per frame
    private void Update()
    {
        ProjectileMovement();
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


    private void ProjectileMovement()
    {
        if (_gamePlay)
        {
            //_rb.constraints = RigidbodyConstraints.None;

            //Debug.Log(speed + " gameplay ");
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            elapsed += Time.deltaTime;

            Debug.Log(elapsed);
            if (elapsed >= 6.5f)
            {
                DestroyOnDistance();
            }
        }
        else if (!_gamePlay)
        {
            //_rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }


    private void DestroyBullet()
    {
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
