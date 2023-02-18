using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    #region Variables
    [SerializeField] ProjectileData data;

    private bool _gamePlay;

    private GameState _state; 
    private WeaponType _weaponType;

    private Rigidbody _rb;

    private bool _useRb;

    private float elapsed;
    private int speed;
    private int rangedDamage, chaseDamage, bossDamage;

    private bool _impactEffet;
    private GameObject impactObject;

    private StudioEventEmitter sound;  

    #endregion

    #region Awake & Start 
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
                    //playShootSound.setPaused(false);
                    //print("gameplay");
                    sound.Play();   
                    break;
                }
            case GameState.Paused:
                {
                    _gamePlay = false;
                    //playShootSound.getPaused(!_gamePlay, out);

                    //playShootSound.setPaused(true);

                    
                    //print("paused");
                    break;
                }
        }
    }

    private void Start()
    {

        

        if (_rb != null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        if(sound!= null) 
        {
            sound = GetComponent<StudioEventEmitter>();

            sound.Play();
        }

        // speed 
        speed = data.speed;

        // sound
        // playShootSound = data.MagicSound;


        // rigidbody 
        _useRb = data._useRBPhysics;
        //  weapon type
        _weaponType = data._magic;

        // enemies damage
        rangedDamage = data.enemyRangedDamage;
        chaseDamage = data.enemyChaseDamage;
        bossDamage = data.enemybossDamage;

        // impact
        _impactEffet = data._useImpact;
        impactObject = data.impactEffect;


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
    }

    #endregion
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
            enemy.TakeDamage(rangedDamage, _weaponType);
            
            DestroyBullet();
            //Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            ChaseEnemy.TakeDamage(chaseDamage, _weaponType);
            
            DestroyBullet();
            

        }
        else if (player != null)
        {
            DestroyBullet();
        }
        else if (hitInfo.tag == "Wall" || hitInfo.tag == "Default")
        {
            DestroyBullet();
        }
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
        if (_useRb)
        {
            if (_gamePlay)
            {
                _rb.constraints = RigidbodyConstraints.None;
            }
            else if (!_gamePlay && _useRb)
            {
                _rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private void UpdatePhysicstTime()
    {
        if (_gamePlay && _useRb)
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
        if (!_useRb)
        {
            if (_gamePlay)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                elapsed += Time.deltaTime;

                //Debug.Log(elapsed);
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
        Instantiate(impactObject, transform.position, Quaternion.identity);
    }

    private void DestroyBullet()
    {
       if (_impactEffet)
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
