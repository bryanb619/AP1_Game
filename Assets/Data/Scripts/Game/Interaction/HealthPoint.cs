using UnityEngine;
using FMODUnity;
using System.Security.Cryptography;


//[RequireComponent(typeof(BoxCollider))]
public class HealthPoint : MonoBehaviour
{
    // Data //
    [SerializeField] private HealthPointData    data;

    //private BoxCollider                         m_BoxCollider;

    // Health //
    private int                                 _health;
    public int                                  Health => _health;

    [SerializeField] private GameObject         m_prefab;
    private Rigidbody                           _rb;

    // Game State //
    private GameState                           _state; 
    private bool                                _gamePlay; 

    // FMOD & Sound Handling //
    private StudioEventEmitter                  m_Emitter;
    private bool                                _audioState;
    private bool                                _usesAudioAmbient;

    // Atraction //
    private bool                                _canUseForce;
    //private float                               startForce; 
    private bool                                _canBeDrawned;
    private int                                 attractionSpeed;
    private float                               maxDistance;
    private bool                                _canIgnorePlayerMaHealth;

    private bool                                _canFloat;

    private LayerMask                           _layerMask;
    public float                                avoidanceForce = 5f;
    public float                                avoidanceDuration = 1.8f;



    // managing // 

    // height of float
    private float                               height;

    private PlayerMovement                      player; 


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {

        _canFloat = true;
        GetComponents();
        GetProfile();
        DetectGround(); 


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

        //float RANDOMNUMBER = UnityEngine.Random.Range(1,3);

        //startForce = RANDOMNUMBER;  

        //{ _rb.velocity = transform.forward * startForce; }

    }
    #region Components, profile & Raycast
    private void GetComponents()
    {
        //m_BoxCollider = GetComponent<BoxCollider>();
        m_Emitter = GetComponent<StudioEventEmitter>();
        _rb = GetComponent<Rigidbody>();

        player = FindObjectOfType<PlayerMovement>();
    }

    private void GetProfile()
    {
        _health = data.Health;
        _canUseForce = data.UseStartForce;
        _canIgnorePlayerMaHealth = data.CanIgnoreHealth;
        height = data.HeightFloat; 
        _usesAudioAmbient = data.UseAudioAmbient;
        _canBeDrawned = data.CanBeattracted;
        attractionSpeed = data.AtractionSpeed;
        maxDistance = data.MaxDistance;
        _layerMask = data.LayerMask;    
    }

    private void DetectGround()
    {
       if(_canBeDrawned) 
       {
            RaycastHit groundHit;

            if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
            {

                height = groundHit.point.y + 1f;
            }
       }
        
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
    #endregion

    private void FixedUpdate() 
    {
        if (_canUseForce && _gamePlay)
        {

            
            //_rb.AddForce(transform.forward * startForce);
            //_canUseForce = false;

            //_rb.Sleep(); 

        }
    }
    private void Update()
    {

        
        switch (_gamePlay) 
        {
           
            case true: 
                {

                    if (_canFloat) 
                    {
                        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * 0.1f * height + height, transform.position.z);
                    }
                    

                    //Debug.Log("In use");
                    if (_usesAudioAmbient) 
                    {
                        _audioState = false;
                        UpdateSound();
                        
                        return; 
                    }

                    if(_canBeDrawned)
                    {
                        OnDraw();
                    }

                    

                    break; 
                }
            case false: 
                {
                    if (_usesAudioAmbient)
                    {
                        _audioState = true;

                        if(_canBeDrawned)
                        {
                            _rb.Sleep();
                        }
                        UpdateSound();
                        return;
                    }
                    break; 
                }
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (player._playerHealthState == PlayerMovement._PlayerHealth.NotMax || _canIgnorePlayerMaHealth)
            {
                //m_BoxCollider.enabled = false;

                player.Takehealth(Health);

                Instantiate(m_prefab, transform.position, Quaternion.identity);
                //Destroy(m_BoxCollider);
                Destroy(gameObject);
            }
            //player
        }

        // Check if the other collider is on the layer we want to avoid
        if (other.gameObject.layer == LayerMask.NameToLayer("InteractiveZone"))
        {
            print("detected"); 
            //_canFloat = false; 

            // Calculate the direction to move away from the other collider
            Vector3 avoidanceDirection = transform.position - other.transform.position;

            // Apply a force to move away from the other collider
            _rb.AddForce(avoidanceDirection.normalized * avoidanceForce);

            Invoke("StopForce", avoidanceDuration);
        }
    }



    private void StopForce()
    {
        //print("no one found");
        _rb.velocity = Vector3.zero;
        _rb.Sleep();

        //_canFloat = true;
    }


    private void UpdateSound()
    {
        m_Emitter.EventInstance.setPaused(_audioState); // set audio
    }


    private void OnDraw()
    {
        var step = attractionSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, player.transform.position) < maxDistance)
        {

            _canFloat = false; 
            transform.position = Vector3.LerpUnclamped(transform.position, player.transform.position, Time.deltaTime);
            //print("player"); 
        }
        else { _canFloat = true; return;  }


        //elapsed += Time.deltaTime;

        //print(elapsed);

        /*if(elapsed >= 1f)
        {
            _rb.Sleep();







        using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public float avoidanceForce = 10f;

    void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is on the layer we want to avoid
        if (other.gameObject.layer == LayerMask.NameToLayer("LayerToAvoid"))
        {
            // Calculate the direction to move away from the other collider
            Vector3 avoidanceDirection = transform.position - other.transform.position;

            // Apply a force to move away from the other collider
            GetComponent<Rigidbody>().AddForce(avoidanceDirection.normalized * avoidanceForce);
        }
    }
}
           
        }
        */
    }





    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
