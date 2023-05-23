using UnityEngine;

public class PlayerMainMovement : MonoBehaviour
{

    private Rigidbody                           _rb;

    [SerializeField] private float              speed; 
    
    // Start is called before the first frame update

    private void Awake()
    {
        GameManager.OnGameStateChanged  += OnGameStateChanged; 
        
        _rb                             = GetComponent<Rigidbody>();
        //_rb.constraints                 = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
        
        
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
        {
            
            
            
        }
    }
    
    private void FixedUpdate()
    {
        Vector3 position = 
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        _rb.velocity = position * speed;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;

    }
}
