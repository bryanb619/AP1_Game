using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Enemy AI chase behaviour
/// Script controls all AI behaviours and actions
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables
    private float rotationSpeed = 1f;


    private enum AI
    {
        _GUARD, _PATROL,_ATTACK,_COVER,_SEARCH,_GLORYKILL,_NONE
    }
    private AI _stateAI;

    private GameState _gameState;

    [Header("AI Profile")]
    [SerializeField] private AIChaseData data;

    [SerializeField] private AIController _control;

    private bool gemSpawnOnDeath = true;
    private GameObject gemPrefab;


    // Reference to the state machine
    private StateMachine stateMachine;

    private NavMeshAgent Agent;
    //private NavMeshPath path;
    private float _Health;

    // References to player
    [SerializeField]private GameObject PlayerObject;
    private PlayerMovement _Player;

    private WarningSystemAI _warn;

    private PredictionModel pathPrediction;

    // Patrol Points

    [Header("Patrol")]
    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    [SerializeField] private Transform playerTarget;
    public Transform PlayerTarget => playerTarget;


    //[SerializeField] 
    private float minDist;
    //[Range(0, 15)]
    //[SerializeField] 
    private float minDistInCover;



    //[Range(10, 150)]
    private float radius;
    public float Radius => radius;
    //[Range(50, 360)]
    private float angle;
    public float Angle => angle;

    private LayerMask targetMask;
    private LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV; // Enemy Editor FOV

    private bool canSeePlayer;
    public bool canSee => canSeePlayer;

    // Attack rate
    //[Range(1, 5)] 
    private float AttackRate;
    private float nextAttack;

    private bool _canAttack; 
    private Color originalColor;
    private int damage;
    internal int damageBoost;


    // hide code
    //[Header("Hide config")]
    private Collider[] Colliders = new Collider[10];

    //[Range(-1, 1)]
    //[Tooltip("Lower is a better hiding spot")]
    private float HideSensitivity = 0;
    //[Range(0.01f, 1f)][SerializeField] 
    private float UpdateFrequency = 0.25f;

    private LayerMask HidableLayers;


    private SceneChecker LineOfSightChecker;

    private Coroutine MovementCoroutine;


    //[Range(0, 10)][SerializeField] 
    private float healthInCreasePerFrame;

    private float MAXHEALTH;

    private Vector3 previousPos;

    private float curSpeed;



    // state condition bools
    //private bool _engage; 
    //private bool InCoverState;
    //private bool _returnPatrol;
    //private bool _inSearch;
    //private bool _canGloryKill; 

    // sub state condition bools
    private bool IsAttacking;
    //private bool _underAttack;
    bool _inAttackRange;
    private bool _canMove; 
    //private bool _initiateStartTimer; 


    //private float retreatDist = 2f;

    // fire damage variables
    private float damagePerSecondFire = 2f;
    private float durationOfFireDamage = 10f;

    private Transform aiTransform;


    private bool _iceWeak, _fireWeak, _thunderWeak; 

    //private PauseMenu _pause;

    private bool _gamePlay; 

    // new cover code
    // The speed at which the AI character moves
    private float moveSpeed = 5f;

    // The distance at which the AI character starts fleeing from the player
    //private float fleeDistance = 15f;

    // The current cover position the AI character is using
    private Transform currentCoverPosition;

    private Slider _healthSlider; 

    //public float aiPointRadius;

    // test code 

    /*
    private Vector3 dest = Vector3.zero;

    [Range(0, 20)]
    [SerializeField]
    float AIradius = 20f;
    */
    #endregion

    #region Awake 
    // Get references to enemies
    private void Awake(){
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    #endregion

    #region Start 

    /// <summary>
    /// INFO in Void Start: 
    /// * starting AI health
    /// * FOV Routine 
    /// * AI States 
    /// * State Transitions
    /// 
    /// </summary>
    private void Start()
    {
        GetComponents();
        GetProfile();
        GetStates();
       
        aiTransform = transform;

        
    }

    #region Components Sync
    private void GetComponents()
    {
        Agent = GetComponent<NavMeshAgent>();

        Agent.updateRotation = false;
        //path = new NavMeshPath();

        pathPrediction = gameObject.AddComponent<PredictionModel>();
        _warn = GetComponent<WarningSystemAI>();

        LineOfSightChecker = GetComponentInChildren<SceneChecker>();
        _healthSlider = GetComponentInChildren<Slider>();


        PlayerObject = GameObject.Find("Player");
        playerTarget = PlayerObject.transform;

        //playerTarget = GetComponent<Transform>();

        
        _Player = FindObjectOfType<PlayerMovement>();

        _Health = 100;
        _healthSlider.value = _Health;

        _canAttack = true;
        _canMove = true;

        switch (_gameState) 
        {
            case GameState.Gameplay: _gamePlay = true; break;
            case GameState.Paused: _gamePlay = false; break;    
        }
        
        
    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {

        // attack 
        minDist = data.MinDist;
        AttackRate = data.AttackRate;
        // FOV
        radius = data.Radius;
        angle = data.Angle;
        targetMask = data.TargetMask;
        obstructionMask = data.ObstructionMask;
        // Cover
        HidableLayers = data.HidableLayers;
        minDistInCover = data.MindistIncover;
        // Health
        _Health = data.Health;
        // Weakness
        _iceWeak = data.Ice;
        _fireWeak = data.Fire;
        _thunderWeak = data.Thunder;
        // Gem
        gemSpawnOnDeath = data.GemSpawnOnDeath;
        gemPrefab = data.Gem;

        //
        damage = data.Damage;

    }
    #endregion

    #region  States Sync 
    private void GetStates()
    {
        #region States
        // Non Combat states
        State onGuardState = new State("",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log(""));

        State PatrolState = new State("Patroling",
            () => Debug.Log(""),
            Patrol,
            () => Debug.Log(""));

        // Combat states

        State ChaseState = new State("",
            () => Debug.Log("Fighting"),
            ChasePlayer,
            () => Debug.Log(""));

        State SearchState = new State("Searching",
           () => Debug.Log(""),
           Search,
           () => Debug.Log(""));

        State FindCover = new State("Help",
            () => Debug.Log(""),
            Cover,
            () => Debug.Log(""));

        State GloryKillState = new State("Glory Kill State",
            () => Debug.Log("Entered glory kill state"),
            GloryKill,
            () => Debug.Log("Left Glory Kill State"));

        #endregion


        #region Trasintion of states
        // Add the transitions
        onGuardState.AddTransition(
            new Transition(
                //canSeePlayer == true
                () => _stateAI == AI._ATTACK || canSeePlayer,
                () => Debug.Log("Player found!"),
                ChaseState));

        ChaseState.AddTransition(
           new Transition(
               //InCoverState == true
               () => _stateAI == AI._COVER,
               () => Debug.Log(""),
               FindCover));
        /*
        ChaseState.AddTransition(
           new Transition(
               //_inSearch == true
               () => _stateAI == AI._SEARCH,
               () => Debug.Log(""),
               SearchState));

        ChaseState.AddTransition(
            new Transition(
                //_canGloryKill == true
                () => _stateAI == AI._GLORYKILL,
                () => Debug.Log(""),
                GloryKillState));

        SearchState.AddTransition(
           new Transition(
               () => _stateAI == AI._SEARCH, // SEEK SOLUTION
               //_stateAI == AI._ATTACK,
               () => Debug.Log(""),
               ChaseState));
       
        SearchState.AddTransition(
           new Transition(
               //_returnPatrol == true
               () => _stateAI == AI._PATROL,
               () => Debug.Log(""),
               PatrolState));
         */
        FindCover.AddTransition(
           new Transition(
               () => //InCoverState == false 
                     _stateAI == AI._ATTACK,
               () => Debug.Log(""),
               ChaseState));

        PatrolState.AddTransition(
           new Transition(
               () => _stateAI == AI._ATTACK, // SEEK SOLUTION
               () => Debug.Log(""),
               ChaseState));

        #endregion

        stateMachine = new StateMachine(PatrolState);

    }
    #endregion


    #endregion

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        // FOV
        radius = data.Radius;
        angle = data.Angle;
        targetMask = data.TargetMask;
        obstructionMask = data.ObstructionMask;

        UpdateFunctions();
    }
    private void UpdateFunctions()
    {
        switch(_gamePlay)
        {
            case true:
                {
                    ResumeAgent();
                    CanFOV();
                    MinimalCheck();
                    HealthCheck();
                    AISpeed();
                    SearchTimer();

                    Action actions = stateMachine.Update();
                    actions?.Invoke();
                    break; 
                }
            case false:
                {
                    PauseAgent();
                    break;
                }
        }
    }

    #region agent state
    private void ResumeAgent()
    {
        // Get the current agent velocity
        Vector3 velocity = Agent.velocity;

        // Calculate the rotation angle based on the agent's velocity
        float rotationAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

        // Rotate the agent around its Y-axis
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationAngle, 0), rotationSpeed * Time.deltaTime);

        //Agent.Resume();
        Agent.isStopped = false;
    }

    private void PauseAgent()
    {
        //Agent.speed = 0f; 
        //Agent.Stop(); 
        Agent.isStopped = true; 
    }
    #endregion

    #endregion

    #region Condition checked in update


    private void CanFOV()
    {
        StartCoroutine(FOVRoutine());
    }
    private void MinimalCheck()
    {
        if(_gamePlay)
        {
            if ((playerTarget.transform.position - transform.position).magnitude < minDist)
            {
                //transform.LookAt(new Vector3(0,playerTarget.position.y, 0));
                //transform.LookAt(playerTarget);
                Agent.SetDestination(playerTarget.transform.position);
                _inAttackRange = true;
            }
            else
            {
                _inAttackRange = false;
            }
        }
        
    }

    private void HealthCheck()
    {
        if(_Health <= 15)
        {
            SetGloryKill();
        }


        else if (_Health <= 50)//&& _Health > 10) 
        {
            //InCoverState = true;

            SetCover();
            
            //HandleGainSight(PlayerTarget); 
        }

        else if (_Health >= 75)
        {
            //InCoverState = false;
            switch (canSeePlayer) 
            {
                case true:
                    {
                        SetAttack();
                        break;
                    }
                case false: 
                    {
                        SetSearch();
                        break;
                    }
            }

        }


    }
    private void SearchTimer()
    {

    }
    
  
    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;
    }
     #endregion
    
    #region AI Actions

    private void Patrol()
    {
        if(_canMove)
        {
            if(!canSeePlayer)
            {
                SetPatrol(); 
            }
            //_returnPatrol = false;

            Agent.autoBraking = false;
            Agent.stoppingDistance = 0.2f;
            Agent.speed = 1f;
            Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
            {
                GotoNetPoint();
            }
        }
        

    }

    private void GotoNetPoint()
    {
       
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

    // Chase the small enemy
    private void ChasePlayer()
    {
        if(_canMove)
        {
            //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
            //transform.LookAt(playerTarget);

            Agent.stoppingDistance = 3f;

            Agent.speed = 5f;
            Agent.acceleration = 11f;

            Agent.SetDestination(playerTarget.position);

            if(!canSeePlayer)
            {
                Search();
            }

            Attack(); 
        }
        //print("ATTACK");
    }


    private void Attack()
    {
        if (_canMove)
        {
            //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
            if (Agent.remainingDistance <= 3)
            {
                Agent.stoppingDistance = 2.7f;

                if (_inAttackRange)
                {
                    if (Time.time > nextAttack)
                    {
                       // transform.LookAt(new Vector3(0, playerTarget.position.y, 0));

                        print("player attacked");
                        //transform.LookAt(playerTarget);

                        nextAttack = Time.time + AttackRate;
                        _Player.TakeDamage(damage);
                        _canAttack = false;
                    }
                    else
                    {
                        IsAttacking = false;
                    }

                }
            }
        }
       
       


    }

    public void GetPlayer()
    {
        if (_canMove && _gamePlay)
        {
            //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
        }
    }


    void OnPlayerWarning(Vector3 Target)
    {
        // The player has been detected within the warning radius!
        // Do something to react to this, such as chasing the player or going into alert mode.
        //GetPlayer();

        GetPlayer();

        /*
        if(canSeePlayer == false)
        {
            //_inSearch = true; 

        }
        
       
        */
    }
    /*
    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position,
        AIradius);
    }
    */

    /*
    private void QuickCover()
    {
        Vector3 retreatPoint = transform.position - transform.forward * retreatDist;
        Agent.SetDestination(retreatPoint);

        // Make the character move towards the destination
        Agent.isStopped = false;

    }
    */
    private void Search()
    {
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {

            switch(canSeePlayer)
            {
                case true:
                    {
                        SetAttack();
                        break; 
                    }
                case false:
                    {
                        if(_stateAI == AI._ATTACK)
                        {
                            SetSearch();
                        }
                        break;
                    }
            }

            /*
            if(canSeePlayer)
            {
                //Vector3 lastPosition = Agent.destination;
                PathPredict();
                //_inSearch = false;


            }
            */
            // Get the player's last position (their destination)
            
            // * play animation ( Move FOV HEAD in Y rotation) and initiate again patrol state

            //Debug.Log("Player's last position: " + lastPosition);

            //GetPath();
           
        }
    }

    private void PathPredict()
    {
        //print("predicting");
        // Predict the target's path using the prediction model
        Vector3[] predictedPath = pathPrediction.PredictPath(PlayerObject);

        // Set the AI agent's destination to be the predicted position of the target at a certain point in the future
        Agent.destination = predictedPath[predictedPath.Length - 1];
    }

    private void Cover()
    {
        if (_canMove) 
        {
            const float MAXHEALTH = 100f;


            Agent.speed = 5f;
            Agent.stoppingDistance = 1f;
            Agent.radius = 1f;

            HandleGainSight(PlayerTarget);
            // GetCover();

            if (curSpeed <= 0.5 && !IsAttacking && _Health >= 16)
            {
                _Health = Mathf.Clamp(_Health + (healthInCreasePerFrame * Time.deltaTime), 0.0f, MAXHEALTH);
                //Debug.Log("Chase health: " + _Health);
            }
            Attack();
        }
    }

    private void HandleGainSight(Transform Target)
    {

        Agent.radius = 1f;

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //playerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }

    private void GloryKill()
    {
        Agent.radius = 1f;
        PauseAgent();

        //Agent.isStopped = true;
    }


    #endregion

    #region Actions Reset

    private void SetGuard()
    {
        _stateAI = AI._GUARD;
    }
    private void SetPatrol()
    {
        _stateAI = AI._PATROL;
    }
    private void SetAttack()
    {
        _stateAI = AI._ATTACK;
        //_control.IsPlayerUnderAttack(); 
        //_control.PlayerAttackStatus(true);
    }
    private void SetCover()
    {
        _stateAI = AI._COVER;
        //_control.PlayerAttackStatus(false);
    }
    private void SetSearch()
    { 
        _stateAI= AI._SEARCH; 
        //_control.PlayerAttackStatus(false);  
    }
    private void SetGloryKill()
    {
        _stateAI = AI._GLORYKILL;
        //_control.PlayerAttackStatus(false);
    }
    
    #endregion

    #region AI Health 
    public void TakeDamage(int _damage, WeaponType _Type)
    {
        print(_Health);
        _Health -= _damage + damageBoost;


        if (_Health <= 0)
        {
            Die();
        }
        else if(_Health > 0) 
        {
            // ALERT AI OF player presence
            //WarningSystemAI warn;
            
            _warn.canAlertAI = true;

            if (_Type == WeaponType.Normal)
            {
                //if (_Health <= 20)
                //{
                    //_canGloryKill = true;
                //}
                _Health -= _damage + damageBoost;
                GetPlayer();
                //QuickCover();
                StartCoroutine(HitFlash());

            }
            else if (_Type == WeaponType.Ice)
            {
                
                // STOP FOR 5 seconds
                StartCoroutine(STFS(5F));

            }
            else if (_Type == WeaponType.Fire)
            {
                StartCoroutine(DamageOverTime(damagePerSecondFire, durationOfFireDamage));
            }
            else if (_Type == WeaponType.Dash)
            {
                _Health -= _damage + damageBoost;

                StartCoroutine(HitFlash());
            }
        }
        
        
        Debug.Log("enemy shot" + _Health);
    }

    

    private void Die()
    {

        if (gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);


        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

       // Debug.Log("Enemy died");
    }
    #endregion

    #region Coroutines (cover & FOV)
    #region Cover Routine

    private IEnumerator Hide(Transform Target)
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateFrequency);
        while (true)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(Colliders[i].transform.position, Target.position) < minDistInCover) //|| Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(Colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, 2f, Agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, Agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSensitivity)
                    {
                        Agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, Agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, Agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSensitivity)
                            {
                                Agent.SetDestination(hit2.position);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
                }
            }
            yield return Wait;
        }
    }

    public int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(Agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(Agent.transform.position, B.transform.position));
        }
    }
    
    #endregion

    #region Field of view Routine

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {

        // collider check
        Collider[] rangeChecks = Physics.OverlapSphere(FOV.position, radius, targetMask);


        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - FOV.position).normalized;

            if (Vector3.Angle(FOV.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(FOV.position, target.position);

                if (!Physics.Raycast(FOV.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;

                    SetAttack();
                }
                    
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
    #endregion
    #endregion

    #region Couroutines AI Damage
    private IEnumerator STFS(float value)
    {

        _canMove= false;

        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);
        yield return new WaitForSeconds(value);

        GetComponent<Renderer>().material.color = originalColor;
        _canMove = true;
    }

    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
        float elapsedTime = 0f; 
        while  (elapsedTime < durationOfFireDamage)
        {
            _Health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(2.5f);
            elapsedTime += 2.5f; 
            
        }

    }

    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = originalColor;
    }
    #endregion

    #region Game state state reception

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

    #endregion

    #region Script destroy actions
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion

    #region Editor Gizmos
    private void OnDrawGizmos()
    {

#if UNITY_EDITOR

        //Vector3 namePosition = new Vector3(transform.position.x, transform.position.y, 2f);

        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;

        GUIStyle yellow = new GUIStyle();
        yellow.normal.textColor = Color.yellow;

        GUIStyle blue = new GUIStyle();
        blue.normal.textColor = Color.blue;
        
        GUIStyle green = new GUIStyle();
        green.normal.textColor = Color.green;
        
        GUIStyle cyan = new GUIStyle();
        cyan.normal.textColor = Color.cyan;
        

        #region AI State Label 

        switch (_stateAI)
        {
            case AI._GUARD:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Guard" + "  Gameplay: "+ _gamePlay, green);
                    break;
                }
            case AI._PATROL:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Patrol" + "  Gameplay: " + _gamePlay, blue);
                    break;
                }
            case AI._ATTACK:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Attack" + "  Gameplay: " + _gamePlay, red);
                    break;
                }
            case AI._SEARCH:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Search" + "  Gameplay: " + _gamePlay, yellow);
                    break;
                }
            case AI._COVER:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Cover" + "  Gameplay: " + _gamePlay, cyan);
                    break;
                }
            case AI._GLORYKILL:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Glory Kill" + "  Gameplay: " + _gamePlay);
                    break;
                }
            case AI._NONE:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "NONE" + "  Gameplay: " + _gamePlay);
                    break;
                }
            default: 
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "NO STATE FOUND" + "  Gameplay: " + _gamePlay);
                    break; 
                }
        }
        #endregion
#endif
    }
    #endregion
}
