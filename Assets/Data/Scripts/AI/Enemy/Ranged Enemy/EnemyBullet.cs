using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{

    #region Variables
    [SerializeField] EnemyProjectileData        data;

    private bool                                _gamePlay;

    private GameState                           _state;

    private Rigidbody                           _rb;
    private float                               m_thrust;
    private bool                                _useRb;

    private int                                 speed;

    public int                                  damage;

    private int                                 maxTime; 

    private float                               elapsed;

    private bool                                _impactEffet;

    private GameObject                          impactObject;

    private float startTime;

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

        startTime = 0f;

        speed = data.speed;

        _useRb = data._useRBPhysics;
        m_thrust = data.thrust; 

        damage = data.damage;

        maxTime = data.timeAirbone;

        _impactEffet = data._useImpact;
        impactObject = data.impactEffect;

        if (_rb != null && _useRb)
        {
            _rb = GetComponent<Rigidbody>();
            _rb.AddForce(transform.forward * m_thrust);
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
            elapsed += Time.deltaTime;

            Debug.Log(elapsed);
            if (elapsed >= maxTime)
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
        PlayerMovement player = hitInfo.GetComponent<PlayerMovement>();

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
        Instantiate(impactObject, transform.position, transform.rotation);

        Destroy(this.gameObject);
    }

    #endregion

    private void DestroyOnDistance()
    {
        Destroy(this.gameObject);
    }

    #region Editor Gizmos
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

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

#endif
    }
    #endregion


    #region Destroy call 
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion
}
