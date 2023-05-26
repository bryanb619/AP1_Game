using UnityEngine;

public class PlayerMainMovement : MonoBehaviour
{

    private Rigidbody                           _rb;

    [SerializeField] private float              speed; 
    
    private GameState                          _gameState;
    
    // Start is called before the first frame update

    private void Awake()
    {
        GameManager.OnGameStateChanged  += OnGameStateChanged; 
        
        _rb                             = GetComponent<Rigidbody>();
        //_rb.constraints               = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;

    }

    private void OnGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
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
    
    private void FixedUpdate()
    {

        
       
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gameState == GameState.Gameplay)
        {
            
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;

    }
}
