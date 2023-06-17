using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;


public class Drop : MonoBehaviour
{
    // Data --------------------------------------------------->
    [SerializeField]    private DropData data;

                        private DropType _dropType;

                        //private BoxCollider m_BoxCollider;

    // Health --------------------------------------------------->
                        private int _health;
                        public int Health => _health;

    // Mana --------------------------------------------------->
                        private int _mana;
                        public int Mana => _mana;


    [FormerlySerializedAs("m_prefab")] [SerializeField]    private GameObject mPrefab;
                        private Rigidbody _rb;

                        // Game State //
                        private GameState _state;
                        private bool _gamePlay;

    // FMOD & Sound Handling --------------------------------------------------->
    private StudioEventEmitter _mEmitter;
                        private bool _audioState;
                        private bool _useAudio;
                        private bool _usesAudioAmbient;

    // Atraction --------------------------------------------------->
                        private bool _canUseForce;
                        //private float                               startForce; 
                        private bool _canBeDrawned;
                        private int _attractionSpeed;
                        private float _maxDistance;
                        private bool _canIgnorePlayerMaHealth;

                        private bool _canFloat;

                        private LayerMask _ignoreMask;
                        public float avoidanceForce = 5f;
                        public float avoidanceDuration = 1.8f;


    // managing --------------------------------------------------->
                        // height of float 
                        private float _height;

                        private PlayerMovement _player;



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
        _mEmitter                   = GetComponent<StudioEventEmitter>();
        _rb                         = GetComponent<Rigidbody>();

        _player                      = FindObjectOfType<PlayerMovement>();
        //manaManager = FindObjectOfType<ManaManager>();    
    }

    private void GetProfile()
    {
        _dropType                   = data.Drop; 

        _health                     = data.Health;
        _mana                       = data.Mana;

        _canFloat                   = data.Float;
        _canUseForce                = data.UseStartForce;
        _canIgnorePlayerMaHealth    = data.CanIgnoreHealth;
        _height                      = data.HeightFloat;
        _usesAudioAmbient           = data.UseAudioAmbient;
       
        _canBeDrawned               = data.CanBeattracted;
        _attractionSpeed             = data.AtractionSpeed;
        _maxDistance                 = data.MaxDistance;
        _ignoreMask                 = data.LayerMask;


    }

    private void DetectGround()
    {
        if (_canFloat)
        {
            RaycastHit groundHit;

            if (Physics.Raycast(transform.position, Vector3.down, out groundHit, _ignoreMask))
            {

                _height = groundHit.point.y + 1.3f;
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
                        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * 0.1f * _height + _height, transform.position.z);
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
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        ManaManager mana = other.GetComponent<ManaManager>();   

        if (player != null)
        {
            //m_BoxCollider.enabled = false;
            switch (_dropType)
            {
                case DropType.Hp:
                    {
                        //player.Takehealth(Health);
                        //PLAYER.Takehealth(Health);
                        string debugColor = "<size=12><color=green>";
                        string closeColor = "</color></size>";

                        Debug.Log(debugColor + "HP picked" + closeColor);
                        player.RegenerateHealth(Health);

                        break;
                    }
                case DropType.Mana:
                    {
                        //print("NOT LINKED UP MANA");
                        string debugColor = "<size=12><color=lightblue>";
                        string closeColor = "</color></size>";

                        Debug.Log(debugColor + "Mana picked" + closeColor);

                       
                        mana.RecoverMana(Mana);
                        break;
                    }
                default: { break; }

            }

            if (_useAudio && mPrefab != null)
            {
                Instantiate(mPrefab, transform.position, Quaternion.identity);
            }

            //Destroy(m_BoxCollider);
            Destroy(gameObject);
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
        _mEmitter.EventInstance.setPaused(_audioState); // set audio
    }


    private void OnDraw()
    {

        if (Vector3.Distance(transform.position, _player.transform.position) < _maxDistance)
        {
            _canFloat = false;
            transform.position = Vector3.LerpUnclamped(transform.position, _player.transform.position,_attractionSpeed * Time.deltaTime);
            //print("player"); 
        }
        else { _canFloat = true; return; }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
