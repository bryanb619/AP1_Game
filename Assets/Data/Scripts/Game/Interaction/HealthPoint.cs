using UnityEngine;
using FMODUnity;


[RequireComponent(typeof(BoxCollider))]
public class HealthPoint : MonoBehaviour
{
    [SerializeField] private HealthPointData data;
    private BoxCollider m_BoxCollider;

    private int m_Health;
    public int Health => m_Health;

    [SerializeField] private GameObject m_prefab;

    private GameState                   _state; 

    private bool                        _gamePlay; 

    private StudioEventEmitter          m_Emitter;

    private bool                        _audioState;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        m_BoxCollider = GetComponent<BoxCollider>();
        m_Emitter = GetComponent<StudioEventEmitter>(); 
       
        m_Health = data.Health;

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

    private void Update()
    {
        switch (_gamePlay) 
        {
            case true: 
                {
                    //Debug.Log("In use");
                    _audioState = false;
                    UpdateSound();
                    break; 
                }
            case false: 
                {
                    _audioState = true;
                    UpdateSound();
                    break; 
                }
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (player._playerHealthState == PlayerMovement._PlayerHealth.NotMax)
            {
                m_BoxCollider.enabled = false;
                
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
        m_Emitter.EventInstance.setPaused(_audioState); // set play
    }

}
