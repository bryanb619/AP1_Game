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
using UnityEngine.Serialization;
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
    [SerializeField]    private AiChaseData                 data;
      
                        // FSM 
                        private StateMachine                _stateMachine;  
                        // AI Path solution
                        private NavMeshAgent                _agent;
                        // AI Warning other agents system
                        private WarningSystemAi             _warn;
                        // AI controller for performance AI actions
                        private AiController                _controller;
                        private AiHandler                   _hanlderAi;

                        // AI Mesh
    [SerializeField]    private SkinnedMeshRenderer         enemyMesh;
                        private Color                       _color;

    // References to player //
                        private GameObject                  _playerObject;
                        private PlayerHealth                _player;
    
                        private Transform                   _playerTarget;
                        public Transform                    PlayerTarget => _playerTarget;
                        private Shooter                     _shooterScript;
                        private ObjectiveUi                 _objectiveUiScript;

                        private Rigidbody                   _rb;
                        
    // Handling ------------------------------------------------------------------------------------------------------->

                        // defines when agent is active
                        private bool                        _deactivateAi;

    // AI ACTIONS -------------------------------------AI ACTIONS------------------------------------------------------>

    // Patrol //

    [Header("Patrol")]

                        private float                       _patrolSpeed; 
                        private int                         _destPoint = 0;
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

                        private Vector3                     _targetPosition;

                        // random attack
                        private float                       _percentage;
                        private int                         _bDamage; 

                        // special attack
                        private const float                 AbilityMaxValue = 100F;
                        private float                       _currentAbilityValue;
                        private float                       _abilityIncreasePerFrame;
                        private int                         _specialDamage;
                        private bool                        _canSpecialAttack;

                        private int _randomPriority; 
                        
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

        _objectiveUiScript = FindObjectOfType<ObjectiveUi>();

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
        
        _color = enemyMesh.material.color;

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
    private void Start()
    {
        GetStates();
        
    }

    #region Components Sync
    private void GetComponents()
    {
        _agent                       = GetComponent<NavMeshAgent>();
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _agent.updateRotation        = false;


        _warn                        = GetComponent<WarningSystemAi>();

        _hanlderAi                   = GetComponent<AiHandler>();

        _rb                          = GetComponent<Rigidbody>();

        _lineOfSightChecker          = GetComponentInChildren<SceneChecker>();
        _animator                    = GetComponentInChildren<Animator>();

        _damageText                  = GetComponentInChildren<TextMeshProUGUI>();

        _controller                  = GetComponentInParent<AiController>();

        _playerObject                = GameObject.Find("Player");
        _playerTarget                = _playerObject.transform;
        _targetPosition              = _playerTarget.position;
        _player                      = FindObjectOfType<PlayerHealth>();

        _shooterScript               = _playerObject.GetComponent<Shooter>();

        // UI 
        _valuesTexts                = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
        
        // random priority
        _randomPriority             = UnityEngine.Random.Range(51, 99);
        _agent.avoidancePriority    = _randomPriority;
        
        _randomRadiusValue         = UnityEngine.Random.Range(2, 6);

    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {
        // Patrol ------------------------------------------------------->

        _patrolSpeed                 = data.PatrolSpeed; 

        // COMBAT ------------------------------------------------------->

        _stopDistance                = data.StopDistance;

        _attackEffect               = data.AttackEffect;    
         _attackRange               = data.AttackDist;
        _attackTimer                = data.AttackTimer;
        _attackSpeed                 = data.AttackSpeed;
        _damage                      = data.Damage;
        _attackRate                  = data.AttackRate;
        
        _minDist                     = data.MinDist;

        // ** ATTACK 2 (CHANCE) **
    
        _percentage                  = data.Percentage;   
        _bDamage                    = data.BDamage;   
        _sAttackEffect             = data.SAttackEffect;

        // ** SPECIAL ATTACK **
        _abilityIncreasePerFrame     = data.AbilityIncreasePerFrame;
        _currentAbilityValue         = data.CurrentAbilityValue;
        _specialAttackEffect        = data.SpecialAttackEffect;    
        _specialDamage               = data.SDamage;



        // combat events
        //_spawnOtherAI               = data.CanSpawnOthers;
        //quantityOfSpawn             = data.Quantity;
        

        // Health ------------------------------------------------------->

        _health                      = data.Health;
        _maxhealth                  = data.Health;
        
        
        healthSlider.value          = _maxhealth;
        healthSlider.maxValue       = _maxhealth;
        
        
        _healthInCreasePerFrame      = data.HealthRegen;
        abilitySlider.value         = _currentAbilityValue;

        // Death & Damage ------------------------------------------------------->

        // death
        _death                       = data.DeathEffect;
        _damageEffectTime            = data.DamageTime;

        // stunned
        _stunnedTime                 = data.StunnedTime;
        _stunedChance                = data.SunnedChance;

        // Loot ------------------------------------------------------->
        _gemSpawnOnDeath             = data.GemSpawnOnDeath;
        _gemPrefab                   = data.Gem;

        _spawnHealth                 = data.SpawnHealth;
        _healthItems                 = data.HealthUnits;
        _healthDrop                  = data.HealthDrop;
        
        _spawnMana                   = data.SpawnMana;
        _manaItems                   = data.ManaItems;
        _manaDrop                    = data.ManaDrop;

        // Sound ------------------------------------------------------->
        _screamSound                = data.Scream;

        //enemyMesh.material.color    = Color.red; 

        // Weakness
        //_iceWeak = data.Ice;
        //_fireWeak = data.Fire;
        //_thunderWeak = data.Thunder;
    }

    #region  States Sync 
    private void GetStates()
    {
        // States --------------------------------------------------->

        #region States

        // Non Combat states

        State onGuardState = new State("GUARD",
            null);
          

        State patrolState = new State("Patrol",
            Patrol);//,
    

        // Combat states

        State chaseState = new State("Chase",
            ChasePlayer);//,
     
        State findCover = new State("Cover",
            Cover);

        State gloryKillState = new State("Glory Kill",
            null);

        #endregion

        // Trasitions --------------------------------------------------->

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
        if (_currentAbilityValue >= AbilityMaxValue) { 
            _canSpecialAttack = true;}

        else { 
            _canSpecialAttack = false;}
    }
    #endregion

    #endregion

    #region Update

    #region Update void

    // Update is called once per frame
    private void Update()
    {
        // Get bool value from AI performance manager
        switch(_hanlderAi.AgentOperate)
        {
            case true:
                {
                    _deactivateAi = false;
                    break; 
                }
            case false: 
                {
                    _deactivateAi = true;
                    break; 
                }
        }


        if (_gameState == GameState.Gameplay && !_deactivateAi) //|| !_deactivateAI)
        {
            //MinimalCheck();

            HealthCheck();

            AiSpeed();

            Action actions = _stateMachine.Update();
            actions?.Invoke();

            if (_useFov)
            {
                StartFov();
            }
        }
        else if (_deactivateAi)
        {
            StopAgent();
            return;
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
        //StartCoroutine(FOVRoutine());
    }

    #region Field of view Routine

    private IEnumerator FovRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

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
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - fov.position).normalized;

            if (Vector3.Angle(fov.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(fov.position, target.position);

                if (!Physics.Raycast(fov.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if(_canAttack)
                    {
                        SetAttack(); 
                        _canSeePlayer = true;
                        return; 
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
        if (stateAi == Ai.Attack)
        {
            int enemymax                    = 15;
            Collider[] aiColliders          = new Collider[enemymax];


            int enemyColliders              = 
                Physics.OverlapSphereNonAlloc(transform.position, 3f, aiColliders);
            
            
            for (int i = 0; i < enemyColliders; ++i)
            {
                EnemyChaseBehaviour chaseai = 
                    aiColliders[i].GetComponent<EnemyChaseBehaviour>();

                PlayerMovement player = 
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
                    if ((_playerTarget.transform.position - transform.position).magnitude < 0.8f)
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


        else if (stateAi != Ai.Attack)
        {
            int playermaxcollider               = 1;
            Collider[] playerHitColliders       = new Collider[playermaxcollider];

            int playerColliders                 =
                Physics.OverlapSphereNonAlloc(attackPoint.position, 2f, playerHitColliders, _playerMask);


            for (int i = 0; i < playerColliders; ++i)
            {
                PlayerMovement player           = playerHitColliders[i].GetComponent<PlayerMovement>(); 

                
                if(player != null)
                {
                    print("player found");
                    stateAi = Ai.Attack;
                }

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

    #region Health Check 
    private void HealthCheck()
    {
        /*
        if (health <= 10)
        {
            SetGloryKill();
            return;
        }

        
        else if (_health <= 50)//&& _Health > 10) 
        {
            SetCover();
            return;
        }
        */
    }
    #endregion

    #region Speed
    private void AiSpeed()
    {
        Vector3 curMove = transform.position - _previousPos;
        _curSpeed = curMove.magnitude / Time.deltaTime;
        _previousPos = transform.position;

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
          
            if(_agent.enabled)
            {
                // get player direction
                Vector3 direction = _playerTarget.position - transform.position;
                //rotation 
                Quaternion disaredrot = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

                // apply rotation to AI 
                transform.rotation = disaredrot;

                // set destination
                _agent.SetDestination(_playerTarget.position);

                switch (_canSpecialAttack)
                {
                    case false:
                        {
                            Attack(); // call attack and random attack

                            if (_currentAbilityValue <= AbilityMaxValue)
                            {
                                Cooldown();
                                return;
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

            _warn.CanAlertAi = true;
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
                    //if ((playerTarget.transform.position - transform.position).magnitude <= 6F)
                    //if (agent.remainingDistance <= stopDistance)
                    if ((_playerTarget.transform.position - transform.position).magnitude <= 3.5F)
                    {
                        PauseAgent();
                        

                        if (Time.time > _nextAttack)
                        {

                            //print("2nd step");
                            //float randomFloat = UnityEngine.Random.value;
                            

                            if (UnityEngine.Random.value < _percentage && _canPerformAttack)
                            {
                                //print("chose random");

                                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, _attackRange, targetMask);

                                foreach (Collider collider in hitEnemies)
                                {
                                    string debugColor = "<size=12><color=yellow>";
                                    string closeColor = "</color></size>";

                                    

                                    PlayerHealth player = collider.GetComponent<PlayerHealth>();    

                                    if (player != null)
                                    {
                                        //print("2nd step");
                                        //AttackAnim();
                                        
                                        RuntimeManager.PlayOneShot(testAttack);
                                        player.TakeDamage(_bDamage);

                                        Instantiate(_attackEffect, attackPoint.transform.position, Quaternion.identity);
  
                                    }

                                    Debug.Log(debugColor + "Attack 2" + closeColor);
                                    
                                    //_Player.TakeDamage(b_damage);

                                    //Instantiate(_s_attackEffect, _attackPoint.transform.position, Quaternion.identity);
                                }


                                //StartCoroutine(AttackTimer());
                            }

                            else
                            {

                                Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, _attackRange, targetMask);

                                foreach (Collider collider in hitEnemies)
                                {
                                    string debugColor = "<size=12><color=green>";
                                    string closeColor = "</color></size>";
                                    Debug.Log(debugColor + "Attack" + closeColor);
                                    //AttackAnim();
                                    //_Player.TakeDamage(damage);
                                   

                                    PlayerHealth player = collider.GetComponent<PlayerHealth>();

                                    if (player != null)
                                    {
                                        //AttackAnim();
                                        RuntimeManager.PlayOneShot(testAttack);

                                        player.TakeDamage(_damage);

                                        Instantiate(_attackEffect, attackPoint.transform.position, Quaternion.identity);

                                    }


                                    //Instantiate(_attackEffect, _attackPoint.transform.position, Quaternion.identity);
                                }

                                //StartCoroutine(AttackTimer());

                            }

                            _canPerformAttack = false;
                            _nextAttack = Time.time + _attackRate;
                        }
                    }
                    else if ((_playerTarget.transform.position - transform.position).magnitude >= 7.5F)
                    {
                        if (_agent.enabled)
                        {
                            _agent.radius = _randomRadiusValue;
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
        return;
    }

    private void AttackAnim()
    {
        _animator.SetBool("Attack", true);
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

                string debugColor = "<size=14><color=orange>";
                string closeColor = "</color></size>";
                Debug.Log(debugColor + "Special attack" + closeColor);


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
        
        abilitySlider.value = _currentAbilityValue;


        if (_currentAbilityValue >= AbilityMaxValue) { _canSpecialAttack = true; return; }

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

        if(!running) 
        {
            running = true;
            _canPerformAttack = false;
            yield return new WaitForSeconds(_attackTimer);

            _canPerformAttack = true;
            running = false;
        }
        
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
        _agent.updateRotation   = true;

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
        StopCoroutine(FovRoutine());
     
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

            healthSlider.value = _health;

           
            if (_canAttack)
            {
                //_warn.CanAlertAi = true;
                SetAttack();
                return;
            }
        }
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

    private void Die()
    {
        if(_spawnHealth)
        {
            for (int i = 0; i < _healthItems; i++)
            {
                SpawnDrop(_healthDrop);
            }   
        }

        if(_spawnMana)
        {
            for (int i = 0; i < _manaItems; i++)
            {
                SpawnDrop(_manaDrop);
            }
        }
        

        if (_gemSpawnOnDeath)
        {
            Instantiate(_gemPrefab, transform.position, Quaternion.identity);
        }

        Instantiate(_death, transform.position, Quaternion.identity);

        _valuesTexts.GetKill();

        _objectiveUiScript.IncreaseEnemyDefeatedCount();

        Destroy(this.gameObject);
    }

    // Drop area
    private void SpawnDrop(GameObject drop)
    {

        Vector3 spawnPosition = transform.position +
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

    IEnumerator DamageTextDisappear()
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
        Debug.Log("Started DOT coroutine");
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

    #region Agents Info Exchange

    // 
    private void AddAttacker()
    {
        _controller.AgentStartedAttacking(gameObject); // Add agent to list
        return; 
    }

    private void RemoveAttacker() 
    {
        _controller.AgentStoppedAttacking(gameObject);
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

    #region Editor Gizmos
    private void OnDrawGizmos()
    {

#if UNITY_EDITOR

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



        #region Gizmos code

        if (showExtraGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, _attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _minDist);


            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 2);
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
#endif
    }
    #endregion
}