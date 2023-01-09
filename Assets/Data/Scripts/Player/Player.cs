using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //private const float MAX_ANGULAR_ACCELERATION = 360.0f;

    private float MAX_FORWARD_ACCELERATION = 30.0f;
    private const float MAX_BACKWARD_ACCELERATION = 8.0f;
    private const float MAX_STRAFE_ACCELERATION = 12.0f;
    private const float JUMP_ACCELERATION = 410.0f;
    private const float GRAVITY_ACCELERATION = 25.0f;

    
    private const float MAX_BACKWARD_VELOCITY = 3.0f; //2
    private const float MAX_STRAFE_VELOCITY = 3.0f; //2
    private const float MAX_FALL_VELOCITY = 50.0f;

    private float SprintMaxSpeed = 8f;

    private const float MAX_BACKWARD_VELOCITY_SPRINT = 4.0f;

    private float sprintStrafeSpeed = 5f;

    private CharacterController _controller;

    [Header("Crouch Settings")]
    [SerializeField]
    private float standingHeight = 1.8f;

    [SerializeField]
    private float crouchingHeight = 1.25f;

    private bool _isCrouching;
    public bool IsCrouching => _isCrouching;

    private Vector3 _acceleration;
    private Vector3 _velocity;
    private bool _jump;
    private bool _sprint;
    [HideInInspector]
    public bool HealthSetAtMax;
    private float _velocityMultiplier;

    // player health
    private const int _MaxHealth = 100;

    private int _currentHealth;
    public int CurretHealth => _currentHealth;


    //[Header("Health bar")]
   // public HealthBar _healthBar;


    [System.Serializable]
    public struct SaveData
    {
        public Vector3 position;
        public Quaternion rotation;

        public int health;
    }


    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _controller = GetComponent<CharacterController>();

        _acceleration = Vector3.zero;
        _velocity = Vector3.zero;

        _velocityMultiplier = 1.0f;

    }

    void Start()
    {
        HealthSetAtMax = true;
        _currentHealth = 100;


        _jump = false;
        //_sprint = false;

    }



    private void Update()
    {
        //CheckForJump();
        //CameraRotation();
        CheckForCrouch();
        //CheckForSprint();
        CheckForDamageCheat();
        //PlayerRotation();
    }



    private void FixedUpdate()
    {
        UpdateAcceleration();
        UpdateVelocity();
        UpdatePosition();
    }


    
    private void CheckForJump()
    {
        if (_controller.isGrounded && Input.GetButtonDown("Jump"))
            _jump = true;

    }
    

    /*
    private void CheckForSprint()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _sprint = true;
        }
        else
        {
            _sprint = false;
        }
    }
    */

    //
    private void CheckForCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _isCrouching = true;

        }
        else
        {
            _isCrouching = false;
        }

        if (_isCrouching == true)
        {
            _controller.height = crouchingHeight;
        }
        else
        {
            _controller.height = standingHeight;
        }
    }
    

    private void UpdateAcceleration()
    {
        _acceleration.z = Input.GetAxis("Vertical");
        _acceleration.z *= (_acceleration.z > 0 ? MAX_FORWARD_ACCELERATION : MAX_BACKWARD_ACCELERATION) * _velocityMultiplier;

        _acceleration.x = Input.GetAxis("Horizontal") * MAX_STRAFE_ACCELERATION * _velocityMultiplier;

        if (_jump)
        {
            _acceleration.y = JUMP_ACCELERATION;
            _jump = false;
        }
        else if (_controller.isGrounded)
        {
            _acceleration.y = 0f;
            _velocity.y = 0f;
        }
        else
            _acceleration.y = -GRAVITY_ACCELERATION;
    }

    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        _velocity.y = (_acceleration.y == 0f) ? -0.1f : Mathf.Max(-MAX_FALL_VELOCITY, _velocity.y);

        _velocity.z = Mathf.Abs(_velocity.z) < 0.01f ? 0f : _velocity.z;

        _velocity.x = Mathf.Abs(_velocity.x) < 0.01f ? 0f : _velocity.x;
    }

    private void UpdatePosition()
    {
        Vector3 motion = transform.TransformVector(_velocity * Time.fixedDeltaTime);

        _controller.Move(motion);
    }


    /*
    private void CameraRotation()
    {


        rotation.x += Input.GetAxis("Mouse_X") * lookSpeed;

        rotation.y += -Input.GetAxis("Mouse_Y") * lookSpeed;

        rotation.y = Mathf.Clamp(rotation.y, -LookUp, LookDown);
        playerCameraParent.localRotation = Quaternion.Euler(rotation.y, 0, 0);


        transform.eulerAngles = new Vector2(0, rotation.x);

        //Debug.Log("Your rotation in Y axis is: " + rotation.y);


    }
    */

    private void CheckForDamageCheat()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        HealthSetAtMax = false;
        Debug.Log("Player Health: " + _currentHealth);
        _currentHealth -= damage;

        //_healthBar.SetHealth(_currentHealth);

        if (_currentHealth <= 0)
        {
            Debug.Log("DEAD");
            //SceneManager.LoadScene("Restart");
            //RestarMenu.SetActive(true);

            //Time.timeScale = 0f;
        }
    }

    public void GiveHealth(int _health)
    {
        // Debug.Log("+ 15 health");
        _currentHealth += _health;

        Debug.Log("Player health is: " + _currentHealth);

        //_healthBar.SetHealth(_currentHealth);

        if (_currentHealth >= _MaxHealth)
        {
            HealthSetAtMax = true;
            // how to variables equal?
            _currentHealth = 100;
            //_healthBar.SetHealth(_currentHealth);
            Debug.Log("Player health: " + _currentHealth);

        }
    }

    /*
    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.position = transform.position;
        saveData.rotation = transform.rotation;

        saveData.health = _currentHealth;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        _controller.enabled = false;

        transform.position = saveData.position;
        transform.rotation = saveData.rotation;

        _currentHealth = saveData.health;
        _healthBar.SetHealth(_currentHealth);

        _controller.enabled = true;
    }
    */

}
