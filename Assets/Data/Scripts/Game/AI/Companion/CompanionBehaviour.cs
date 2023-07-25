using System;
using System.Collections;
using LibGameAI.FSMs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;



namespace Data.Scripts.Game.AI.Companion
{ 
    // The script that controls an agent using an FSM
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class CompanionBehaviour : MonoBehaviour
    {
        #region Variables

        // States & AI ---------------------------------------------------->
        public enum CompanionState
        {
            Idle,
            Follow,
            Combat
        }

        [Header("State")] 
        public CompanionState stateAi;
        private StateMachine _stateMachine;
        internal NavMeshAgent Companion;

        private bool _gameplay;
        public bool Gameplay => _gameplay;
        private GameState _gameState;

        private CursorGame _cursor;

        private PlayerMovement _player;

        //[SerializeField]private GameObject _EPI; // Enemy presence Image
        //private mini _MiniMapCollor;

        // Movement ------------------------------------------------->

        [Header("Float")] 
        [SerializeField] private float speed = 0.3f;

        private float _t = 0f;

        [SerializeField] private Transform floatPos;

        [SerializeField] private Transform lowPos;
        [SerializeField] private Transform highPos;

        private Vector3 _targetPosition;

        private float _acceleration = 2000f;

        [Header("Positions")] 
        [SerializeField] private Transform //_l_rTarget, // low right
            lLTarget; // up right

        [FormerlySerializedAs("_u_rTarget")]
        [Header("Positions")] 
        [SerializeField] private Transform //_l_rTarget, // low right
            // low left
            uRTarget; // up right
        //_u_lTarget; // up left


        private Transform _primeTarget, _attackPos;


        //private enum CompanionPos   {_L_L_POS, _L_R_POS, U_L_POS, _U_R_POS}
        private enum CompanionPos { LLPos, URPos}
        
        [SerializeField] private CompanionPos nextPos;
        [SerializeField] private CompanionPos currentPos;

        private bool _changePos;
        public bool ChangePos => _changePos;

        private bool _enemiesInRange; 


        [Header("Rotation")] 
        [SerializeField] private float rotateDirection;
        [SerializeField] private float rotateSpeed;

        [SerializeField] private float x;
        [SerializeField] private float y;

        // Movement -------------------------------------------------------------------------------->

        [SerializeField] private float followSpeed;
        [SerializeField] private float attackSpeed;


        // Materials ---------------------------------------------------------------------->
        private CompanionSpawn _point;

        private MeshRenderer _companionMesh;

 
        [Header("Material")]
        [SerializeField] private Material alphaLow;
        [SerializeField] private Material normal;

        // Combat ------------------------------------------------------------------------>


        [Header("Masks")] 
        [SerializeField] private LayerMask walkMask;
        [SerializeField] private LayerMask attackMask;
        [SerializeField] private LayerMask playerMask;

        [Header("Detection")] [SerializeField] private float detectionRadius;

        private Camera _mainCamera;

        private Vector3 _direction;

        [SerializeField] bool playerDirection;


        // DEBUG ---------------------------------------------------------------------------------->

        [SerializeField] private GameObject testGame;


        #endregion

        #region Awake

        private void Awake()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;

            GetComponents();
        
        }

        #endregion

        private void GetComponents()
        {
            //_StateAI                    = CompanionState._idle;

            Companion = GetComponent<NavMeshAgent>();

            //_rb = GetComponent<Rigidbody>();

            Companion.angularSpeed = 0;
            Companion.updateRotation = false;

            Companion.acceleration = _acceleration;

            _companionMesh = GetComponent<MeshRenderer>();
            
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 

            _point = FindObjectOfType<CompanionSpawn>();

            _cursor = FindObjectOfType<CursorGame>();

            _primeTarget = _point.transform;


            _player = FindObjectOfType<PlayerMovement>();

            _targetPosition = transform.position;

            _attackPos = _primeTarget;
            currentPos = CompanionPos.LLPos;
            nextPos = CompanionPos.URPos;

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Start

        // Create the FSM
        private void Start()
        {
            
            // states
            
            var idleState = new State("Companion Idle", Idle);

            var followState = new State(("Companion Follow"), Follow);

            var combatState = new State("Companion combat", Combat);
        
            //  Transitions  ------------------------------------------------------------------------------------------>

            // Idle --> Follow, Combat 
            
            // Idle -> Follow
            idleState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Follow,
                    followState));

            // idle -> Combat
            idleState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Combat,
                    combatState));
            
            //---------------------------------->
            
            // Follow --> Idle, Combat

            // Follow -> Idle
            followState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Idle,
                    idleState));

            // Follow -> Combat
            followState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Combat,
                    combatState));

            //----------------------------------->
            
            // Combat --> Follow, Idle
            
            // Combat -> Follow
            combatState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Follow,
                    followState));

            // Combat -> Idle 
            combatState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Idle,
                    idleState));

            //----------------------------------->
            
            // Idle --> Follow, Combat
            
            // Idle -> Follow
            idleState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Follow,
                    followState));
            
            // idle -> Combat
            idleState.AddTransition(
                new Transition(
                    () => stateAi == CompanionState.Combat,
                    combatState));
            //---------------------------------->
            
            // Create the state machine
            _stateMachine = new StateMachine(idleState);
        }

        #endregion
    
        #region Update

        // Request actions to the FSM and perform them
        private void Update()
        {
            if (_gameState == GameState.Gameplay)
            {
                ResumeAgent();
            
                Aim();
                
                StartCoroutine(EnemyCheckRoutine());

                if (!_enemiesInRange)
                {
                    CheckMoveBool();
                }
            
                var actions = _stateMachine.Update();
                actions?.Invoke();
            
            }
            else
            {
                AgentPause();
            }
        }

        #endregion
    
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


            if (playerDirection)
            {
                _player.transform.forward = direction;
            }
        }

        private (bool success, Vector3 position) GetMousePosition()
        {
            
            _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // subtrair altura das balas do chao pela altura da camara
            
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, walkMask))
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
    
        #region Enemy detection
        private IEnumerator EnemyCheckRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);

            while (true)
            {
                yield return wait;
                EnemyCheck();
            }
        }

        private void EnemyCheck()
        {
            Collider[] aiHits = Physics.OverlapSphere(transform.position, detectionRadius, attackMask);

            if (aiHits.Length > 0)
            {
                _enemiesInRange = true;
            }
            else
            {
                _enemiesInRange = false; 
            }
        }
        #endregion

        #region AI Actions

        private void CheckMoveBool()
        {
            //print(_playerIsMoving); 

            if (_player.IsMoving && (_primeTarget.position - transform.position).magnitude >= 1f)
            {
                //_StartFollow = true;
                stateAi = CompanionState.Follow;
            
            }
        
            if (!_player.IsMoving && (_primeTarget.position - transform.position).magnitude <= 0.8f)
            {
                // _StartFollow = false;
                stateAi = CompanionState.Idle;
            
            }

        }

        #region Idle State

        // idle
        private void Idle()
        {
            StartIdle();

            FloatCompanion();

            RotateAround();
        }

        private void FloatCompanion()
        {

            _t += Time.deltaTime * speed;

            float newy = Mathf.SmoothStep
                (lowPos.position.y, highPos.position.y, _t);

            transform.position = new Vector3
                (transform.position.x, newy, transform.position.z);


            if (_t >= 1f)
            {
                _t = 0f;

                Transform temporary = lowPos;

                lowPos = highPos;
                highPos = temporary;
            }

        }

        private void RotateAround()
        {
            transform.RotateAround(_player.transform.position,
                new Vector3(x, rotateDirection, y), rotateSpeed * Time.deltaTime);
        }

        #endregion

        #region Follow State

        private void Follow()
        {
            StartFollow();
            PosUpdate(_primeTarget);
     
        }
    
        private void PosUpdate(Transform pos)
        {

            Companion.isStopped = false;
        
            _primeTarget = _point.transform;
            Companion.SetDestination(pos.position);
        
            Companion.speed = 7f;



            if ((pos.position - transform.position).magnitude <= 2f)
            {
                Companion.speed = followSpeed;
            }

            else switch ((pos.position - transform.position).magnitude)
            {
                case <= 0.8f:
                    SlowDown();
                    return;
                case >= 5f:
                    StartCoroutine(CatchPlayer(pos));
                    return;
            }
        }

    
        private IEnumerator CatchPlayer(Transform pos)
        {
            Setlow();
            transform.position = pos.position;

            yield return new WaitForSeconds(0.2f);
            SetHigh();

            yield return new WaitForSeconds(0.2f);
            Setlow();

            yield return new WaitForSeconds(0.3f);
            SetHigh();
        }


        private void SlowDown()
        {
            Companion.velocity = Companion.velocity.normalized / 1.4f;
        }


 
        #endregion

        #region Combat

        private void Combat()
        {
            StartCombat();
            Aim();
            ObstacleCheck();
        }

        private void ObstacleCheck()
        {
            RaycastHit hit;

            Debug.DrawRay(transform.position, transform.forward, Color.magenta);


            if (Physics.Raycast
                    (transform.position, transform.forward, out hit, 50f, playerMask)
                && _changePos)
            {
            
                _changePos = false;

            }
        
            if (!_changePos)
            {
                Destination();
            }
            else
            {
                PersistantPosCheck();
            }

       


        }

        private void Destination()
        {
        
            if (currentPos == CompanionPos.LLPos)
            {
                nextPos = CompanionPos.URPos;
            }
            else if (currentPos == CompanionPos.URPos)
            {
                nextPos = CompanionPos.LLPos;

            }
            NewPosition(nextPos);
        
            /*

        _companion.speed = 8f; 

        if (_companion.remainingDistance <= 0.5)
        {
            switch (_nextPos)
            {
                case  CompanionPos._L_L_POS:
                {
                    _companion.speed = attackSpeed; 
                    _companion.SetDestination(_l_lTarget.position);
                    
                    if((transform.position - _l_lTarget.position).magnitude < 0.5f)
                    {
                        _currentPos = CompanionPos._L_L_POS;
                        _changePos = true; 
                    }
                    break;
      
                }
                
                case CompanionPos._U_R_POS:
                {
                    _companion.speed = attackSpeed; 
                    _companion.SetDestination(_u_rTarget.position);

                    if ((transform.position - _u_rTarget.position).magnitude < 0.5f)
                    {
                        _currentPos = CompanionPos._U_R_POS;
                        _changePos = true; 
                    }

                    break;
                }
                default:{break;}
            }
        }
        */
        }

        private void NewPosition(CompanionPos nextPos)
        {
            Transform newDest;
        
            switch (nextPos)
            {

                case CompanionPos.LLPos:
                {
                
                    if ((transform.position - lLTarget.position).magnitude < 0.1f)
                    {
                        currentPos = CompanionPos.LLPos;
                        _changePos = true; 
                    }
                    else
                    {
                        newDest = lLTarget;
                        PosUpdate(newDest);
                    }

                    break;
                }

                case CompanionPos.URPos:
                {
                    if ((transform.position - uRTarget.position).magnitude < 0.1f)
                    {
                        currentPos = CompanionPos.URPos;
                        _changePos = true; 
                    }
                    else
                    {
                        newDest = uRTarget;
                        PosUpdate(newDest);
                    }

                
                    break;
                }
            
            }
        }

        private void PersistantPosCheck()
        {
            switch (currentPos)
            {
                case CompanionPos.LLPos:
                {
                    PosUpdate(lLTarget);
                    if ((transform.position - lLTarget.position).magnitude > 2f)
                    {
                    
                    }
                    break;
                }
                case CompanionPos.URPos:
                {
                    PosUpdate(uRTarget);
                    if ((transform.position - uRTarget.position).magnitude > 2f)
                    {
                    
                    }
                    break;
                }
            
            }
        
        }
    
        #endregion
        #endregion

        #region State data

        private void StartIdle()
        {
            Companion.enabled = false;
            return;
        }
        private void StartFollow()
        {
        
            Companion.enabled                  = true;
            Companion.obstacleAvoidanceType    = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            return;
        }

        private void StartCombat()
        {
            Companion.enabled = true;

            //_companion.radius = 0.2f;
            Companion.acceleration             = 2000f;
            Companion.speed                    = attackSpeed;
            Companion.obstacleAvoidanceType    = ObstacleAvoidanceType.NoObstacleAvoidance;

            return;
        }
        #endregion

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
            _companionMesh.material = alphaLow;
        }

        private void SetHigh()
        {
            // change to normal material
            _companionMesh.material = normal;
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
            if(Companion.enabled)
            {
                if (stateAi == CompanionState.Follow)
                {

                    Companion.isStopped = true;
                
                }
                else
                {
                    Companion.isStopped = false;
                
                }
            }
        
        }

        private void AgentPause()
        {
            if(Companion.enabled)
            {
                Companion.isStopped = true;
                Companion.velocity = Vector3.zero;
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
}
