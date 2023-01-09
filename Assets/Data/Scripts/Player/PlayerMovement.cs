using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement"), SerializeField]
                     internal float walkSpeed, dashSpeed, dashSpeedChangeFactor, groundDrag, moveSpeed, maxSpeed;

    [Header("Jumping"), SerializeField]
                     private float jumpForce, jumpCooldown, airMultiplier;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Crouching")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
                     private float crouchModifier = 4;
                     private bool crouching = false;

    [Header("Ground Check"), SerializeField]
                     private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    
    [SerializeField] private Transform orientation;
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

    [HideInInspector]public bool CanMove; 

                     private bool PlayerOnMove;
                     public bool _PlayerOnMove => PlayerOnMove;
                     private CompanionBehaviour _CompanionMovement;
                     private EnemyBehaviour enemyHealth;
                     
                     private RestartMenu restartMenu;
                     
                     private float speed;
                     
                     // player health
                     private const int _MaxHealth = 100;
                     
                     internal int _currentHealth;
                     public int CurretHealth => _currentHealth;

    [Header("Health bar")]
                     public HealthBar _healthBar;
    [HideInInspector]
                     public bool HealthSetAtMax;

    [Header("Dash Explosion"), SerializeField]
                     private float explosionForce = 1000f;
    [SerializeField] private int explosionDamage = 200;
    
                     public int shield = 0;

    private enum MovementState
    {
        walking,
        dashing,
        crouching,
        air
    }


    private void Start()
    {
        this.enabled = true;

        CanMove = true;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        _CompanionMovement = FindObjectOfType<CompanionBehaviour>();
        restartMenu = FindObjectOfType<RestartMenu>();
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


        if (state == MovementState.walking || state == MovementState.crouching)
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

        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = walkSpeed - crouchModifier;
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
        if(CanMove == true)
        {
          
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");


            if (Input.GetKey(jumpKey) && readyToJump && grounded && !crouching)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }

            if (Input.GetKeyDown(crouchKey) && grounded)
            {
                crouching = true;
            }
            else if (Input.GetKeyUp(crouchKey))
            {
                crouching = false;
            }
        }
        else if(CanMove == false)
        {
          
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

        if(state == MovementState.crouching)
            rb.AddForce(moveDirection.normalized * (moveSpeed - crouchModifier) * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * (moveSpeed - crouchModifier) * 10f * airMultiplier, ForceMode.Force);

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
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if(speed >= 5f)
        {
            _CompanionMovement._playerIsMoving = true;

        }
        else if (speed >= 12)
        {

            _CompanionMovement._playerIsMoving = true;
        }
        else
        {
            _CompanionMovement._playerIsMoving = false;
        }
    }

    void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == "Enemy" && state == MovementState.dashing)
        {
            enemyHealth = other.gameObject.GetComponent<EnemyBehaviour>();

            //direction and distance of the push
            Vector3 pushDirection = other.transform.position - transform.position;
            float pushDistance = pushDirection.magnitude;
            pushDirection.Normalize();

            rb.AddExplosionForce(15f, new Vector3(transform.position.x, transform.position.y, pushDirection.z), 5f, 3f);

            other.gameObject.transform.position += pushDirection * (explosionForce / pushDistance) * Time.deltaTime;

            //explosion damage
            enemyHealth.TakeDamage(explosionDamage);
        }
    }

    #region Cheats
    public void TakeDamage(int damage)
    {
        HealthSetAtMax = false;
        _currentHealth -= (damage - shield);

        _healthBar.SetHealth(_currentHealth);
        Debug.Log("Player Health: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            Debug.Log("DEAD");
            restartMenu.LoadRestart();
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