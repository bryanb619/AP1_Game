using UnityEngine;
using UnityEngine.VFX;

public class ParticleSystemHandler : MonoBehaviour
{
    private ParticleSystem[]    _system;
    private VisualEffect        _effect;
    
                                private float _time;    
    [SerializeField]            private float destroyTime; 

    [SerializeField]            private bool getParticleInChild;  
    
                                private GameState _state; 

    

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    #region Gamplay State

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                {
                    _state = GameState.Paused;
                    Pause();
                    break;
                }
            case GameState.Gameplay:
                {
                    _state = GameState.Gameplay;
                    Resume();
                    break;
                }
        }
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {

        if (getParticleInChild)
        {
            _system = GetComponentsInChildren<ParticleSystem>();
        }

        switch (_state)
        {
            case GameState.Paused:
                {
                    _state = GameState.Paused;
                    Pause();
                    break;
                }
            case GameState.Gameplay:
                {
                    _state = GameState.Gameplay;
                    Resume();
                    break;
                }
        }
    }


    private void Update()
    {
        switch (_state) 
        {
           case GameState.Gameplay:
           {
               destroyTime += Time.deltaTime;
               
               if (_time >= destroyTime)
               {
                   Destroy(gameObject);
               }
               break;
            }
           default:{break;}
        }
    }


    private void Resume()
    {
        foreach (ParticleSystem p in _system)
        {
            p.Play();
        }
    }

    private void Pause()
    {
        foreach (ParticleSystem p in _system)
        {
           p.Pause();
        }
  
    }


    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
}
