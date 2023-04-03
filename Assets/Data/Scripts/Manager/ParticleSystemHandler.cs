using UnityEngine;

public class ParticleSystemHandler : MonoBehaviour
{
    private ParticleSystem[] _system;

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
                    break;
                }
            case GameState.Gameplay:
                {
                    _state = GameState.Gameplay;
                    break;
                }
        }
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
      
        _system = GetComponentsInChildren<ParticleSystem>();


        switch (_state)
        {
            case GameState.Paused:
                {
                    _state = GameState.Paused;
                    break;
                }
            case GameState.Gameplay:
                {
                    _state = GameState.Gameplay;
                    break;
                }
        }
    }


    private void FixedUpdate()
    {
        switch (_state) 
        {
            case GameState.Paused:
                {
                    Pause();
                    break; 
                }

            case GameState.Gameplay:
                {
                    Resume();
                    break;
                }
        }
    }


    private void Resume()
    {
        foreach (ParticleSystem p in _system)
        {
            p.Play();
        }
        return; 
    }

    private void Pause()
    {
        foreach (ParticleSystem p in _system)
        {
           p.Pause();
        }
        return; 
    }


    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
}
