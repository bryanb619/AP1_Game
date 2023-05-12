using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{

    #region Variables
    [SerializeField] EnemyProjectileData        data;

    private bool                                _gamePlay;

    private GameState                           _state;

    private Rigidbody                           _rb;
    private float                               _mThrust;
    private bool                                _useRb;

    private int                                 _speed;

    private int                                  _damage;

    private int                                 _maxTime; 

    private float                               _elapsed;

    private bool                                _impactEffet;

    private GameObject                          _impactObject;
    #endregion

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
                    //sound.Play();   
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

    // Start is called before the first frame update
    void Start()
    {

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

       

        _speed = data.speed;

        _useRb = data.useRbPhysics;
        _mThrust = data.thrust; 

        _damage = data.damage;

        _maxTime = data.timeAirbone;

      
        _impactEffet = data.useImpact;

        _impactObject = data.impactEffect;

        if (_rb != null && _useRb)
        {
            _rb = GetComponent<Rigidbody>();
            _rb.AddForce(transform.forward * _mThrust);
        }
    }

    private void FixedUpdate()
    {
        PhsysicsMovement();
    }
    private void Update()
    {
        ProjectileMovement();

        UpdatePhysicstTime();
    }

    #region Physics, movement and Time in game
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
        if (_gamePlay)
        {
            _elapsed += Time.deltaTime;

            //Debug.Log(elapsed);
            if (_elapsed >= _maxTime)
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
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
                //Debug.Log(speed + " gameplay ");
            }
        }
    }
    #endregion

    #region Collision
    private void OnTriggerEnter(Collider hitInfo)
    {
        //EnemyBehaviour enemy = hitInfo.GetComponent<EnemyBehaviour>();
        //EnemyChaseBehaviour ChaseEnemy = hitInfo.GetComponent<EnemyChaseBehaviour>();
        PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();

/*
        if (enemy != null)
        {
           
            DestroyBullet();
            //Debug.Log("HIT");

        }
        else if (ChaseEnemy != null)
        {
            DestroyBullet();

        }
*/
        if (player != null)
        {
           print("PLAYER DAMAGE");
            player.TakeDamage(_damage);
            DestroyBullet();
            
        }
        else if (hitInfo.tag == "Wall" || hitInfo.tag == "Default")
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

    private void DestroyBullet()
    {
        if(_useRb) 
        {
            
        }
        if(_impactEffet)
        {
            Instantiate(_impactObject, transform.position, transform.rotation);

        }
       
        Destroy(this.gameObject);
    }

    #endregion

    private void DestroyOnDistance()
    {
        Destroy(this.gameObject);
    }

    #region Destroy call 
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion
#if UNITY_EDITOR
    
        #region Editor Gizmos
        private void OnDrawGizmos()
        {
  

            switch (_gamePlay)
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

   
        }
        #endregion
    
#endif
}
