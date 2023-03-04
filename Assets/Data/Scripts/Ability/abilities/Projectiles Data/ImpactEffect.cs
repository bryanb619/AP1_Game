using UnityEngine;
using FMODUnity;
using UnityEditor;

public class ImpactEffect : MonoBehaviour
{
    [SerializeField] private ImpactData         data;

    private float                               elapsed;
    private int                                 maxTime;

    private GameState                           _state;
    private bool                                _gamePlay;

    private StudioEventEmitter                  emitter;



    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    // Start is called before the first frame update
    private void Start()
    {
        CollectData();
    }

    private void CollectData()
    {
        emitter = GetComponent<StudioEventEmitter>();
        
        maxTime = data.timeInScene;

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

    // Update is called once per frame
    private void Update()
    {
        UpdateImpact();
        UpdateSound();

    }

    private void UpdateImpact()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= maxTime)
        {
            DestroyEffect();
        }
    }

    private void UpdateSound()
    {
        switch (_gamePlay)
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
        emitter.EventInstance.setPaused(false); // set play
    }

    private void SoundSetPause()
    {
        emitter.EventInstance.setPaused(true); // set paused
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

    private void DestroyEffect()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        switch(_gamePlay)
        {
            case true:
                {
                    Handles.Label(transform.position + Vector3.up, "GamePlay");
                    break; 
                }
            case false:
                {
                    Handles.Label(transform.position + Vector3.up, "Paused");
                    break;
                }

        }
        
    }
#endif
}
