using UnityEngine;
using FMODUnity; 


public class Drop : MonoBehaviour
{
    // Data //
    [SerializeField] private DropData data;

    private DropType _dropType;

    //private BoxCollider m_BoxCollider;

    // Health //
    private int _health;
    public int Health => _health;

    // Mana //
    private int _mana;
    public int Mana => _mana;


    [SerializeField] private GameObject m_prefab;
    private Rigidbody _rb;

    // Game State //
    private GameState _state;
    private bool _gamePlay;

    // FMOD & Sound Handling //
    private StudioEventEmitter m_Emitter;
    private bool _audioState;
    private bool _useAudio;
    private bool _usesAudioAmbient;

    // Atraction //
    private bool _canUseForce;
    //private float                               startForce; 
    private bool _canBeDrawned;
    private int attractionSpeed;
    private float maxDistance;
    private bool _canIgnorePlayerMaHealth;

    private bool _canFloat;

    private LayerMask _ignoreMask;
    public float avoidanceForce = 5f;
    public float avoidanceDuration = 1.8f;



    // managing // 

    // height of float
    private float height;

    private PlayerMovement player;

    //private ManaManager manaManager;


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {


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
        //manaManager = FindObjectOfType<ManaManager>();    
    }

    private void GetProfile()
    {
        _dropType = data.Drop; 

        _health = data.Health;
        _canFloat = data.Float;
        _canUseForce = data.UseStartForce;
        _canIgnorePlayerMaHealth = data.CanIgnoreHealth;
        height = data.HeightFloat;
        _usesAudioAmbient = data.UseAudioAmbient;
        _canBeDrawned = data.CanBeattracted;
        attractionSpeed = data.AtractionSpeed;
        maxDistance = data.MaxDistance;
        _ignoreMask = data.LayerMask;
    }

    private void DetectGround()
    {
        if (_canFloat)
        {
            RaycastHit groundHit;

            if (Physics.Raycast(transform.position, Vector3.down, out groundHit, ~_ignoreMask))
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

                    if (_canBeDrawned)
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

                        if (_canBeDrawned)
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
        PlayerMovement PLAYER = other.GetComponent<PlayerMovement>();
        ManaManager MANA = other.GetComponent<ManaManager>();   
        if (PLAYER != null)
        {
            if (PLAYER._playerHealthState == PlayerMovement._PlayerHealth.NotMax || _canIgnorePlayerMaHealth)
            {
                //m_BoxCollider.enabled = false;
                switch(_dropType)
                {
                    case DropType._HP: 
                        {
                            //player.Takehealth(Health);
                            PLAYER.Takehealth(Health);
                            print("NOT LINKED UP TO HP");
                            break;  
                        }
                    case DropType._MANA: 
                        {
                            print("NOT LINKED UP MANA");
                            MANA.RecoverMana(Mana);
                            break; 
                        }
                    default: { break; }

                }
                

                if (_useAudio)
                {
                    Instantiate(m_prefab, transform.position, Quaternion.identity);
                }

                //Destroy(m_BoxCollider);
                Destroy(gameObject);
            }
            //player
        }

        // Check if the other collider is on the layer we want to avoid
        if (other.gameObject.layer == LayerMask.NameToLayer("InteractiveZone"))
        {
            //print("detected"); 

            Vector3 avoidanceDirection = transform.position - other.transform.position;

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
        else { _canFloat = true; return; }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
