using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;


// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{
    #region Variables

    // States & AI ---------------------------------------------------->
                        public enum CompanionState      {_idle, _follow, _combat}
    [Header("State")]
                        public CompanionState           _stateAI;
                        private StateMachine            _stateMachine;            
                        internal NavMeshAgent           _companion;

                        private bool                    _gameplay;
                        public bool                     Gameplay => _gameplay;
                        private GameState               _gameState;
                        
                        private CursorGame              _cursor;

                        private PlayerMovement          _player;

    //[SerializeField]private GameObject _EPI; // Enemy presence Image
    //private mini _MiniMapCollor;

    // Movement ------------------------------------------------->

    [Header("Float")]

    [SerializeField]    private float                   speed = 0.3f;

                        private float                   t = 0f;

    [SerializeField]    private Transform               floatPos;

    [SerializeField]    private Transform               _lowPos, _highPos;

                        private Vector3                 targetPosition;

                        private float                   acceleration = 2000f;

    [Header("Positions")]
    [SerializeField]    private Transform               //_l_rTarget, // low right
                                                        _l_lTarget, // low left
                                                        _u_rTarget; // up right
                                                        //_u_lTarget; // up left

    
                        private Transform               _primeTarget, _attackPos;


                        //private enum CompanionPos   {_L_L_POS, _L_R_POS, U_L_POS, _U_R_POS}
                        private enum CompanionPos       { _L_L_POS, _U_R_POS }
   //[SerializeField]     
                        private CompanionPos            _nextPos, _currentPos;

                        private bool                    _obstructed;
                        public bool                     Obstructed => _obstructed;


    [Header("Rotation")]
    [SerializeField]    private float                   rotateDirection;
    [SerializeField]    private float                   rotateSpeed;

    [SerializeField] private float x;
    [SerializeField] private float y;
    
    // -------------------------------------------------------------------------------->

    [SerializeField] private float followSpeed;
    [SerializeField] private float attackSpeed; 


    // Materials ---------------------------------------------------------------------->
    private CompanionSpawn          point;

                        private MeshRenderer            CompanionMesh;

    [Header("Material")]
    [SerializeField]    private Material                AlphaLow, normal;

    // Combat ------------------------------------------------------------------------>

    [Header("Masks")]
    [SerializeField]    private LayerMask               _walkMask,_attackMask, _playerMask;

    [Header("Detection")]
    [SerializeField]    private float                   detectionRadius; 

                        private Camera                  mainCamera;

                        private Vector3                 direction;


    [SerializeField]    bool                            _playerDirection;


    // DEBUG ---------------------------------------------------------------------------------->

    [SerializeField]    private GameObject             testGame; 

    
    #endregion

    #region Awake
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }
    #endregion

    #region Start
    // Create the FSM
    private void Start()
    {

        //_StateAI                    = CompanionState._idle;

        _companion                      = GetComponent<NavMeshAgent>();

        //_rb = GetComponent<Rigidbody>();

        _companion.angularSpeed         = 0;
        _companion.updateRotation       = false;

        _companion.acceleration         = acceleration;

        CompanionMesh                   = GetComponent<MeshRenderer>();   

        mainCamera                      = FindObjectOfType<Camera>();

        point                           = FindObjectOfType<CompanionSpawn>();
        
        _cursor                          = FindObjectOfType<CursorGame>();

        _primeTarget                    = point.transform;


        _player                          = FindObjectOfType<PlayerMovement>();

        targetPosition                  = transform.position;

        _attackPos                      = _primeTarget;
        _currentPos                     = CompanionPos._L_L_POS;

        //startingPosition = transform.position;

        switch (_gameState)
        {
            case GameState.Paused:
                {
                    //_gameplay = false;

                    _gameState = GameState.Paused;
                    break;
                }
            case GameState.Gameplay:
                {
                    //_gameplay = true;

                    _gameState = GameState.Gameplay; 

                    break;
                }
        }

        //CompanionMesh = GetComponent<MeshRenderer>();
        //_MiniMapCollor = FindObjectOfType<mini>();
        //mainCamera = Camera.main;
        //_playerIsMoving = false;

       
        // states ------------------------------------------------------------->

        State IdleState = new State("Companion Idle",Idle);

        State FollowState = new State(("Companion Follow"), Follow); 

        State CombatState = new State("Companion combat", Combat);
            

        /*
        State RotateState = new State("",
            () => Debug.Log(""),
            RotateAroundPlayer,
            () => Debug.Log(""));

        */

        // Add the transitions


        // Idle -> Follow
        IdleState.AddTransition(
            new Transition(
                () => _stateAI == CompanionState._follow,
                FollowState));

        // idle -> Combat
        IdleState.AddTransition(
            new Transition(
                () => _stateAI == CompanionState._combat,
                CombatState));


        // Follow -> Idle
        FollowState.AddTransition(
           new Transition(
               () => _stateAI == CompanionState._idle,
               IdleState));

        // Follow -> Combat
        FollowState.AddTransition(
        new Transition(
            () => _stateAI == CompanionState._combat,
            CombatState));


        // Combat -> Idle 
        CombatState.AddTransition(
           new Transition(
               () => _stateAI == CompanionState._idle,
               IdleState));


        // Combat -> Follow

        CombatState.AddTransition(
          new Transition(
              () => _stateAI == CompanionState._follow,
              FollowState));

        // Create the state machine
        _stateMachine = new StateMachine(IdleState);
    }

    #endregion


    private void FixedUpdate()
    {
        if(_gameState == GameState.Gameplay)
        {
            Collider[] aiHits = Physics.OverlapSphere(transform.position, detectionRadius, _attackMask);

            if (aiHits.Length > 0)
            {
                _stateAI = CompanionState._combat;
            }
            
        }
        
    }

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        if(_gameState == GameState.Gameplay) 
        {
            ResumeAgent();

            if (_stateAI != CompanionState._combat)
            {
                //CheckDist();
                CheckMoveBool();
            }


            Aim();
            castHandler();
            

            Action actions = _stateMachine.Update();
            actions?.Invoke();

            

        }   
        else
        {
            AgentPause();
        }
    }
    #endregion

    private void CheckDist()
    {
        if ((_primeTarget.position - transform.position).magnitude >= 1f)
        {
            //_StateAI = CompanionState._follow;
            return; 
        }
        else
        {
            //_StateAI = CompanionState._idle;
            return; 
        }
    }
    
    #region Raycast aim mouse Update
    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // You might want to delete this line.
            // Ignore the height difference.
            direction.y = 0;

            // Make the transform look in the direction.
            transform.forward = direction;


            if (_playerDirection)
            {
                _player.transform.forward = direction;
            }
        }
        //if(Input.GetMouseButtonDown(0)) 
        //{
            
        //}
    }
    
    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _walkMask))
        {
            //companionAim.transform.position = hitInfo.point;

            // add novo ponto com nova altura
            Vector3 point = hitInfo.point;

            // somar na coordenada Y, novo valor
            point.y += 2.5F;

            //return (success: true, position: hitInfo.point);
            //Instantiate(testGame, point, Quaternion.identity); 
            return (success: true, position:point);
        }

        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void castHandler()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;


        // Perform the raycast
        if (Physics.Raycast(ray, out hit, 50F, _attackMask))
        {
            // Print the name of the object hit by the ray

            //Debug.Log(hit.transform.name);
            _cursor.UpdateCursor(CursorGame.CursorState._ATTACK);

        }
        else
        {
            _cursor.UpdateCursor(CursorGame.CursorState._NORMAL);
        }
    }

    #endregion

    #region AI Actions
    private void CheckMoveBool()
    {
        //print(_playerIsMoving); 
        
        if (_player.IsMoving && (_primeTarget.position - transform.position).magnitude >= 1f )
        {
            //_StartFollow = true;
            _stateAI = CompanionState._follow;
            return;
        }
        else if(!_player.IsMoving && (_primeTarget.position - transform.position).magnitude <= 0.8f)
        {
            // _StartFollow = false;
            _stateAI = CompanionState._idle;
            return; 
        }
        
    }

    #region Idle State
    // Chase the small enemy
    private void Idle()
    {
        StartIdle();

        FloatCompanion();

        RotateAround();
    }

    private void FloatCompanion()
    {

        t += Time.deltaTime * speed;

        float NEWY = Mathf.SmoothStep
            (_lowPos.position.y, _highPos.position.y, t);

        transform.position  = new Vector3
            (transform.position.x, NEWY, transform.position.z);


        if (t >= 1f)
        {
            t = 0f;

            Transform TEMPORARY     = _lowPos;

            _lowPos                 = _highPos;
            _highPos                = TEMPORARY;
        }
        
    }

    private void RotateAround()
    {
        transform.RotateAround(_player.transform.position,
            new Vector3(x, rotateDirection, y), rotateSpeed * Time.deltaTime);
    }
         

    private void PosUpdate()
    {

        _companion.isStopped = false;

        //Companion.SetDestination(_primeTarget.position); 
        _primeTarget = point.transform;

        _companion.SetDestination(_primeTarget.position);

        //transform.rotation = point.transform.rotation;  

        //Companion.speed = 3.4f;

        //Companion.speed = 8f;
        _companion.speed = 5f;
        //Companion.acceleration = 10f; 


        if ((_primeTarget.position - transform.position).magnitude <= 2f)
        {
            //Companion.speed = 3.5f;
            _companion.speed = followSpeed;
        }

        else if ((_primeTarget.position - transform.position).magnitude <= 0.8f)
        {
            SlowDown();
            return;
        }
        else if ((_primeTarget.position - transform.position).magnitude >= 7f)
        {
            StartCoroutine(CatchPlayer());
            return;
        }




    }
    #endregion

    #region Follow State
    private void Follow()
    {
        StartFollow();
        // Companion.Warp(transform.position); 
        //FloatCompanion();
        //StopCoroutine(floatStart());

        PosUpdate();
       // StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        
        //yield return null;

        PosUpdate();


        // add reference to player navmesh agent

        /* CODE EXAMPLE 
         * NavMeshAgent playerAgent = Player.GetComponentInChildren<NavMeshAgent>();
            Vector3 playerDestination = playerAgent.destination;
            Vector3 positionOffset = FollowRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value),
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)
        ).normalized;

        Agent.SetDestination(playerDestination + positionOffset);

        */

        //yield return null;

        yield return new WaitUntil(() => _companion.remainingDistance <= _companion.stoppingDistance);
    }


    private IEnumerator CatchPlayer()
    {
        Setlow();
        transform.position = _primeTarget.position;

        yield return new WaitForSeconds(0.2f);
        SetHigh();

        yield return new WaitForSeconds(0.2f);
        Setlow();

        yield return new WaitForSeconds(0.3f);
        SetHigh();
    }


    private void SlowDown()
    {
        _companion.velocity = _companion.velocity.normalized / 1.4f;
    }

    #endregion
    private void Combat()
    {
        StartCombat();
        //Aim();
        ObstacleCheck();
    }
    private void ObstacleCheck()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward);


        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f, _playerMask))
        {
            ChangeCourse(_currentPos);
        }

        Destination(_attackPos);

    }

    private void ChangeCourse(CompanionPos currentPos)
    {

        switch (currentPos)
        {
            case CompanionPos._L_L_POS:
                {
                    //Destination(_u_rTarget);
                    //_currentPos = currentPos;
                    _nextPos = CompanionPos._U_R_POS;

                    _currentPos = _nextPos;

                    _attackPos = _u_rTarget;
                     
                    break;
                }

            case CompanionPos._U_R_POS:
                {
                    //Destination(_l_lTarget);
                    _currentPos = currentPos;
                    _nextPos = CompanionPos._L_L_POS;

                    //_currentPos = _nextPos;

                    _attackPos = _l_lTarget;

                    break;
                }

            default: { break; }
        }
    }


    private void Destination(Transform pos)//, CompanionPos newPos)
    {

        if ((pos.position - transform.position).magnitude >= 0.1f)
        {
            _companion.SetDestination(pos.position);
            //transform.position = pos.position;
        }
        else 
        {
            _companion.velocity = Vector3.zero;
        }
        /*

        else if ((pos.position - transform.position).magnitude <= 0.2f)
        {
            _currentPos = newPos;
        }
        */
    }


    #endregion

    #region Enemy detection



    private void CheckEnemy()
    {

        //_EPI.SetActive(true);
        //_MiniMapCollor.SetCollorRed();

        //_EPI.SetActive(false);
        //_MiniMapCollor.SetCollorDefault();
        
    }
    #endregion

    
    private void StartIdle()
    {
       _companion.enabled = false;
        return;
    }
    private void StartFollow()
    {
        _companion.enabled = true;
        return;
    }

    private void StartCombat()
    {
        _companion.enabled = true;

        //_companion.radius = 0.2f;
        _companion.acceleration     = 2000f;
        _companion.speed            = attackSpeed;

        return;
    }

    #region Alpha update
    private void AlphaUpdate()
    {
        /*
        float maxDistance = 5.0f; 

        float distance = Vector3.Distance(transform.position, AlphaPoint.position);
        lerpProgres = Mathf.Clamp01(distance / maxDistance);
        CompanionMesh.material.Lerp(normal, AlphaLow, transitionSpeed);
        */
        /*
        if ((AlphaPoint.transform.position - transform.position).magnitude < minDist)
        {
            Setlow();

           
        }
        else
        {
            SetHigh();
        }
        */
    }


    private void Setlow()
    {
        // change to transparent material version
        CompanionMesh.material = AlphaLow;
    }

    private void SetHigh()
    {
        // change to normal material
        CompanionMesh.material = normal;
    }
    #endregion

    #region Gamplay State
    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                {
                    _gameplay = false;

                    _gameState = GameState.Paused;
                    
                    break;
                }
            case GameState.Gameplay:
                {
                    _gameState = GameState.Gameplay;
                    _gameplay = true;
                    break;
                }
        }
    }
    private void ResumeAgent()
    {
        if(_companion.enabled)
        {
            if (_stateAI == CompanionState._follow)
            {

                _companion.isStopped = true;
                return;
            }
            else
            {
                _companion.isStopped = false;
                return;
            }
        }
        
    }

    private void AgentPause()
    {
        if(_companion.enabled)
        {
            _companion.isStopped = true;
            _companion.velocity = Vector3.zero;
        }
       
        return;
    }
    #endregion

    #region Destroy

    public void Replace()
    {
        Destroy(gameObject); 
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    #endregion
}
