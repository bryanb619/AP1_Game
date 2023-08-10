using UnityEngine;

public class PlayerMainMovement : MonoBehaviour
{

    private Rigidbody                           _rb;

    [SerializeField] private float              speed = 400f; 
    
    private GameState                          _gameState;
    
    private Camera                             _camera;
    [SerializeField]    private LayerMask       walkMask;
    
    // Start is called before the first frame update

    private void Awake()
    {
        GameManager.OnGameStateChanged  += OnGameStateChanged; 
        
        _rb                             = GetComponent<Rigidbody>();
        _camera                         = GetComponentInChildren<Camera>(); 
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
        float horizontal    = Input.GetAxisRaw("Horizontal");
        float vertical      = Input.GetAxisRaw("Vertical");
        
        Vector3 movement    = new Vector3(-horizontal, 0f, -vertical).normalized; // movement

        _rb.velocity = movement * (speed * Time.fixedDeltaTime);
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
