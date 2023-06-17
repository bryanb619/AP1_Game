#region Library
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using FMODUnity;
using TMPro;


#if UNITY_EDITOR // EDITOR ONLY CODE
using UnityEditor; 
#endif

#endregion

public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables

    // States --------------------------------------------------------------------------------------------------------->

                        // AI states
                        private enum Ai                     {Guard, Patrol, Attack, Cover, Glorykill, None}
                        

    [SerializeField]    private Ai                          stateAi;
            
                        // Handle state avoids conflict with _gameState variable
                        private enum HandleState            {Stoped, None}
                        private HandleState                 _currentState;


                        // Game State
                        private GameState                   _gameState;

    // Components ----------------------------------------------------------------------------------------------------->

    [Header("AI Profile")]

    // AI profile data
    [SerializeField]    private AIChaseData                 data;
      
    // Systems -------------------------------------------------------------------------------------------------------->
                        // FSM 
                        private StateMachine                _stateMachine;  
                        // AI Path solution
                        private NavMeshAgent                _agent;
                        // AI Warning other agents system
                        private WarningSystemAi             _warn;
                        // AI controller for performance AI actions
                        //private AiController                _controller;
                        private AIHandler                   _hanlderAi;
                        private AIHealth                    _healthBar;
                        
                        // AI Mesh
    [SerializeField]    private SkinnedMeshRenderer         enemyMesh;
                        private Color                       _color;

    // References to player //
                        private GameObject                  _playerObject;
                        private PlayerHealth                _player;

                        public Transform                    PlayerTarget { get; private set; }

                        private Shooter                     _shooterScript;
                        private ObjectiveUI                 _objectiveUiScript;

                        private Rigidbody                   _rb;
                        
    // Handling ------------------------------------------------------------------------------------------------------->

                        // defines when agent is active
                        private bool                        _deactivateAi;
                        

    // AI ACTIONS -------------------------------------AI ACTIONS------------------------------------------------------>


                        //  AI 
                        private float                       _pathUpdateDeadLine = 0.2f; // time to path update
                        
    // Patrol //

    [Header("Patrol")]

                        private float                       _patrolSpeed; 
                        private int                         _destPoint;
    [SerializeField]    private Transform[]                 patrolPoints;

    // Cover // ------------------------------------------------------------------------------------------------------->
                        private Collider[]                  _colliders = new Collider[10];

            //Lower is a better hiding spot
                        private float                       _hideSensitivity = 0;
                        private float                       _updateFrequency =  0.25f;
                        private float                       _minDistInCover;
                        private LayerMask                   _hidableLayers;
                        private SceneChecker                _lineOfSightChecker;
                        private Coroutine                   _movementCoroutine;

    // Combat // ------------------------------------------------------------------------------------------------------>


    [Header("Attack")]
          
        [SerializeField]
                        // attack position
                        private Transform                   attackPoint;
                        // attack range
                        private float                       _attackRange;
                        
                        // random attack range
                        private float                       _minAttackRange;
                        // attack rate of AI 
                        private float                       _attackRate;
                        // time out between attacks
                        private float                       _attackTimer;
                        // attack special effects
                        private GameObject                  _attackEffect, _sAttackEffect, _specialAttackEffect;
           
                        // stoping distance from target
                        private float                       _stopDistance;
                
                        private float                       _nextAttack;
                        private float                       _attackSpeed;

                        private bool                        _canAttack;
                        private bool                        _canPerformAttack = true; 
                        private bool                        _isAttacking;

                        private float                       _minDist;

                        //private Vector3                     _targetPosition;

                        // random attack
                        private float                       _percentage;
                        private int                         _bDamage; 

                        // special attack
                        private const float                 AbilityMaxValue = 100F;
                        private float                       _currentAbilityValue;
                        private float                       _abilityIncreasePerFrame;
                        private int                         _specialDamage;
                        private bool                        _canSpecialAttack;

                        private int                         _randomPriority;

    [SerializeField]    private Transform dropPos; 
                        
    // FOV ------------------------------------------------------------------------------------------------------------>

    [SerializeField]    private Transform                   fov;
                        public Transform                    Eefov => fov; // Enemy Editor FOV

                        private bool                        _useFov;

    [SerializeField]    private float                       radius;
                        public float                        Radius => radius;

    [SerializeField]    private float                       angle;
                        public float                        Angle => angle;
                        
    [SerializeField]    private LayerMask                   targetMask;
    [SerializeField]    private LayerMask                   obstructionMask;
    
                        private bool                        _canSeePlayer;
                        public bool                         CanSee => _canSeePlayer;


    // Combat Events -------------------------------------------------------------------------------------------------->

                        // Health //
                        private float                       _health;
                        private int                         _maxhealth;
                        private float                       _healthInCreasePerFrame;

                        // Death //
                        private GameObject                  _death;

                        // Drops & Loot //
                        private bool                        _spawnHealth;

                        private int                         _healthItems;
                        private GameObject                  _healthDrop;

                        private bool                        _spawnMana; 

                        private int                         _manaItems;
                        private GameObject                  _manaDrop;

            
                        private float                       _dropRadius = 2f;

                        private bool                        _gemSpawnOnDeath;
                        private GameObject                  _gemPrefab;

                        // damage //
                        private float                       _damageOverTime = 2f;
                        private float                       _durationOfDot = 10f;

                        private int                         _damage;
                        internal int                        DamageBoost;
                        private float                       _damageEffectTime = 0.5f;

                        // Stunned             
                        private float                       _stunnedTime;
                        //private float                     stunnedFrequency;
                        private float                       _stunedChance;

                        private bool                        _spawnOtherAi;
                        private int                         _quantityOfSpawn; 
                        

    [SerializeField]    private GameObject                  targetEffect;

    [SerializeField]    private GameObject                  spawnEffect;
    
                        private float                       _closeAttack;
                        private float                       _rate = 1f;
                        
                        
                        
    //[SerializeField]    private GameObject                  enemyChase;
    //[SerializeField]    private GameObject                  enemyRaged;


    // Sound FMOD ----------------------------------------------------------------------------------------------------->
            
                        // grunt sounds
                        private EventReference              _gruntSoundA;
                        private EventReference              _soundGruntB;

                        // special attack sound
                        private EventReference              _screamSound;
                        
                        // attack sounds
                        private EventReference              _soundAttackA;
                        private EventReference              _soundAttackB;
                        
                        
    [SerializeField]    private EventReference              testAttack; 


    // UI ------------------------------------------------------------------------------------------------------------->

    [Header("UI Sliders")] 

    [SerializeField]    private Slider                      healthSlider;

    [SerializeField]    private Slider                      abilitySlider; 

                        private TextMeshProUGUI             _damageText;

                        private ValuesTextsScript           _valuesTexts;


    // Animator ------------------------------------------------------------------------------------------------------->
                        private Animator                    _animator;
                        private int                         _randomForAttacks;    
                        private ChaseEnemyAnimationHandler  _enemyAnimationHandler;

    // DEBUG ---------------------------------------------------------------------------------------------------------->
    [Header("Debug")]

    [SerializeField]    private bool                        showExtraGizmos;
    
    [Header("Debug")]

    [SerializeField]    private bool                        showLabelGizmos;


                        private Vector3                     _previousPos;
                        private float                       _curSpeed;
    
                        private int                         _enemyMask;
                        private int                         _playerMask;

                        private int                         _randomRadiusValue; 
                        
    #endregion

    #region Awake 
    // Get references to enemies
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        _objectiveUiScript = FindObjectOfType<ObjectiveUI>();

        switch (_gameState)
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

        GetComponents(); 
        GetProfile();
        AbilityCheck();
        
        

        _currentState = HandleState.None;
        _canAttack = true;
        //_useFOV = true;

        _enemyMask = LayerMask.GetMask("AI_Enemy_Layer");
        _playerMask = LayerMask.GetMask("PlayerTarget");
        
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
    #region Components Sync
    private void GetComponents()
    {

        _enemyAnimationHandler       = GetComponent<ChaseEnemyAnimationHandler>();
        _agent                       = GetComponent<NavMeshAgent>();
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _agent.updateRotation        = false;


        _warn                        = GetComponent<WarningSystemAi>();
        _hanlderAi                   = GetComponent<AIHandler>();
        _healthBar                  = GetComponentInChildren<AIHealth>();

        _rb                          = GetComponent<Rigidbody>();

        _lineOfSightChecker          = GetComponentInChildren<SceneChecker>();
        _animator                    = GetComponentInChildren<Animator>();

        _damageText                  = GetComponentInChildren<TextMeshProUGUI>();
        _objectiveUiScript          = FindObjectOfType<ObjectiveUI>();

        //_controller                  = GetComponentInParent<AiController>();
        
        // Player refs 
        _playerObject                = GameObject.Find("Player");
        PlayerTarget                = _playerObject.transform;
        //_targetPosition              = _playerTarget.position;
        _player                      = FindObjectOfType<PlayerHealth>();

        _shooterScript               = _playerObject.GetComponent<Shooter>();
        
        // MESH
        _color                      = enemyMesh.material.color;

        // UI 
        _valuesTexts                = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
        
        // random priority
        _randomPriority             = UnityEngine.Random.Range(52, 80);
        _agent.avoidancePriority    = _randomPriority;
        
        _randomRadiusValue          = UnityEngine.Random.Range(2, 3);
        
        _minAttackRange             = UnityEngine.Random.Range(5f, 5.5f);
        //print(_minAttackRange);
    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {
        // Patrol ----------------------------------------------------------------------------------------------------->

        _patrolSpeed                    = data.PatrolSpeed; 

        // COMBAT ----------------------------------------------------------------------------------------------------->

        _stopDistance                   = data.StopDistance;
        _attackEffect                   = data.AttackEffect;    
         _attackRange                   = data.AttackDist;
        _attackTimer                    = data.AttackTimer;
        _attackSpeed                    = data.AttackSpeed;
        _damage                         = data.Damage;
        _attackRate                     = data.AttackRate;
        _minDist                        = data.MinDist;

        // ** ATTACK 2 (CHANCE) **
    
        _percentage                     = data.Percentage;   
        _bDamage                        = data.BDamage;   
        _sAttackEffect                  = data.SAttackEffect;

        // ** SPECIAL ATTACK **
        _abilityIncreasePerFrame        = data.AbilityIncreasePerFrame;
        _currentAbilityValue            = data.CurrentAbilityValue;
        _specialAttackEffect            = data.SpecialAttackEffect;    
        _specialDamage                  = data.SDamage;
        
        // combat events
        //_spawnOtherAI               = data.CanSpawnOthers;
        //quantityOfSpawn             = data.Quantity;
        

        // Health ----------------------------------------------------------------------------------------------------->

        _health                         = data.Health;
        _maxhealth                      = data.Health;
        _healthInCreasePerFrame         = data.HealthRegen;
        abilitySlider.value             = _currentAbilityValue;
        
        
        
        //healthSlider.value              = _maxhealth;
        //healthSlider.maxValue           = _maxhealth;

      

        // Death & Damage --------------------------------------------------------------------------------------------->

        // death
        _death                          = data.DeathEffect;
        _damageEffectTime               = data.DamageTime;

        // stunned
        _stunnedTime                    = data.StunnedTime;
        _stunedChance                   = data.SunnedChance;

        // Loot ------------------------------------------------------------------------------------------------------->
        _gemSpawnOnDeath                = data.GemSpawnOnDeath;
        _gemPrefab                      = data.Gem;

        _spawnHealth                    = data.SpawnHealth;
        _healthItems                    = data.HealthUnits;
        _healthDrop                     = data.HealthDrop;
        
        _spawnMana                      = data.SpawnMana;
        _manaItems                      = data.ManaItems;
        _manaDrop                       = data.ManaDrop;

        // Sound ------------------------------------------------------->
        _screamSound                    = data.Scream;
    }

    #region  States Sync 
    
    private void Start()
    {
        GetStates();
        _healthBar.HealthValueSet(_health);
        
    }
    private void GetStates()
    {
        // States --------------------------------------------------->

        #region States

        // Non Combat states

        var onGuardState = new State("GUARD",
            null);
          

        var patrolState = new State("Patrol",
            Patrol);//,
    

        // Combat states

        var chaseState = new State("Chase",
            ChasePlayer);//,
     
        var findCover = new State("Cover",
            Cover);

        var gloryKillState = new State("Glory Kill",
            null);

        #endregion

        // Trasitions ------------------------------------------------------------------------------------------------->

        #region Trasition of states
        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                () => stateAi == Ai.Attack,
                chaseState));
     
        // PATROL -> CHASE
        patrolState.AddTransition( 
            new Transition(
               () => stateAi == Ai.Attack,
               chaseState));

        // CHASE --------------------------------------------->

        // CHASE -> PATROL
        chaseState.AddTransition(
           new Transition(
               () => stateAi == Ai.Patrol, // SEEK SOLUTION
               //() => Debug.Log("CHASE -> PATROL"),
               patrolState));

        //CHASE -> Glory Kill 
        chaseState.AddTransition
            (new Transition(
                () => stateAi == Ai.Glorykill,
                gloryKillState));

        /*
            // CHASE -> COVER
            ChaseState.AddTransition(
               new Transition(
                   //InCoverState == true
                   () => _stateAI == AI._COVER,
                   //() => Debug.Log("CHASE -> COVER"),
                   FindCover));

            // COVER -> CHASE
            FindCover.AddTransition(
               new Transition(
                   () => _stateAI == AI._ATTACK,
                   ChaseState));
        */

        #endregion

            _stateMachine = new StateMachine(patrolState);
    }
    #endregion


    #endregion

    #region Ability state starting check
    private void AbilityCheck()
    {
        if (_currentAbilityValue >= AbilityMaxValue) 
        { _canSpecialAttack = true;}

        else 
        { _canSpecialAttack = false;}
    }
    #endregion

    #endregion

    #region Update

    #region Update void

    // Update is called once per frame
    private void Update()
    {
        // Get bool value from AI performance manager
        /*
        _deactivateAi = _hanlderAi.AgentOperate switch
            
        {
            true => false,
            false => true
        };
        
        */
        
        if (_gameState == GameState.Gameplay && _hanlderAi.activeAi == AIHandler.ActiveAi.Active) //|| !_deactivateAI)
        {
            _agent.enabled = true;
           //MinimalCheck();
            
            AiSpeed();
            var actions = _stateMachine.Update();
            actions?.Invoke();
            
            if (stateAi != Ai.Attack)
            {
                StartFov();
            }
        }
        else if (_hanlderAi.activeAi != AIHandler.ActiveAi.Active) //|| _deactivateAI)
        {
            _agent.enabled = false;
            //StopAgent();
        }
    }
    #endregion

    #region Agent State
    private void ResumeAgent()
    {
        if (_currentState == HandleState.None)
        {
            if (_agent.enabled)
            {
                // resume agent
                HandleStateAi(false);

            }
        }        
    }

    private void PauseAgent()
    {
        if (_currentState == HandleState.None)
        {
            transform.rotation = Quaternion.identity;
            if (_agent.enabled)
            {
                //stop agent
                HandleStateAi(true);
            }
        }
       
            
    }

    private void StopAgent()
    {
        StopAllCoroutines();
        _useFov = false;    

        if (_agent.enabled)
        {
            // fully stop agent
            HandleStateAi(true);
            return;
        }
    }
    #endregion

    #region FOV
    private void StartFov()
    {
       // StartCoroutine(FovRoutine());
    }

    #region Field of view Routine

    private IEnumerator FovRoutine()
    {
        var wait = new WaitForSeconds(0.5f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        
        // collider check
        Collider[] rangeChecks = Physics.OverlapSphere(fov.position, radius, targetMask);


        if (rangeChecks.Length != 0)
        {
            var target = rangeChecks[0].transform;
            var directionToTarget = (target.position - fov.position).normalized;

            if (Vector3.Angle(fov.forward, directionToTarget) < angle / 2)
            {
                var distanceToTarget = Vector3.Distance(fov.position, target.position);

                if (!Physics.Raycast(fov.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if(_canAttack)
                    {
                        SetAttack(); 
                        _canSeePlayer = true;
                        print("I can see you");
                    }
                    
                }

                else
                    _canSeePlayer = false;
            }
            else
                _canSeePlayer = false;
        }
        else if (_canSeePlayer)
            _canSeePlayer = false;
    }
    #endregion

    #endregion

    #region Distance Check
    private void MinimalCheck()
    {
        /*
        if (stateAi == Ai.Attack)
        {
            int enemymax                    = 15;
            Collider[] aiColliders          = new Collider[enemymax];


            int enemyColliders              = 
                Physics.OverlapSphereNonAlloc(transform.position, 3f, aiColliders);
            
            
            for (int i = 0; i < enemyColliders; ++i)
            {
                var chaseai = 
                    aiColliders[i].GetComponent<EnemyChaseBehaviour>();

                var player = 
                    aiColliders[i].GetComponent <PlayerMovement>();

                if (chaseai != null && chaseai.gameObject != gameObject)
                {
                    _agent.enabled = false;
                    //print("OTHER AI FOUND");

                    Vector3 direction = transform.position - chaseai.transform.position;
                    _rb.AddForce(direction.normalized * 15f);

                }
                else if (player != null)
                {
                    if ((PlayerTarget.transform.position - transform.position).magnitude < 0.8f)
                    {

                        _agent.enabled = false;
                        print("PLAYER FOUND");
                        Vector3 direction = transform.position - player.transform.position;
                        _rb.AddForce(direction.normalized * 8f);
                    }
                 
                }
                else
                {
                    _agent.enabled =true;
                }
            }
        }
        */

        if (stateAi != Ai.Attack)
        {
            int playermaxcollider               = 10;
            Collider[] playerHitColliders       = new Collider[playermaxcollider];

            int playerColliders                 =
                Physics.OverlapSphereNonAlloc(attackPoint.position, 10f, playerHitColliders, _playerMask);


            for (int i = 0; i < playerColliders; ++i)
            {
                var player = playerHitColliders[i].GetComponent<PlayerMovement>();


                if (player == null) return;

#if  UNITY_EDITOR
                print("player found");
#endif
                
                stateAi = Ai.Attack;

            }
        }

        /*
        if ((playerTarget.transform.position - transform.position).magnitude < minDist && _canAttack)
        {
            SetAttack();

        }
        */
    }

    #endregion
    
    #region Speed
    private void AiSpeed()
    {
        var position = transform.position;
        var curMove = position - _previousPos;
        _curSpeed = curMove.magnitude / Time.deltaTime;
        _previousPos = position;

        if(_curSpeed >= 0.8f)
        {

            _animator.SetBool("walk", true);

            /*
            if (this._animator.GetCurrentAnimatorStateInfo(0).IsName("AngryAnimation"))
            {
                _animator.SetBool("walk", false);
            }
            else
            {
                _animator.SetBool("walk", true);
                return; 
            }
            */
            
            return;
        }
        else
        {
            
            _animator.SetBool("walk", false);
             
            return;
        }
    }
    #endregion

    #region AI ACTIONS

    #region PATROL
    private void Patrol()
    {
       if(_agent.enabled) 
       {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f && !_canSeePlayer)
            {
                SetPatrol();
                GotoNetPoint();
            }
       }
    }

    private void GotoNetPoint()
    {

        // Returns if no points have been set up
        if (patrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        _agent.destination = patrolPoints[_destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        _destPoint = (_destPoint + 1) % patrolPoints.Length;
    }
    #endregion

    #region Chase & Attack

    // Chase enemy
    private void ChasePlayer()
    {
        if (_canAttack) 
        {
            DistanceOnAttackCheck();
            
            if(_agent.enabled)
            {
                UpdatePath();
            }
            
            UpdateRotation();
            
            switch (_canSpecialAttack)
            {
                case false:
                {
                    Attack(); // call attack and random attack

                    if (_currentAbilityValue <= AbilityMaxValue)
                    {
                        Cooldown();
                                
                    }

                    break;
                }

                case true:
                {
                    // special attack
                    HabilityAttack();
                    break;
                }
            }
        }
        
    }

    private void UpdatePath()
    {
        if (!(Time.time >= _pathUpdateDeadLine)) return;
        
        // update time
        _pathUpdateDeadLine = Time.time + 0; 
        // set destination
        _agent.SetDestination(PlayerTarget.position);
    }

    private void UpdateRotation()
    {
        // get player direction
        var direction = PlayerTarget.position - transform.position;
        //rotation 
        var disaredRot = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // apply rotation to AI 
        transform.rotation = disaredRot;
    }

    private void DistanceOnAttackCheck()
    {
        if ((PlayerTarget.transform.position - transform.position).magnitude <= 1.8f)
        {
            //print("under 1.8 units");
            
            // DISABLE AGENT
            EnableAgent(false);
            
            if (Time.time > _closeAttack)
            {
                CloseAttack();
            }
            _closeAttack = Time.time + _rate;
        }

        else if(!_isAttacking)
        {
            // ENABLE AGENT
            EnableAgent(true);
        }
    }


    private void CloseAttack()
    {
        //print("CLOSE ATTACK enabled");
          
        var hitEnemies = Physics.OverlapSphere(attackPoint.position, 1.4f, targetMask);

        foreach (var coll in hitEnemies)
        {
            var player = coll.GetComponent<PlayerHealth>();
            
            if (player == null) return;

            print("PLAYER FOUND");
            
            // EFFECTS (SOUND & PARTICLES)
            RuntimeManager.PlayOneShot(testAttack);
            Instantiate(_attackEffect, attackPoint.transform.position, Quaternion.identity);

            // DAMAGE
            player.TakeDamage(35);

#if UNITY_EDITOR

            // DEBUG ------------------------------------------------------------------------------------------>
            const string debugColor = "<size=14><color=red>";
            const string closeColor = "</color></size>";
            Debug.Log(debugColor + "Close attack" + closeColor);
#endif
        }
    }
    

    // * Attack & Chance Attack -------------------------------->
    private void Attack()
    {
        switch (_canPerformAttack)
        {
            case true:
                {
                    SetAttack();
                    
                    if ((PlayerTarget.transform.position - transform.position).magnitude <= _minAttackRange)
                    {
                        PauseAgent();

                        if (Time.time > _nextAttack)
                        {
                            StartAttack();

                        }
                    }
                    else if ((PlayerTarget.transform.position - transform.position).magnitude >= 7.5F)
                    {
                        if (_agent.enabled)
                        {
                            
                            _agent.radius = 
                                Mathf.Clamp(_agent.radius + (1 * Time.deltaTime), 0.0f, _randomRadiusValue); 
                            print(_agent.radius);
                            ResumeAgent();
                        }
                    }
                    else
                    {
                        ResumeAgent();
                    }
                    break;
                }

            case false:
                {
                    StartCoroutine(AttackTimer());
                    break;
                }
        }
        
    }
    
    private void StartAttack()
    {
        if (UnityEngine.Random.value < _percentage && _canPerformAttack)
        {
            PercentageAttack();

            //StartCoroutine(AttackTimer());
        }

        else
        {
            NormalAttack();
        }
        _canPerformAttack = false;
        _nextAttack = Time.time + _attackRate;
    }

    public void ActualAttack()
    {
        var i = UnityEngine.Random.Range(1, 3);

        var player = FindObjectOfType<PlayerHealth>();

        switch (i)
        {
            case 1:
            {
                RuntimeManager.PlayOneShot(testAttack);
                _agent.velocity = Vector3.zero;
                player.TakeDamage(_damage);
                Instantiate(_attackEffect, attackPoint.transform.position, Quaternion.identity);
                break;
            }

            case 2:
            {
                RuntimeManager.PlayOneShot(testAttack);
                _agent.velocity = Vector3.zero;
                Instantiate(_attackEffect, attackPoint.transform.position, Quaternion.identity);
                player.TakeDamage(_bDamage);
                break; 
            }
                
            default: break;
        }
    }
    
    public void SetAiAfterAttack(bool canMove)
    {
        switch (canMove)
        {
            case true:
            {
                //ResumeAgent();
                _isAttacking = false;
                _agent.enabled = true;
                
                //_agent.isStopped = false;
                break; 
            }
            case false:
            {
                _isAttacking = true;
                _agent.enabled = false;
                //_agent.velocity = Vector3.zero;
                //_agent.isStopped = true;
                //PauseAgent();
                break;
            }
                
        }

    }

    private void PercentageAttack()
    {
        var hitEnemies = Physics.OverlapSphere(attackPoint.position, _minAttackRange, targetMask);

        foreach (var col in hitEnemies)
        {
            PlayerHealth player = col.GetComponent<PlayerHealth>();

            if (player == null) return;
            
#if UNITY_EDITOR
            const string debugColor = "<size=12><color=yellow>";
            const string closeDebug = "</color></size>";
            Debug.Log(debugColor + "Chance attack" + closeDebug);
#endif
            //_enemyAnimationHandler.RecievePlayerCollision(player);
            _animator.SetTrigger("Attack2");
            
        }
    }

    private void NormalAttack()
    {
        var hitEnemies = Physics.OverlapSphere(attackPoint.position, _minAttackRange, targetMask);

        foreach (Collider collider in hitEnemies)
        {

            var player = collider.GetComponent<PlayerHealth>();

            if (player != null)
            {
                //AttackAnim();
                _animator.SetTrigger("Attack");

#if UNITY_EDITOR
                const string debugColor = "<size=12><color=green>";
                const string closeColor = "</color></size>";
                Debug.Log(debugColor + "Attack" + closeColor);
#endif

            }
            //Instantiate(_attackEffect, _attackPoint.transform.position, Quaternion.identity);
        }
    }
    
    private void AttackAnim()
    {
        _animator.SetTrigger("Attack");
    }

    internal void ReceiveAnim()
    {
        _animator.SetBool("Attack", false);
    }

    // * Special Attack -------------------------------->
    private void HabilityAttack()
    {
        if(_agent.enabled) 
        {
            _agent.speed = 25f;
        }
        
        //agent.acceleration = 14f;
        
        //if ((playerTarget.transform.position - transform.position).magnitude <= 6F) // define min distance

        if (_agent.remainingDistance <= _stopDistance)
        {
            PauseAgent();

            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, _attackRange, targetMask);

            foreach (Collider collider in hitEnemies)
            {
                RuntimeManager.PlayOneShot(_screamSound);
                
#if UNITY_EDITOR
                const string debugColor = "<size=14><color=orange>";
                const string closeColor = "</color></size>";
                Debug.Log(debugColor + "Special attack" + closeColor);
#endif
                


                _currentAbilityValue = 0;
                //_AbilitySlider.value = currentAbilityValue;

                _player.TakeDamage(_specialDamage);

                
                Instantiate(_specialAttackEffect, attackPoint.transform.position, Quaternion.identity);

                //_animator.SetBool("specialAttack", false);
                _canPerformAttack = false; 
                _canSpecialAttack = false;

            }
        }
        else
        {
            ResumeAgent();
        }
    }

    // Cooldown
    private void Cooldown()
    {
        _currentAbilityValue = 
            Mathf.Clamp(_currentAbilityValue + (_abilityIncreasePerFrame * Time.deltaTime), 
                0.0f, AbilityMaxValue);
        
       // abilitySlider.value = _currentAbilityValue;


        if (_currentAbilityValue >= AbilityMaxValue) { //_canSpecialAttack = true; return;
        }

        //else { _canSpecialAttack = false; return; }
    }

    // * AI Spawn -------------------------------->
    private IEnumerator SpawnAi()
    {
        bool running = false;

        if (!running)
        {
            running = true;

            _spawnOtherAi = false;
            int i;
        

            //yield return new WaitForSeconds(0.5f);

            for (i =0; i < 2; i++)
            {

                Vector3 spawnPosition = transform.position +
                    new Vector3(UnityEngine.Random.Range(2, 4), 0f,
                    UnityEngine.Random.Range(2, 3));

               
                yield return new WaitForSeconds(1f);

                Instantiate(spawnEffect, spawnPosition, spawnEffect.transform.rotation);


                //Instantiate(enemyChase, spawnPosition, transform.rotation); 

                StartCoroutine(StopForSeconds(2F));

                print(i);
            }

            //ResumeAgent();
            
            running = false;
        }
    }

    // * Attack time outs -------------------------------->
    private IEnumerator AttackTimer()
    {
        bool running = false;

        if (running) yield break;
        running = true;
        _canPerformAttack = false;
        yield return new WaitForSeconds(_attackTimer);

        _canPerformAttack = true;
        running = false;

    }
    /*
    private IEnumerator timeBetweenAttack()
    {

        yield return new WaitForSeconds(0.4f);

        _animator.SetBool("specialAttack", false);
        _animator.SetBool("walk", true);

    }
    */
    #endregion

    #region Cover
    private void Cover()
    {
        HandleGainSight(PlayerTarget);

        //print(curSpeed); 
        if (_curSpeed <= 0.5 && _health >= 16)
        {
            _health = Mathf.Clamp(_health + (_healthInCreasePerFrame * Time.deltaTime), 0.0f, _maxhealth);
            //Debug.Log("Chase health: " + _health);

            healthSlider.value = _health;


            if ( _health >= 50 && _currentAbilityValue >= 65) { _canAttack = true; return;}

            else {_canAttack = false; return;}

        }
        else if(_health < 15) { SetGloryKill();}

        //else if(_canAttack){ SetAttack();}
    }

    private void HandleGainSight(Transform target)
    {
        //agent.radius = 1f;
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            return;
        }
        //playerTarget = Target;

        _movementCoroutine = StartCoroutine(Hide(target));
    }

    #region COVER Coroutine

    private IEnumerator Hide(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(_updateFrequency);
        while (true)
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(_agent.transform.position, _lineOfSightChecker.collider.radius, _colliders, _hidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(_colliders[i].transform.position, target.position) < _minDistInCover) //|| Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    _colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(_colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(_colliders[i].transform.position, out NavMeshHit hit, 2f, _agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, _agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (target.position - hit.position).normalized) < _hideSensitivity)
                    {
                        _agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(_colliders[i].transform.position - (target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, _agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, _agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized) < _hideSensitivity)
                            {
                                _agent.SetDestination(hit2.position);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Unable to find NavMesh near object {_colliders[i].name} at {_colliders[i].transform.position}");
                }
            }
            yield return wait;
        }
    }

    public int ColliderArraySortComparer(Collider a, Collider b)
    {
        if (a == null && b != null)
        {
            return 1;
        }
        else if (a != null && b == null)
        {
            return -1;
        }
        else if (a == null && b == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(_agent.transform.position, a.transform.position).CompareTo(Vector3.Distance(_agent.transform.position, b.transform.position));
        }
    }
    #endregion

    #endregion

    #endregion

    #endregion

    #region AI States Set

    private void SetPatrol()
    {
        //print("patrol FIRED");
        stateAi                 = Ai.Patrol;

        _agent.autoBraking      = false;
        _agent.updateRotation   = false;

        _agent.stoppingDistance = 0.1f;
        _agent.speed            = _patrolSpeed;

        _agent.radius           = 0.5f;

        if(_hanlderAi.AgentOperate)
        {
            _useFov = true;
        }
        
    }

    private void SetAttack()
    {
        // Agent configuration
      
        if(_agent.enabled) 
        {
            _agent.speed                    = _attackSpeed;
            _agent.angularSpeed             = 0f;
            _agent.updateRotation           = false;
            _agent.radius                   = 0.4f;
            _agent.obstacleAvoidanceType    = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
            
            _canAttack                      = true;
            
            stateAi                         = Ai.Attack;

            _useFov                         = false;
            //StopCoroutine(FovRoutine());
        }
        
    }

    private void SetStunned()
    {
        // start coroutine 
    }
    private void SetCover()
    {
        _canAttack = false;
        _agent.angularSpeed = 120f;
        _agent.updateRotation = true;
 

        _agent.radius = 1f;

        _agent.speed = 4f;
        _agent.stoppingDistance = 0.3f;

        stateAi = Ai.Cover;

        //_useFOV = true;

        return;
    }

    private void SetGloryKill()
    {
        stateAi = Ai.Glorykill;
        _agent.radius = 0.5f;
        _useFov = false;
        //StopCoroutine(FovRoutine());
     
    }

    #endregion

    #region AI Health 
    public void TakeDamage(int damage, WeaponType type)
    {
        
        if (_health <= 0)
        {
            Die();
        }

        else if (_health > 0)
        {
            switch (type)
            {
                case WeaponType.Normal:
                    {
                        _health -= damage + DamageBoost;

                        StartCoroutine(HitFlash());

                        //damageEffectTime = 2f; 
                        break;
                    }

                case WeaponType.Ice:
                    {
                        if (_shooterScript.WUpgraded == true)
                        {
                            StartCoroutine(DamageOverTime(_damageOverTime, _durationOfDot));
                            StartCoroutine(HitFlash());
                        }
                        else
                        {
                            _health -= damage + DamageBoost;
                            Instantiate(targetEffect, transform.position, transform.rotation);
                            StartCoroutine(HitFlash());
                        }
                        
                        break;
                    }

                case WeaponType.Fire:
                    {
                        _health -= damage + DamageBoost;

                        StartCoroutine(HitFlash());

                        //damageEffectTime = 2.3f;

                        break;
                    }

                case WeaponType.Thunder:
                    {
                        _health -= damage + DamageBoost;

                        if (_shooterScript.RUpgraded == true)
                            StartCoroutine(StopForSeconds(_stunnedTime));
                        else
                            StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Dash:
                    {
                        _health -= damage + DamageBoost;

                        StartCoroutine(HitFlash());
                        break;
                    }
            }

            if (_spawnHealth || _spawnMana && !_spawnOtherAi)
            {
                DropSpawnCheck();
            }
            
            _damageText.text = damage.ToString();
            StartCoroutine(DamageTextDisappear());

            //healthSlider.value = _health;
            
            
            if (_canAttack)
            {
                //_warn.CanAlertAi = true;
                SetAttack();
            }
        }
        
        _healthBar.HandleBar(damage);
    }

    private void DropSpawnCheck()
    {
        float randomFloat = UnityEngine.Random.value;

        if (randomFloat <= 0.08f)
        {
            float randomPercent = UnityEngine.Random.value;

            if (randomPercent <= 0.6f && _spawnHealth)
            {
                SpawnDrop(_healthDrop);
            }
            else if (randomPercent >= 0.61 && _spawnMana)
            {
                SpawnDrop(_manaDrop);
            }
        }
    }

    public void Die()
    {
        if(_spawnHealth)
        {
            for (var i = 0; i < _healthItems; i++)
            {
                SpawnDrop(_healthDrop);
            }   
        }

        if(_spawnMana)
        {
            for (var i = 0; i < _manaItems; i++)
            {
                SpawnDrop(_manaDrop);
            }
        }

        if (_gemSpawnOnDeath)
        {
            Instantiate(_gemPrefab, dropPos.position, Quaternion.identity);
        }

        Instantiate(_death, transform.position, _death.transform.rotation);

        _valuesTexts.GetKill();

        //_objectiveUiScript.IncreaseEnemyDefeatedCount();

        Destroy(this.gameObject);
    }

    // Drop area
    private void SpawnDrop(GameObject drop)
    {

        var spawnPosition = dropPos.position +
                            new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius),
                                0f,
                                UnityEngine.Random.Range(-_dropRadius, _dropRadius));

        Instantiate(drop, spawnPosition, Quaternion.identity);
    }

    #endregion

    #region Receive Warning
    void OnPlayerWarning(Vector3 target)
    {
        if (_canAttack)
        {
            SetAttack();
        }
    }
    #endregion

    #region Control & Visual Coroutines

    private IEnumerator HitFlash()
    {
        bool isrunning = false;

        if (!isrunning)
        {
            isrunning = true;
            

            enemyMesh.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            enemyMesh.material.color = _color;

            isrunning = false;
        }

    }

    private IEnumerator DamageTextDisappear()
    {
        yield return new WaitForSeconds(2f);
        _damageText.text = " ";
    }

    // stop agent for seconds
    private IEnumerator StopForSeconds(float value)
    {
        // Start bool at false to make sure coroutine is not being run more than once
        bool stfsEffect = false;

        switch(stfsEffect)
        {
            case false:
                {
                    stfsEffect = true;

                    print("STARTED STFS COROUTINE SUCCESFULLY");

                    _canAttack = false;
                    _currentState = HandleState.Stoped;

                    HandleStateAi(true);

                    //originalColor = GetComponent<Renderer>().material.color;
                    //GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);

                    yield return new WaitForSeconds(value);

                    //GetComponent<Renderer>().material.color = originalColor;

                    _currentState = HandleState.None;

                    HandleStateAi(false);
                    _canAttack = true;

                    stfsEffect = false;

                    break;
                }
            default: {break;}
        }
    }

    // Damage over time
    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
#if UNITY_EDITOR
        
        Debug.Log("Started DOT coroutine");
#endif
        float elapsedTime = 0f;
        while (elapsedTime < durationOfdamage)
        {
            _health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(damagePerSecond);
            elapsedTime += 2.5f;
        }

    }

    #endregion

    #region Sounds
    private void AttackSound()
    {

    }

    private void GruntSound()
    {

    }

    private void DeathSound()
    {

    }

    #endregion

    #region Game state reception

    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
        {
            case GameState.Gameplay:
                {
                    _gameState = GameState.Gameplay;
                    ResumeAgent();

                    break;
                }
            case GameState.Paused:
                {
                    _gameState = GameState.Paused;

                    PauseAgent();
                    break;
                }
        }
        //throw new NotImplementedException();
    }

    private void HandleStateAi(bool stop)
    {
        switch (stop)
        {
            case true:
                {
                    _agent.velocity = Vector3.zero;
                    _agent.isStopped = true;
                    break;
                }
            case false:
                {
                    _agent.isStopped = false;
                    break;
                }
        }
    }
    #endregion
    
    private void EnableAgent(bool active)
    {
        switch (active)
        {
            case true:
                {
                    _agent.enabled = true;
                    break;
                }
            case false:
                {
                    _agent.enabled = false;
                    break;
                }
        }
    }

    #region Agents Info Exchange

    // 
    private void AddAttacker()
    {
        //_controller.AgentStartedAttacking(gameObject); // Add agent to list
        return; 
    }

    private void RemoveAttacker() 
    {
        //_controller.AgentStoppedAttacking(gameObject);
        return;
    }

    // orders from AI Controller
    public void StopAttack()
    {
        //SetPatrol();
        PauseAgent();
        return;
    }

    #endregion

    #region Script destroy actions 
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion

#if UNITY_EDITOR
    #region Editor Gizmos
    private void OnDrawGizmos()
    {
        var red = new GUIStyle
        {
            normal =
            {
                textColor = Color.red
            }
        };

        var yellow = new GUIStyle
        {
            normal =
            {
                textColor = Color.yellow
            }
        };

        var blue = new GUIStyle
        {
            normal =
            {
                textColor = Color.blue
            }
        };

        var green = new GUIStyle
        {
            normal =
            {
                textColor = Color.green
            }
        };

        var cyan = new GUIStyle
        {
            normal =
            {
                textColor = Color.cyan
            }
        };

        #region Gizmos code

        if (showExtraGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, _attackRange);

            Gizmos.color = Color.yellow;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, _minDist);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position, 1);
        }

        #region AI State Label 
        if (showLabelGizmos)
        {
            switch (stateAi)
            {
                case Ai.Guard:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "Guard" + "  Gameplay: " + _gameState, green);
                        break;
                    }
                case Ai.Patrol:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "Patrol" + "  Gameplay: " + _gameState, blue);
                        break;
                    }
                case Ai.Attack:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "Attack" + "  Gameplay: " + _gameState, red);
                        break;
                    }

                case Ai.Cover:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "Cover" + "  Gameplay: " + _gameState, cyan);
                        break;
                    }
                case Ai.Glorykill:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "Glory Kill" + "  Gameplay: " + _gameState);
                        break;
                    }
                case Ai.None:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "NONE" + "  Gameplay: " + _gameState);
                        break;
                    }
                default:
                    {
                        Handles.Label(fov.transform.position + Vector3.up * 2f, "NO STATE FOUND" + "  Gameplay: " + _gameState);
                        break;
                    }
            }
        }
        
        #endregion

        #endregion
    }

    #endregion
#endif
}