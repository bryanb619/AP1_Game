using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile speed")]
    [Range(1f, 30f)][SerializeField] private int                speed;

    [Tooltip("Set weapon type")]
    [SerializeField] private WeaponType                         _type;

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int                                enemyDamage;

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int                                enemyChaseDamage;


    private float                                               elapsed;

    private bool                                                _gamePlay;


    private Rigidbody                                           _rb;


    private Vector3                                             m_YAxis;




    private void Awake()
    {
        
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        _rb = GetComponent<Rigidbody>();

        // auto updating damage values on START
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
        _rb.velocity = transform.forward * speed;

    }

    // Update is called once per frame
    private void Update()
    {
        
        

        if (_gamePlay)
        {
            _rb.constraints = RigidbodyConstraints.None;

            //Debug.Log(speed + " gameplay ");
            //transform.Translate(Vector3.forward * speed * Time.deltaTime);

            elapsed += Time.deltaTime;

            Debug.Log(elapsed);
            if (elapsed >= 6.5f)
            {
                DestroyOnDistance();
            }
        }
        else if(!_gamePlay) 
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        

    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();
        EnemyChaseBehaviour ChaseEnemy = other.GetComponent<EnemyChaseBehaviour>();



        if (enemy != null)
        {
            enemy.TakeDamage(enemyDamage, _type);
            
            Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            ChaseEnemy.TakeDamage(enemyChaseDamage, _type);
            
            Debug.Log("HIT");

        }
   
        DestroyProjectile();

    }
    */

    private void DestroyProjectile()
    {

        // play collision sound

        // instantiate impact effect

        Destroy(gameObject);



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
