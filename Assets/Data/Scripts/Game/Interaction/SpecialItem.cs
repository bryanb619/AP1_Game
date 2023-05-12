using UnityEngine;
using FMODUnity;


//[RequireComponent(typeof(BoxCollider))]
public class SpecialItem : MonoBehaviour
{
    // Data //
    [SerializeField] private SpecialDropData    data;

    private BoxCollider                         _mBoxCollider;
    private DropType                            _dropType;   

    // Health //
    private int                                 _healthEmpower;
    public int                                  Health => _healthEmpower;

    // Mana //

    private int                                 _manaEmpower; 
    internal int                                Mana => _manaEmpower;


    [SerializeField] private GameObject         soundGameObject;
    private Rigidbody                           _rb;

    // Game State //
    private GameState                           _state; 
    private bool                                _gamePlay;

    private bool                                _rotate;
    private float                               _rotateSpeed; 

    // FMOD & Sound Handling //
    private StudioEventEmitter                  _mEmitter;
    private bool                                _audioState;
    private bool                                _useAudio;
    private bool                                _usesAudioAmbient;

    // Atraction //
    private bool                                _canUseForce;
    //private float                               startForce; 
    private bool                                _canBeDrawned;
    private int                                 _attractionSpeed;
    private float                               _maxDistance;

    private bool                                _canFloat;

    private LayerMask                           _ignoreMask;
    public float                                avoidanceForce = 5f;
    public float                                avoidanceDuration = 1.8f;

    private bool                                _canActivateCollider;
    private float                               _elapsed; 

    // managing // 

    // height of float
    private float                               _height;

    private PlayerMovement                      _player;

    [SerializeField] private GameObject         parent; 

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
        _mBoxCollider = GetComponent<BoxCollider>();   

        _mEmitter = GetComponent<StudioEventEmitter>();
        _rb = GetComponent<Rigidbody>();

        _player = FindObjectOfType<PlayerMovement>();
    }

    private void GetProfile()
    {
        _dropType                   = data.Drop;
        _healthEmpower              = data.Health;

        _manaEmpower                = data.Mana;   
        
        _canFloat                   = data.Float; 
        _canUseForce                = data.UseStartForce;

        _height                      = data.HeightFloat;


        //_rotate                     = data.Rotate;
        _rotateSpeed                 = data.RotateSpeed;

        _usesAudioAmbient           = data.UseAudioAmbient;
        _canBeDrawned               = data.CanBeattracted;
        _attractionSpeed             = data.AtractionSpeed;
        _maxDistance                 = data.MaxDistance;
        _ignoreMask                 = data.LayerMask;    
    }

    private void DetectGround()
    {
       if(_canFloat) 
       {
            RaycastHit groundHit;

            if (Physics.Raycast(transform.position, Vector3.down, out groundHit, ~_ignoreMask))
            {

                _height = groundHit.point.y + 1f;
            }
       }
        
    }

    internal bool CheckRotation()
    {
        return true; 
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

                    if(_canBeDrawned )
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
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        ManaManager mana = other.GetComponent<ManaManager>();

        if (player != null)
        {
            switch (_dropType)
            {
                case DropType.SpecialHealth:
                    {
                        string debugColor = "<size=14><color=green>";
                        string closeColor = "</color></size>";

                        Debug.Log(debugColor + "HP Increased" + closeColor);

                        // increase health bar
                        player.EmpowerHealth(Health);

                        // take health
                        player.Takehealth(Health);

                        break;
                    }
                case DropType.SpecialMana:
                    {
                        string debugColor = "<size=14><color=lightblue>";
                        string closeColor = "</color></size>";


                        Debug.Log(debugColor + "Mana Increased" + closeColor);
                        
                        // Increase mana bar
                        mana.ManaIncrease(Mana);

                        // take aditional mana
                        mana.RecoverMana(Mana);

                        break;
                    }
            }
        }

        if (_useAudio)
        {
            Instantiate(soundGameObject, transform.position, Quaternion.identity);
        }

        //Destroy(m_BoxCollider);
        //Destroy(gameObject);
        Destroy(parent);

        
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
        var step = _attractionSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _player.transform.position) < _maxDistance)
        {

            _canFloat = false; 
            transform.position = Vector3.LerpUnclamped(transform.position, _player.transform.position, Time.deltaTime);
            //print("player"); 
        }
        else { _canFloat = true; return;}
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    
}
