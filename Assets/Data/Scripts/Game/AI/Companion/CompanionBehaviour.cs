using System;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;


// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{

    #region Variables

    // States & AI ---------------------------------------------------->
    public enum CompanionState                          {_idle, _follow, _combat, _rotate }
                        public CompanionState           _StateAI;

                        private StateMachine            stateMachine;
                        

                        internal NavMeshAgent           _companion;

                        private bool                    _gameplay;
                        public bool                     gameplay => _gameplay;

                        private GameState               _gameState;


                        private PlayerMovement          player;

    //[SerializeField]private GameObject _EPI; // Enemy presence Image
    //private mini _MiniMapCollor;

    // Movement ------------------------------------------------->

    [SerializeField]    private float              speed = 0.3f;

                        private float               t = 0f;

    [SerializeField]    private Transform           floatPos;

    [SerializeField]    private Transform          _lowPos, _highPos;

                        private Vector3             targetPosition;

                        private float               acceleration = 2000f;

    [SerializeField]    private Transform           _l_rTarget, // low right
                                                    _l_lTarget, // low left
                                                    _u_rTarget, // up right
                                                    _u_lTarget; // up left
                                                    

                        private Transform           _primeTarget;


                        //private enum CompanionPos   {_L_L_POS, _L_R_POS, U_L_POS, _U_R_POS}
                        private enum CompanionPos   { _L_L_POS, _U_R_POS }
   [SerializeField]     private CompanionPos        _nextPos, _currentPos;

                        private bool                _obstructed;
                        public bool                 Obstructed => _obstructed;  
    

    // Materials ---------------------------------------------------------------------->
                        private CompanionSpawn      point;

                        private MeshRenderer        CompanionMesh;

    [SerializeField]    private Material            AlphaLow, normal;

    // Combat ------------------------------------------------------------------------>

    [SerializeField]    private LayerMask           _attackLayers, _playerMask;

                        private Camera              mainCamera;

    // NEW CODE
    //[SerializeField]
    //private float followsRadius = 2f;


    // 


    //[HideInInspector] public bool _playerIsMoving;

    //private bool _StartFollow;

    //private PlayerMovement _Player;
    //[Header("Mesh Configuration")]

    //[SerializeField] private MeshRenderer CompanionMesh;
    //[SerializeField] Material normal, AlphaLow;

    //[SerializeField] private Transform AlphaPoint;

    //private float minDist = 0.4f;
    // Reference to the state machine


    //private bool _enemyIS;
    //public bool canSee => _enemyIS;


    //[Range(10, 150)]
    //public float radius;
    //[Range(50, 360)]
    //public float angle;

    //public LayerMask targetMask;
    //public LayerMask obstructionMask;
    //[SerializeField] private Transform FOV;
    //public Transform EEFOV => FOV; // Enemy Editor FOV

    //private Rigidbody _rb;

    #endregion





    //private Vector3 startingPosition;
    //public float floatingMagnitude = 0.5f;



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

        _StateAI                    = CompanionState._idle;

        _companion                   = GetComponent<NavMeshAgent>();

        //_rb = GetComponent<Rigidbody>();

        _companion.angularSpeed      = 0;
        _companion.updateRotation    = false;

        _companion.acceleration      = acceleration;

        CompanionMesh               = GetComponent<MeshRenderer>();   

        mainCamera                  = FindObjectOfType<Camera>();

        point                       = FindObjectOfType<CompanionSpawn>();

        _primeTarget                = point.transform;


        player                      = FindObjectOfType<PlayerMovement>();

        targetPosition              = transform.position;

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
                () => _StateAI == CompanionState._follow,
                FollowState));

        // idle -> Combat
        IdleState.AddTransition(
            new Transition(
                () => _StateAI == CompanionState._combat,
                CombatState));


        // Follow -> Idle
        FollowState.AddTransition(
           new Transition(
               () => _StateAI == CompanionState._idle,
               IdleState));

        // Follow -> Combat
        FollowState.AddTransition(
        new Transition(
            () => _StateAI == CompanionState._combat,
            CombatState));


        // Combat -> Idle 
        CombatState.AddTransition(
           new Transition(
               () => _StateAI == CompanionState._idle,
               IdleState));


        // Combat -> Follow

        CombatState.AddTransition(
          new Transition(
              () => _StateAI == CompanionState._follow,
              FollowState));

        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    #endregion

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        if(_gameState == GameState.Gameplay) 
        {
          
            if(_StateAI != CompanionState._combat)
            {
                //CheckDist();
                CheckMoveBool();
            }


            Aim(); 
            ResumeAgent();

            Action actions = stateMachine.Update();
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
            
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _attackLayers))
        {
            //companionAim.transform.position = hitInfo.point;
            return (success: true, position: hitInfo.point);
               
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void ObstacleCheck() 
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, _playerMask))
        {
            _obstructed = true;
            ChangeCourse();
        }
        else
        {
            _obstructed = false;
        }
      
        

        
      
    }

    private void ChangeCourse()
    {
        switch (_currentPos)
        {
            // lower positions ------------------->

            // Lower left
            case CompanionPos._L_L_POS:
                {
                    //Companion.SetDestination(_u_rTarget.position);
                    //Destination(_u_rTarget);
                    _nextPos = CompanionPos._U_R_POS;

                    Destination(_u_rTarget, _nextPos);


                    break;
                }

                /*
            // lower right
            case CompanionPos._L_R_POS:
                {
                    //Companion.SetDestination(_u_lTarget.position);
                    _nextPos = CompanionPos.U_L_POS;

                    Destination(_u_lTarget, _nextPos);


                    break;
                }
            */

            //  Upper positions --------------->
            /*
            // up left
            case CompanionPos.U_L_POS:
                {
                    //Companion.SetDestination(_u_lTarget.position);
                    _nextPos = CompanionPos.U_R_POS; 

                    Destination(_u_rTarget, _nextPos);

                    break;
                }
                */
            // up left
            case CompanionPos._U_R_POS:
                {
                    //Companion.SetDestination(_l_lTarget.position);

                    _nextPos = CompanionPos._L_L_POS;

                    Destination(_l_lTarget, _nextPos);

                    break;
                }

                default: {break;}
        }



    }
        

    private void Destination(Transform pos, CompanionPos NewPos)
    {

        if ((pos.position - transform.position).magnitude >= 0.1f)
        {
            _companion.SetDestination(pos.position);
            //transform.position = pos.position;
        }

        if ((pos.position - transform.position).magnitude <= 0.2f)
        {
            _currentPos = NewPos;
        }
        
    }


    #endregion

    #region AI Actions
    private void CheckMoveBool()
    {
        //print(_playerIsMoving); 
        
        if (player.IsMoving && (_primeTarget.position - transform.position).magnitude >= 1f && _StateAI != CompanionState._combat )
        {
            //_StartFollow = true;
            _StateAI = CompanionState._follow;
            return;
        }
        else if(!player.IsMoving && (_primeTarget.position - transform.position).magnitude <= 0.8f && _StateAI != CompanionState._combat)
        {
            // _StartFollow = false;
            _StateAI = CompanionState._idle;
            return; 
        }
        
    }

    #region Idle State
    // Chase the small enemy
    private void Idle()
    {

        //Companion.updatePosition = false;
        StartCoroutine((floatStart()));

        FloatCompanion();


        //float floatingOffset = Mathf.Sin(Time.time * floatingSpeed) * floatingMagnitude;
        //Vector3 newPosition = startingPosition + new Vector3(0f, floatingOffset, 0f);
        //transform.position = newPosition;

    }


    private IEnumerator floatStart()
    {
        _companion.velocity = Vector3.zero;

        _companion.isStopped = true;

        yield return new WaitForSeconds(2f);
        
    }

    private void FloatCompanion()
    {

        t += Time.deltaTime * speed;

        float NEWY = Mathf.SmoothStep
            (_lowPos.position.y, _highPos.position.y, t);

        transform.position = new Vector3
            (transform.position.x, NEWY, transform.position.z);


        if (t >= 1f)
        {
            t = 0f;

            Transform TEMPORARY = _lowPos;

            _lowPos             = _highPos;
            _highPos            = TEMPORARY;
        }
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
        _companion.speed = 8f;
        //Companion.acceleration = 10f; 


        if ((_primeTarget.position - transform.position).magnitude <= 2f)
        {
            //Companion.speed = 3.5f;
            _companion.speed = 4f;
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

   private void Combat()
   {
        StartCombat();
        //Aim();
        ObstacleCheck();
   }


    #endregion

    #region Follow State
    private void Follow()
    {
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
        //Companion.acceleration = 8F;
        //Companion.speed = 3F;
        //Companion.velocity = Companion.velocity.normalized * 4f;

        _companion.velocity = _companion.velocity.normalized / 1.4f;
    }

    #endregion

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

    }
    private void StartFollow()
    {

    }

    private void StartCombat()
    {
        _companion.radius = 0.8f;
        _companion.acceleration = 2000f;
        _companion.speed = 4f;
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
            if (_StateAI == CompanionState._follow)
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
