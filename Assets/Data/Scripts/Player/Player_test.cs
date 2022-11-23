using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_test : MonoBehaviour
{
    #region Rayden Movement
    /*
    // Variables

    // player speed
    [Header("Player Speed")]
    [SerializeField] private float walkingSpeed = 5.0f;

    // Player Sprint speed
    [Header("Player Sprint Speed")]
    [SerializeField] private float runningSpeed = 8.5f;
    // Jump Speed
    [Header("Player Jump Speed")]
    [SerializeField] private float jumpSpeed = 6.0f;
    // Graavity basics
    [Header("Player Gravity")]
    [SerializeField] private float gravity = 20.0f;
    // Player camera
    [Header("Player Camera")]
    [SerializeField] private Camera playerCamera;
    // Look Speed
    [SerializeField] private float lookSpeed = 2.0f;
    // Camera X limitation
    [SerializeField] private float lookXLimit = 45.0f;

    // Character Controller
    CharacterController characterController;

    // Vector motion
    Vector3 moveDirection = Vector3.zero;
    // player rotation
    float rotationX = 0;

    // can player move
    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        // Get Character controller
        characterController = GetComponent<CharacterController>();

        // Lock and hide cursor
        HideCursor();
    }

    void Update()
    {
        // detect player motion
        DetectMotion();

    }

    void DetectMotion()
    {
        // When grounded  are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);



        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;

        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    */
    #endregion

    #region New tryout movement

    [Header("Movement"), SerializeField]
    private float walkSpeed, dashSpeed, dashSpeedChangeFactor, groundDrag;
    [SerializeField]
    private float moveSpeed, maxSpeed;

    [Header("Jumping"), SerializeField]
    private float jumpForce, jumpCooldown, airMultiplier;
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check"), SerializeField]
    private float playerHeight;
    [SerializeField]
    private LayerMask whatIsGround;
    
    [SerializeField]
    private Transform orientation;
    private MovementState state;

    [Header("Slope Handling")]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    private bool readyToJump, grounded;
    private float horizontalInput, verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public bool dashing;

    private bool PlayerOnMove;
    public bool _PlayerOnMove => PlayerOnMove;
    private CompanionBehaviour _CompanionMovement;

    private float speed;

    // player health
    private const int _MaxHealth = 100;

    private int _currentHealth;
    public int CurretHealth => _currentHealth;

    [Header("Health bar")]
    public HealthBar _healthBar;
    [HideInInspector]
    public bool HealthSetAtMax;

    private enum MovementState
    {
        walking,
        dashing,
        air
    }


    private void Start()
    {


        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        _CompanionMovement = FindObjectOfType<CompanionBehaviour>();
        HealthSetAtMax = true;
        _currentHealth = 100;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        CheatCheck();

        //InMotion();


        if (state == MovementState.walking)
            rb.drag = groundDrag;
        else
            rb.drag = 0;




    }
    /*
    public bool InMotion()
    {
        return !Mathf.Approximately(rb.velocity.magnitude, 0f);
    }
    */


    private void FixedUpdate()
    {
        MovePlayer();
        PlayerSpeed();

       

        
    }

    private float desiredMoveSpeed, lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < maxSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = maxSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)   keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

        

    }

    private float speedChangeFactor;
    private Vector3 lastPosition;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

       
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }


        if(state == MovementState.walking)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }



        else
        { 
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);   
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }



    private void PlayerSpeed()
    {
        Debug.Log(speed);
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if(speed >= 5f)
        {
            //Debug.Log("true");
            //PlayerOnMove = true;
            //_CompanionMovement.PlayerInput(PlayerOnMove);

            _CompanionMovement.PlayerIsMoving = true;

        }
        else
        {
            //PlayerOnMove = false;
            // _CompanionMovement.PlayerInput(PlayerOnMove);
            _CompanionMovement.PlayerIsMoving = false;
        }

        
    }
    public void TakeDamage(int damage)
    {
        HealthSetAtMax = false;
        Debug.Log("Player Health: " + _currentHealth);
        _currentHealth -= damage;

        _healthBar.SetHealth(_currentHealth);

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

        _healthBar.SetHealth(_currentHealth);

        if (_currentHealth >= _MaxHealth)
        {
            HealthSetAtMax = true;
            // how to variables equal?
            _currentHealth = 100;
            _healthBar.SetHealth(_currentHealth);
            Debug.Log("Player health: " + _currentHealth);

        }
    }

    private void CheatCheck()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GiveHealth(20);
        }
    }

    #endregion
}