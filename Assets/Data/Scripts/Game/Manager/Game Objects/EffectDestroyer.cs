using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    [SerializeField] private float _destroyTime;
    private GameState _gameState; 

    private float elapsed = 0f;
    // Start is called before the first frame update

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChange; 
    }
   
    // Update is called once per frame
    void Update()
    {
        switch (_gameState) 
        {
            case GameState.Gameplay:
                {
                    elapsed += Time.deltaTime;

                    if (elapsed >= _destroyTime) { Destroy(gameObject); }
                    
                    break; 
                }
            case GameState.Paused: 
                {
                    break; 
                }
        }
    }

    private void GameManager_OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                {
                    _gameState = GameState.Gameplay;
                    break;
                }
            case GameState.Paused:
                {
                    _gameState = GameState.Paused;
                    break;
                }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChange;
    }
}
