#region Library
using UnityEngine;
using FMODUnity;
using UnityEditor;

#endregion

public class TestProjectile : MonoBehaviour
{
    #region Variables
    [SerializeField] ProjectileData     data;

    private bool                        _gamePlay;

    private GameState                   _state; 

    private WeaponType                  _weaponType;

    private Rigidbody                   _rb;
    private float                       thrust; 
    private bool                        _useRb;

    private float                       elapsed;
    private int                         speed;

    private int                         rangedDamage, chaseDamage, bossDamage;

    private bool                        _impactEffet;
    private GameObject                  impactObject;

    //private EventInstance               effectSound;
    //[SerializeField]
    private StudioEventEmitter          _emitter;

    private float                       volume; 
    #endregion

    #region Awake
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    #endregion

    #region Start
    private void Start()
    {
        CollectData();
        UseRB();
    }

    /// <summary>
    ///  Data collection of:
    ///  Components
    ///  Variables
    ///  Game State
    /// </summary>
    private void CollectData()
    {
        #region Check Game State at start

        _emitter = GetComponent<StudioEventEmitter>();

        //OnSFXValueChange.AddListener(Setvolume);

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
        #endregion

        #region Scriptable object data
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

        //effectSound = RuntimeManager.CreateInstance("event:/path/to/your/sound");
        //effectSound.start();

        #endregion
    }

    private void UseRB()
    {
        if (_useRb && _rb != null)
        {
            _rb = GetComponent<Rigidbody>();
        }
    }

    #endregion

    #region Fixed update & Update
    private void FixedUpdate()
    {
        PhsysicsMovement();
    }

    // Update is called once per frame
    private void Update()
    {
        ProjectileMovement();
        UpdatePhysicstTime();
        SoundUpdate();
    }
    #endregion

    #region Collision
    private void OnTriggerEnter(Collider hitInfo)
    {
        EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        EnemyChaseBehaviour ChaseEnemy = hitInfo.GetComponent<EnemyChaseBehaviour>();
        //PlayerMovement player = hitInfo.GetComponent<PlayerMovement>();

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
        /*
        else if (player != null)
        {
            DestroyBullet();
        }
        */
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
    #endregion

    #region Physics, Movement, Time and sound
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

            //Debug.Log(elapsed);
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

    /// <summary>
    /// Set sound state acording to game state
    /// </summary>
    private void SoundUpdate()
    {
        switch(_gamePlay) 
        {
            case true:
                {
                    SoundSetPlay();
                    break;
                }
            case false:
                {
                    SoundSetPause();
                    break;
                }
        }
    }

    private void SoundSetPlay()
    {
        _emitter.EventInstance.setPaused(false); // set play
    }

    private void SoundSetPause()
    {
        _emitter.EventInstance.setPaused(true); // set paused
    }
    
    private void Setvolume(float newVolume)
    {
        volume = newVolume; 
        _emitter.SetParameter("volume", volume);
    }

    private void ImpactEffect()
    {
        // spawn projectile
        Instantiate(impactObject, transform.position, Quaternion.identity);
    }
    #endregion

    #region Destroy Game object
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

    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
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

    // unsubscribe of Game manager on destroy
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged; 
    }
    #endregion

    /*
    #region Editor Gizmos
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        switch(_gamePlay)
        {
            case true:
                {
                    Handles.Label(transform.position + Vector3.up, "Gameplay");
                    break; 
                }
            case false:
                {
                    Handles.Label(transform.position + Vector3.up, "Pause");
                    break;
                }
        }

#endif
    }
    #endregion
    */
}
