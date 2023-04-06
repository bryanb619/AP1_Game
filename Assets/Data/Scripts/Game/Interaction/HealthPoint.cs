using UnityEngine;
using FMODUnity;
using System.Security.Cryptography;


//[RequireComponent(typeof(BoxCollider))]
public class HealthPoint : MonoBehaviour
{
    // Data
    [SerializeField] private HealthPointData data;
    private BoxCollider m_BoxCollider;

    // Health
    private int health;
    public int Health => health;

    [SerializeField] private GameObject         m_prefab;
    private Rigidbody                           _rb;

    // Game State
    private GameState                           _state; 
    private bool                                _gamePlay; 

    // FMOD & Sound Handling
    private StudioEventEmitter                  m_Emitter;
    private bool                                _audioState;
    private bool                                _usesAudioAmbient;

    // Atraction 
    private bool                                _canUseForce;
    private float                               startForce; 
    private bool                                _canBeDrawned;
    private int                                 attractionSpeed;
    private float                               maxDistance;
    private bool                                _canIgnorePlayerMaHealth;


    // managing
    private float                               elapsed = 0f;

    float height = 4f;

    private PlayerMovement                      player; 



    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        GetComponents();
        GetProfile();


        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // If the ray hits the terrain, set the floating height to be slightly above the terrain height
            height = hit.point.y + 0.5f;
        }


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

        float RANDOMNUMBER = UnityEngine.Random.Range(1,3);

        startForce = RANDOMNUMBER;  

      
            
                //{ _rb.velocity = transform.forward * startForce; }

    }

    private void GetComponents()
    {
        m_BoxCollider = GetComponent<BoxCollider>();
        m_Emitter = GetComponent<StudioEventEmitter>();
        _rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerMovement>();
    }

    private void GetProfile()
    {
        health = data.Health;
        _canUseForce = data.UseStartForce;
        _canIgnorePlayerMaHealth = data.CanIgnoreHealth;
        _usesAudioAmbient = data.UseAudioAmbient;
        _canBeDrawned = data.CanBeattracted;
        attractionSpeed = data.AtractionSpeed;
        maxDistance = data.MaxDistance;
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

                    transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * 0.1f * height + height, transform.position.z);

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
            transform.position = Vector3.LerpUnclamped(transform.position, player.transform.position, Time.deltaTime);
        } 


        elapsed += Time.deltaTime;

        print(elapsed);

        if(elapsed >= 1f)
        {
            _rb.Sleep();
           
        }
    }


    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
