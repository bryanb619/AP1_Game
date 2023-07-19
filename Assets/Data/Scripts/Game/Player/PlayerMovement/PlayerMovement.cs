    using System.Collections;
    using Data.Scripts.Game.AI.Companion;
    using UnityEngine;
    using UnityEngine.AI;



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement"), SerializeField]
                        internal float                  walkSpeed;
    [SerializeField]    internal float                  dashSpeed, dashSpeedChangeFactor, groundDrag, moveSpeed, maxSpeed;
    
                        // Player input type -------------------------------------------------------------------------->
                        private PlayerInputState _playerInputState;

    [Header("Keyboard/Joystick Movement")]
    [SerializeField]    private float                   controllerSpeed = 5f; 
    [SerializeField]    private float                   controllerTurnSpeed = 10f;
    
                        private Vector2                 _controllerInput;
                        private float                   _angle; 
                        private Quaternion              _targetRotation;
                        private Transform               _camTransform;
                        
    // Components ----------------------------------------------------->
                        private Camera                  _mainCamera;
                        internal NavMeshAgent           Agent;
                        
    // Player Movement ------------------------------------------------>
    [SerializeField]    private LayerMask               ignoreLayer;

                        private float                   _maxAngle = 30f;
                        private float                   _playerSpeed = 5f;
    [SerializeField]    private float                   playerAcceleration = 2000f;
    [SerializeField]    private float                   turnSpeed;
    //private bool _isMoving;

    // Click ------------------------------------------------------------>
                        private enum EffectState        { Clicked, Unclicked}
                        private EffectState             _cursorState;

    [SerializeField]    private GameObject              clickEffect;

                        private float                   _height = 0;
    [SerializeField]    private float                   heightOffset;

    // target & direction ------------------------------------------------->
                        private Vector3                 _targetPosition;
                        private Vector3                 _direction;

    // Enemy detection ----------------------------------------------------->
    [SerializeField]    private float                   maxRange;
    
    [Header("Ground Check"), SerializeField]
                        private float                   playerHeight;
    [SerializeField]    private LayerMask               whatIsGround;
    
    [SerializeField]    private Transform               orientation;
                        private MovementState           _state;

    [SerializeField]    private LayerMask               aiLayer;


    [Header("Slope Handling")]
    [HideInInspector]   public bool                    canMove; 
                        private float                  _maxSlopeAngle;
                        private RaycastHit             _slopeHit;
                        private bool                   _exitingSlope;

                        private bool                   _grounded;
                        private float                  _horizontalInput, _verticalInput;
                        Vector3                        _moveDirection;
                        Rigidbody                      _rb;
                        public bool                    dashing;


                        private bool                   _playerOnMove;
                        public bool                    PlayerOnMove => _playerOnMove;
                        private CompanionBehaviour     _companionMovement;
                        private EnemyBehaviour         _enemyHealth;
                     
                        private RestartMenu            _restartMenu;
                     
                        private float                   _speed;


    [Header("Dash Explosion"), SerializeField]
                        private float                   explosionForce = 10f;
    //[SerializeField] private float explosionDamage = 20f;  
                     

    [Header("References"), SerializeField]
                        private MeshRenderer            playerMaterial;
    //[SerializeField] private Camera mainCamera; 
    [SerializeField]    private GameObject              effect;
    [SerializeField]    private LayerMask               seeThroughLayer; 
    [SerializeField]    private GameObject              playerMesh;

                        private SkinnedMeshRenderer[]   _skinnedMeshRenderers;
                        private Color[]                 _originalColors;
                        private bool                    _gamePlay;
                        
                        private PlayerInput            _playerInput;

                        private NavMeshPath             _path;
                        //private float elapsed = 0.0f;
                     
                        //internal NavMeshAgent agent;
                     
                        private LayerMask               _layerMask;
                     
                        public float                    acceleration = 2f;
                        public float                    deceleration = 60f;
                     
                        public float                    closeEnoughMeters = 3f;
                     
                        private Vector3                 _currentDest;
                     
                        private bool                    _isMoving; 
                        public bool                     IsMoving => _isMoving;
                        
                        
                        
    //  Player controller WASD MOVEMENT ------------------------------------------------------------------------------->
    
    
    private Vector3 lastPosition;

    private enum MovementState
    {
        Walking,
        Dashing
    }

    private void Awake()
    {
        GameManager.OnGameStateChanged      += GameManager_OnGameStateChanged;
        
        PlayerControl.OnPlayerInputChanged  += PlayerController_OnPlayerInputChanged; 
        
        this.enabled = true;

        // Make a list of all SkinnedMeshRender in the character
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Create an array to store the original colors
        _originalColors = new Color[_skinnedMeshRenderers.Length];
        for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
        {
            _originalColors[i]   = _skinnedMeshRenderers[i].material.color;
        }

        canMove                 = true;

        _rb                      = GetComponent<Rigidbody>();
        _rb.freezeRotation       = true;

        Agent                   = GetComponent<NavMeshAgent>();
        Agent.updateRotation    = false;

        _companionMovement      = FindObjectOfType<CompanionBehaviour>();
        _restartMenu             = FindObjectOfType<RestartMenu>();
        

        _path                    = new NavMeshPath();
        //elapsed = 0.0f;

        _currentDest             = transform.position;
        
        Components();
        PlayerProfile();
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

        //throw new NotImplementedException();
    }

    private void PlayerController_OnPlayerInputChanged(PlayerInput input)
    {
        switch (input)
        {
            case PlayerInput.Keyboard:
            {
                HandleKeyBoard(); 
                break;
            }
            case PlayerInput.Mouse:
            {
                HandleMouse(); 
                break;
            }

        }
    }

    private void HandleKeyBoard()
    {
        _playerInput = PlayerInput.Keyboard;
        Agent.enabled = false;

#if UNITY_EDITOR
        Debug.Log("Keyboard");
#endif
    }
    private void HandleMouse()
    {
        
        _playerInput = PlayerInput.Mouse;
        Agent.enabled = true;
        
#if UNITY_EDITOR
        //Debug.Log("mouse");
#endif
    }
    
    private void Start()
    {
        
    }


    private void Components()
    {
        Agent                   = GetComponent<NavMeshAgent>();
        _mainCamera             = FindObjectOfType<Camera>();
    }

    private void PlayerProfile()
    {
        Agent.speed             = _playerSpeed;
        Agent.acceleration      = playerAcceleration;


        Agent.updateRotation    = false;
        Agent.angularSpeed      = 0;

        _cursorState            = EffectState.Unclicked;

    }

    private void Update()
    {    

        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        EnemiesAround();
        Aim();

        if (_state == MovementState.Walking /*|| state == MovementState.crouching*/)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0;
    }
    
    private void FixedUpdate()
    {

        MainInputType(); 
        MovePlayer();
        PlayerSpeed();
    }

    private void MainInputType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            switch (Agent.enabled)
            {
                case true:
                    {
                        Agent.enabled = false;
                        break;
                    }
                case false:
                    {
                        Agent.enabled = true;
                        break;
                    }
            }
        }

        Vector3 position = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _rb.velocity = position * (_playerSpeed * Time.deltaTime);

    }
    
    
    #region Raycast aim mouse Update

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        
        if (!success) return;
        
        // Calculate the direction
        var transform1 = transform;
        var direction = position - transform1.position;

        // You might want to delete this line.
        // Ignore the height difference.
        direction.y = 0;

        // Make the transform look in the direction.
        transform1.forward = direction;
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
            
        _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
        // subtrair altura das balas do chao pela altura da camara
            
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, whatIsGround))
        {
            //companionAim.transform.position = hitInfo.point;

            // add novo ponto com nova altura
            Vector3 point = hitInfo.point;

            // somar na coordenada Y, novo valor
            point.y += 2.5F;

            //return (success: true, position: hitInfo.point);
            //Instantiate(testGame, point, Quaternion.identity); 
            return (success: true, position: point);
        }

        else
        {
            return (success: false, position: Vector3.zero);
        }
    }
    
    #endregion
    
    private float _desiredMoveSpeed, _lastDesiredMoveSpeed;
    private MovementState _lastState;
    private bool _keepMomentum;
    private void StateHandler()
    {
        if (dashing)
        {
            _state = MovementState.Dashing;
            _desiredMoveSpeed = dashSpeed;
            _speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (_grounded)
        {
            _state = MovementState.Walking;
            _desiredMoveSpeed = walkSpeed;
        }

        bool desiredMoveSpeedHasChanged = _desiredMoveSpeed != _lastDesiredMoveSpeed;
        if(_lastState == MovementState.Dashing)   _keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if(_keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = _desiredMoveSpeed;
            }
        }

        _lastDesiredMoveSpeed = _desiredMoveSpeed;
        _lastState = _state;
    }

    private float _speedChangeFactor;
    private Vector3 _lastPosition;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(_desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = _speedChangeFactor;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = _desiredMoveSpeed;
        _speedChangeFactor = 1f;
        _keepMomentum = false;
    }


    private void MyInput()
    {
        switch(_gamePlay)
        {
            case true:
                {
                    
                    MoveAiInput();
                    NewDirection();
                    break;
                }
                //KeyboardOnMove();
                    
                   // if (Math.Abs(input.x) < 1 Math.Abs(input.y) < 1) { return;}
                    
            case false:
                {
                    if (Agent.enabled)
                    {
                        Agent.velocity = Vector3.zero;
                        Agent.isStopped = true;
                    }
                      
                    break;

                }
        }
    }


    #region Player Movement Controller/Keyboard
    
    private void inputKC()
    {

        
        
        CalculateDirection();
        KcRotate();
        MoveKC();
    
    }


    /// <summary>
    /// Input from horizontal and vertical axis
    /// </summary>
    private void CalculateDirection()
    {
        
    }

    private void KcRotate()
    {
        
    }
    
    private void MoveKC()
    {
      
    }
    



    #endregion

    #region Player Movement AI
    private void MoveAiInput()
    {
       
        if (Input.GetMouseButtonDown(1))
        {
            Destination(true);
        }

        else if (Input.GetMouseButton(1))
        {
            _cursorState = EffectState.Clicked;
        }
        else
        {
            _cursorState = EffectState.Unclicked;
        } 
        
    }
    
    
    private void NewDirection()
    {
        if (Agent.enabled)
        {
            if (_isMoving)
            {
                // set direction
                _direction = _targetPosition - transform.position;

                // Ignore Y position 
                _direction.y = 0;

                // Rotate player towards the target position
                Quaternion targetRotation = Quaternion.LookRotation(_direction);

                // apply rotation with desired speed
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            }

            if (_cursorState == EffectState.Clicked)
            {
                Destination(false);
            }

            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Agent.velocity = Vector3.zero;
                Agent.isStopped = true;
            }

        }
    }

    private void Destination(bool input)
    {
        RaycastHit hit;

        Vector3 destination;

        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 50, ~ignoreLayer))
        {
            if (Agent.enabled)
            {
                Vector3 newDirection = (hit.point - transform.position).normalized;
                float angle = Vector3.Angle(_direction, newDirection);

                destination = hit.point;

                _direction = newDirection;

                NavMeshHit navHit;

                if (NavMesh.SamplePosition(destination, out navHit, maxRange, NavMesh.AllAreas))
                {

                    if (angle > _maxAngle && Agent.remainingDistance >= 0.2f)
                    {
                        Agent.velocity = Vector3.zero;

                        _targetPosition = navHit.position;

                        //_isMoving = false;
                    }
                    else
                    {
                        _isMoving = true;
                        Agent.isStopped = false;
                        _targetPosition = (navHit.position);

                        Agent.SetDestination(navHit.position);
                    }


                }

                if (input)
                {
                    EffectSpawn(hit);
                }
            }        
        }
    }


    private void EffectSpawn(RaycastHit hit)
    {
        _height = hit.point.y + heightOffset;

        GameObject spawnedObject = Instantiate(clickEffect, hit.point, clickEffect.transform.rotation);
        spawnedObject.transform.position = new Vector3
            (spawnedObject.transform.position.x,
            _height,
            spawnedObject.transform.position.z);
    }
    #endregion
    
    private void MovePlayer()
    {
        if (_state == MovementState.Dashing) return;

        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }


        if(_state == MovementState.Walking)
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        _rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(OnSlope() && !_exitingSlope)
        {
            if (_rb.velocity.magnitude > moveSpeed)
                _rb.velocity = _rb.velocity.normalized * moveSpeed;
        }

        else
        { 
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
    
    private void PlayerSpeed()
    {
        float speed; 
        
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
        

        if (speed >= 0.5f)
        {
           _isMoving = true;
           //print("walk");
            //_CompanionMovement._playerIsMoving = true;

        }
        else
        {
            _isMoving = false;
            //_CompanionMovement._playerIsMoving = false;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == "Enemy" && _state == MovementState.Dashing)
        {
            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            Debug.Log("Dash damage");

            _enemyHealth = other.gameObject.GetComponent<EnemyBehaviour>();
            
            //direction and distance of the push
            Vector3 pushDirection = other.transform.position - transform.position;
            float pushDistance = pushDirection.magnitude;
            pushDirection.y = 0;
            pushDirection.Normalize();

            Debug.Log("Push Direction: " + pushDirection);
            _rb.AddForce(-pushDirection * explosionForce, ForceMode.Impulse);
            enemyRb.AddForce(pushDirection * explosionForce * 0.05f, ForceMode.Impulse);

            //explosion damage
            //enemyHealth.TakeDamage((int)explosionDamage, WeaponType.Dash);
        }
    }

    private void EnemiesAround()
    {
        //Collider[] aiHits = Physics.OverlapSphere(transform.position, 30f, aiLayer);

    }
    
    /// <summary>
    /// Stop the player from moving
    /// </summary>
    public void StopMovement()
    {
        // TODO - temp solution to stop the player from moving ( correct way is to call the type of movement enabled)
        
        if (Agent.enabled)
        {
            Agent.isStopped = true;
        }
            
        this.enabled = false;
    }

    /// <summary>
    /// Restart the player movement
    /// </summary>
    public void RestartMovement()
    {
        if (Agent.enabled)
        {
            Agent.isStopped = false;
        }
       
        this.enabled = true;
    }


    #region Enumerators

    internal IEnumerator VisualFeedbackDamage()
    {

        //Change the material colors to red
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        // Change the material colors back to their original colors
        for (int j = 0; j < _skinnedMeshRenderers.Length; j++)
        {
            _skinnedMeshRenderers[j].material.color = _originalColors[j];
        }
    }

    internal IEnumerator VisualFeedbackHeal()
    {
        for (int i = 0; i <= 2; i++)
        {
            // Set the material colors to green
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in _skinnedMeshRenderers)
            {
                skinnedMeshRenderer.material.color = Color.green;
            }

            yield return new WaitForSeconds(0.2f);

            // Change the material colors back to their original colors
            for (int j = 0; j < _skinnedMeshRenderers.Length; j++)
            {
                _skinnedMeshRenderers[j].material.color = _originalColors[j];
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged          -= GameManager_OnGameStateChanged;
        PlayerControl.OnPlayerInputChanged      -= PlayerController_OnPlayerInputChanged;
    }
}