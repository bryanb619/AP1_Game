#region Libs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor; 
#endif

#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    /*
    #region  Variables

    // States --------------------------------------------------------------------------------------------------------->
                            private enum Ai                             { 
                                                                        Guard, 
                                                                        Patrol,
                                                                        Attack,
                                                                        Cover, 
                                                                        Search, 
                                                                        Glorykill, 
                                                                        None }
                            
        [FormerlySerializedAs("_stateAI")] [SerializeField]    private Ai                                  stateAi;

                            private enum HandleState                    { Stoped, None }
                            private HandleState                         _currentState;

                            private GameState                           _gamePlay;


    // Components ----------------------------------------------------------------------------------------------------->
        
                            // Reference to AI data
        [SerializeField]    private AiRangedData                        data;

                            // Reference to the state machine
                            private StateMachine                        _stateMachine;

                            // Reference to the NavMeshAgent
                            internal NavMeshAgent                       Agent;

        // Reference to the Outline component
        [SerializeField]    private Outline                             outlineDeactivation;

                            private WarningSystemAi                     _warn;

        [FormerlySerializedAs("_agentAI")] [SerializeField]    private Agents                              agentAi;

                            private AiHandler                           _handlerAi;
                            private bool                                _deactivateAi;

                            [FormerlySerializedAs("_dizzyText")] public TMP_Text                             dizzyText;
                            private bool                                _activeDizzy;

                            private GemManager                          _gemManager;

                            private SkinnedMeshRenderer                 _enemyMesh;

                            private ObjectiveUi                         _objectiveUiScript;

        // Combat  ---------------------------------------------------------------------------------------------------->

        // Attack 

        [FormerlySerializedAs("_shootPos")] [SerializeField]    private Transform                           shootPos;

                            private GameObject                          _bullet, _randomBullet, _specialPower;

                            private float                               _fireRate = 2f;
                            private float                               _nextFire = 0f;
                            private float                               _percentage;



        // special ability 

                            private const float                         AbilityMaxValue = 100F;

                            private float                               _currentAbilityValue;

                            private float                               _abilityIncreasePerFrame;

        // Effects

        [FormerlySerializedAs("_targetEffect")] [SerializeField]    private GameObject                          targetEffect;
        
        
        // FOV ---------------------------------------------------------------------------------------------------------------->
                            private bool                                     _canFov;            
        [Range(10, 150)]
        [SerializeField]    private float                                   radius;
        public float                                                        Radius => radius;
        [Range(50, 360)]
        [SerializeField]    private float                                   angle;
        public float                                                        Angle => angle;

        private LayerMask                                                   _targetMask;
        private LayerMask                                                   _obstructionMask;
        [SerializeField] private Transform                                  fov;
        public Transform                                                    EefovTransform => fov; // Enemy Editor FOV

        private bool                                                        _canSeePlayer;
        public bool                                                         CanSee => _canSeePlayer;


    // Drops & Death -------------------------------------------------------------------------------------------------->
        [FormerlySerializedAs("_death")] [SerializeField]    private GameObject                              death;

                            private bool                                    _gemSpawnOnDeath;
                            private GameObject                              _gemPrefab;

                            private bool                                _spawnHealth;
                            private int                                 _healthItems;
                            private GameObject                          _healthDrop;

                            private bool                                _spawnMana; 
                            private int                                 _manaItems;
                            private GameObject                          _manaDrop;

                            private int                                 _dropRadius; 


    // Health --------------------------------------------------------------------------------------------------------->

        // UI

        [FormerlySerializedAs("_healthSlider")]
        [Header("UI ")]

        [SerializeField] 
        private Slider                              healthSlider;
        private float                               _health;


    private Color                                                       _originalColor;
    public int                                                          damageBoost = 0;



    // References to enemies
    [FormerlySerializedAs("PlayerObject")] public GameObject                                                   playerObject;

    private Transform                                                   _playerTarget;
    public Transform                                                    PlayerTarget => _playerTarget;

    private PlayerHealth                                                _player; 

    private float                                                       _minDist = 3f;

    private Shooter                                                     _shooterScript;

    // AI SPEED
    private float                                                       _curSpeed;
    private Vector3                                                     _previousPos;

    private float                                                       _attackRequiredDistance = 6.5f; // 6.5

    // Patrol Points

    private int _destPoint = 0;
    [FormerlySerializedAs("_PatrolPoints")] [SerializeField] private Transform[]                                patrolPoints;



    // COMBAT //

    private float                                                       _damageEffectTime;


   

    // hide code
    [Header("Hide config")]
    private Collider[]                                                  _colliders = new Collider[10];

    [FormerlySerializedAs("HideSensitivity")] [Range(-1, 1)]
    public float hideSensitivity = 0;
    [FormerlySerializedAs("UpdateFrequency")] [Range(0.01f, 1f)][SerializeField] private float                    updateFrequency = 0.65f;

    [FormerlySerializedAs("HidableLayers")] [SerializeField] private LayerMask                                  hidableLayers;

    [Range(0, 5f)]
    private float                                                       _minObstacleHeight = 0f;

    [FormerlySerializedAs("LineOfSightChecker")] public SceneChecker                                                 lineOfSightChecker;

    private Coroutine                                                   _movementCoroutine;


  

    [FormerlySerializedAs("_abilitySlider")] [SerializeField] private Slider                                     abilitySlider;

    private ValuesTextsScript                                           _valuesTexts;

    // animation
    private Animator                                                    _animator;

    // damage over time variables
    private float                                                       _damageOverTime = 2f;
    private float                                                       _durationOfDot = 10f;

    // stunned variables
    private float                                                       _stunnedTime = 1.5f;

    // states & actions
    private bool                                                        _canGloryKill;



    private bool                                                        _canMove;

    private bool                                                        _canSpecialAttack = false; 
        
    private bool                                                        _canAttack;

    private bool                                                        _isAttacking;

    private bool                                                        _canPeformAttack;



    // performance



    // Debug // 

    [FormerlySerializedAs("_showExtraGizmos")] [SerializeField] private bool                                       showExtraGizmos; 


    #endregion

    #region Awake
    // Get references to enemies
    private void Awake()
    { 
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        _objectiveUiScript = FindObjectOfType<ObjectiveUi>();
    }

    #endregion

    #region Start
    // Create the FSM
    private void Start()
    {
        GetComponents();
        GetProfile();
        GetStates();


        // temp code
        _canMove            = true;
        _canAttack          = true;
        _isAttacking        = false;
        _canPeformAttack    = true;
        dizzyText.enabled  = false;
        _currentState       = HandleState.None; 
    }

    #region Components Sync
    private void GetComponents()
    {
        Agent               = GetComponent<NavMeshAgent>();
        _warn               = GetComponent<WarningSystemAi>();
        _handlerAi          = GetComponent<AiHandler>();

        agentAi            = GetComponentInChildren<Agents>();
        _animator           = GetComponentInChildren<Animator>();
        healthSlider       = GetComponentInChildren<Slider>();
        _enemyMesh           = GetComponentInChildren<SkinnedMeshRenderer>();

        lineOfSightChecker  = GetComponentInChildren<SceneChecker>();

        
        _player             = FindObjectOfType<PlayerHealth>();
        playerObject        = GameObject.Find("Player");
        _playerTarget       = playerObject.transform;

        _shooterScript       = playerObject.GetComponent<Shooter>();

        _valuesTexts         = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
    }
    #endregion

    #region Profile Sync
    private void GetProfile()
    {
       
        // HEALTH //
        _health = data.Health;
       
        // ATTACK //
        _fireRate = data.AttackRate;

        _minDist = data.MinDist;

        _percentage = data.Percentage; 

        // Special attack Ability

        _currentAbilityValue = data.CurrentAbilityValue;

        _abilityIncreasePerFrame = data.AbilityIncreasePerFrame;


        //specialDamage = data.SpecialDamage;

        // projectiles //

        _bullet          = data.NProjectile;
        _randomBullet    = data.RProjectile;
        _specialPower    = data.SProjectile;

        // cover //
        //fleeDistance = data.FleeDistance; 

        
        // Death & Loot //

        _gemPrefab = data.Gem;

        _spawnHealth = data.SpawnHealth;    
        _healthDrop = data.HealthDrop;
        _healthItems = data.HealthItems;    

        _spawnMana = data.SpawnMana;
        _manaDrop = data.ManaDrop;
        _manaItems = data.ManaItems;  
        
        _dropRadius = data.DropRadius;  

        // FOV //

        //radius = data.Radius;
        
        //angle = data.Angle;

        _targetMask = data.TargetMask;

        _obstructionMask = data.ObstructionMask; 

        // UI //
        healthSlider.value = _health;
        abilitySlider.value = _currentAbilityValue; 

    }
    #endregion

    #region States
    private void GetStates()
    {
        
        // Create the states
        State onGuardState = new State("Guard" ,null);

        State patrolState = new State("On Patrol", Patrol);

        State chaseState = new State("Fight",ChasePlayer);

        State coverState = new State("Cover",Cover);

        State gloryKillState = new State("Glory Kill",GloryKill);

        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                () => _canSeePlayer == true,
                //() => Debug.Log(" GUARD -> CHASE"),
                chaseState));

        // CHASE->PATROL
        chaseState.AddTransition(
            new Transition(
                () => stateAi == Ai.Patrol,
                //() => Debug.Log("CHASE -> PATROL"),
                patrolState));

       // CHASE -> GLORY KILL
        chaseState.AddTransition(
            new Transition(
                () => _canGloryKill == true,
               // () => Debug.Log("CHASE -> GLORY KILL"),
                gloryKillState));

        //  PATROL -> CHASE 
        patrolState.AddTransition(
           new Transition(
               () => stateAi == Ai.Attack,
               //() => Debug.Log("PATROL -> CHASE"),
               chaseState));

        //state machine
        _stateMachine = new StateMachine(patrolState);


        //CoverState.AddTransition(new Transition(() => _canAttack == true && canSeePlayer == false, ()=> Debug.Log("Cover State"), PatrolState));
        //CoverState.AddTransition(new Transition(() => _canAttack == true && canSeePlayer == true, () => Debug.Log("Cover State"), ChaseState));

        //ChaseState.AddTransition(new Transition(() => _canAttack == false, () => Debug.Log("Cover State"), CoverState));
    }
    #endregion

    #endregion

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        UpdateAi();
    }
    #endregion

    #region AI update
    private void UpdateAi()
    {
        switch(_handlerAi.AgentOperate)
        {
            case true:
                {
                    if(_gamePlay == GameState.Gameplay)
                    {
                        outlineDeactivation.enabled = false;

                        MinimalCheck(); // Tester

                        if(stateAi != Ai.Attack)
                        {
                            StartCoroutine(FovRoutine());
                        }
                        

                        AiSpeed();

                        //Dizzy(); 

                        Action actions = _stateMachine.Update();
                        actions?.Invoke();

                        if(_currentState == HandleState.None)
                        {
                            ResumeAgent(); 
                        }
                    }

                    
                    break;
                }
            case false:
                {
                    StopAgent();
                    break;
                }
        }
    }


    #region Agent State
    private void ResumeAgent()
    {
        
        Agent.isStopped = false;
        return;
    }

    private void PauseAgent()
    {
        //Agent.speed = 0f; 
        //Agent.Stop(); 
        
        Agent.velocity = Vector3.zero; 
        Agent.isStopped = true;
        return;
    }

    private void StopAgent()
    {
        //_gamePlay = false;
        //agent.isStopped = true;
        PauseAgent();
        StopAllCoroutines();
        return;
    }

    #endregion

    private void MinimalCheck()
    {
        if (stateAi != Ai.Attack)
        {
            StartCoroutine(DistanceCheck());
         
        }

        if (!_canGloryKill && _canAttack && stateAi != Ai.Attack)
        {
            if ((_playerTarget.transform.position - transform.position).magnitude < 2 && _canAttack)
            {
                SetAttack();
                return;
            }
        }
       
    }

    private IEnumerator DistanceCheck() 
    {
        yield return new WaitForSeconds(1f);

        if((_playerTarget.transform.position - transform.position).magnitude < 25f)
        {
            _canFov = true;
        }
        else
        {
            _canFov = false;
        }
        

    }

    private void AiSpeed()
    {
        Vector3 curMove = transform.position - _previousPos;
        _curSpeed = curMove.magnitude / Time.deltaTime;
        _previousPos = transform.position;

        if (_curSpeed > 0.2)
        {
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }

    }

    private void Dizzy()
    {
        switch(_activeDizzy) 
        {
            case true:
                {
                    ActiveDizzy(); 
                    break;
                }
            case false: 
                {
                    DisableDizzy(); 
                    break;
                }
        }
    }

    private void ActiveDizzy()
    {
        dizzyText.enabled = true;

        dizzyText.ForceMeshUpdate();
        
        var textInfo = dizzyText.textInfo;

        for(int i = 0; i < textInfo.characterCount; i++) 
        {
            var charInfo = textInfo.characterInfo[i]; 
            
            if(!charInfo.isVisible ) 
            {
                continue;
            }
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + 
                    new Vector3
                    (0, Mathf.Sin(Time.time * 2f + orig.x * 0.2f)* 10f, 0); 
                    
            }
        }

        for(int i = 0;i < textInfo.meshInfo.Length;i++)
        {
            var meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;

            dizzyText.UpdateGeometry(meshInfo.mesh, i); 
        }


    }

    private void DisableDizzy()
    {
        dizzyText.enabled = false;
        return; 
    }


    void OnPlayerWarning(Vector3 target)
    {
        if(_canAttack) 
        {
            SetAttack();
            return;
        }
        
    }
    #endregion

    #region AI ACTIONS
    private void HandleGainSight(Transform target)
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
        }
        _playerTarget = target;

        _movementCoroutine = StartCoroutine(Hide(target));
    }


    // Chase 
    private void ChasePlayer()
    {
        //StartAttacking();

        switch(_canSpecialAttack)
        {
            case true:
                {
                    
                    if ((_playerTarget.transform.position - transform.position).magnitude >= _attackRequiredDistance)  //
                    {
                        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
                        PauseAgent();

                        //StartAttacking();
                        if (_canAttack) { SpecialAttack();}
                    }

                    else if ((_playerTarget.transform.position - transform.position).magnitude < 
                             _attackRequiredDistance && !_canSpecialAttack) //|| currentAbilityValue < ABILITY_MAX_VALUE)
                    {
                        if(_canAttack && _currentState == HandleState.None) 
                        { 
                            ResumeAgent();
                            GetDistance(9F);
                        }
                    }

                    break;
                }
            case false:
                {
                    
                    if ((_playerTarget.transform.position - transform.position).magnitude >= _attackRequiredDistance)
                    {
                        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
                        PauseAgent();
                        //StartAttacking();
                        if(_canAttack && !_canSpecialAttack) { Attack(); }
                    }

                    else if ((_playerTarget.transform.position 
                              - transform.position).magnitude < _attackRequiredDistance && !_canSpecialAttack) 
                    {
                        if (_canAttack && _currentState == HandleState.None)
                        {
                            ResumeAgent();
                            GetDistance(3.5F);
                        }
                            
                    }

                    CoolDoownPower();
                    break; 
                }
        }
        //print("ATTACK");
    }

    private void Attack()
    {
        Agent.speed = 4f;

        if (Time.time > _nextFire)
        {
            //transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));

            //float randomFloat = UnityEngine.Random.value;

            //print("percentage is: "+randomPercentage);

            if(UnityEngine.Random.value < _percentage && _canPeformAttack)
            {
                _animator.SetBool("isAttacking", true);

                string debugAttack = "<size=12><color=yellow>";
                string closeAttack = "</color></size>";
                Debug.Log(debugAttack + "Attack 2: " + closeAttack);

                Instantiate(_randomBullet, shootPos.position, shootPos.rotation);
                StartCoroutine(AttackTimer()); 
                
            }
            else if(_canPeformAttack)
            {
                _animator.SetBool("isAttacking", true);

                string debugAttack = "<size=12><color=green>";
                string closeAttack = "</color></size>";
                Debug.Log(debugAttack + "Attack: " + closeAttack);

                Instantiate(_bullet, shootPos.position, shootPos.rotation);
                StartCoroutine(AttackTimer());

            }
            _nextFire = Time.time + _fireRate;
        }
        else
        {
            _animator.SetBool("isAttacking", false);
            
        }
    }

    private void SpecialAttack()
    {
        PauseAgent();

        string debugAttack = "<size=12><color=purple>";
        string closeAttack = "</color></size>";
        Debug.Log(debugAttack + "Special Attack: " + closeAttack);

        Instantiate(_specialPower, shootPos.position, shootPos.rotation);

        _currentAbilityValue = 0;
        abilitySlider.value = _currentAbilityValue;
        _canSpecialAttack = false;
        _animator.SetBool("isAttacking", false);
        StartCoroutine(SpecialAttackTimer());

    }


    private void CoolDoownPower()
    {

        if (_currentAbilityValue >= AbilityMaxValue)
        {
            _canSpecialAttack = true;
        }
        else
        {
            _currentAbilityValue = Mathf.Clamp(_currentAbilityValue + (_abilityIncreasePerFrame * Time.deltaTime), 0.0f, AbilityMaxValue);   
        }

        abilitySlider.value = _currentAbilityValue;
        //print(currentAbilityValue);
    }

    private IEnumerator SpecialAttackTimer()
    {
        bool isruning = false; 

        if(!isruning) 
        {
            isruning = true;
            _canPeformAttack = false;
            _canAttack = false;
            _activeDizzy = true; 
            yield return new WaitForSeconds(4f);
            _activeDizzy = false;
            _canAttack = true;
            _canPeformAttack = true;
            ResumeAgent();  
            isruning = false;
        }
        
    }

    private IEnumerator AttackTimer()
    {
        bool isruning = false;

        if (!isruning)
        {
            isruning = true;
            _canPeformAttack = false;
            yield return new WaitForSeconds(1f);
            _canPeformAttack = true;
            isruning = false;
        }


    }
    private void GetDistance(float speed)
    {
        Agent.speed = speed;
        Agent.acceleration = 12;
        

        if (_curSpeed <= 1 && CanSee)          
        {
            //transform.LookAt(_playerTarget.position);
            Attack();
            return;
        }

        HandleGainSight(PlayerTarget);

    }

    private void Cover()
    {
        StopAttacking(); 
        HandleGainSight(PlayerTarget);
    }
    
    #region Get Distance

    private IEnumerator Hide(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(updateFrequency);
        while (true)
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, lineOfSightChecker.collider.radius, _colliders, hidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(_colliders[i].transform.position, target.position) < _attackRequiredDistance || _colliders[i].bounds.size.y < _minObstacleHeight)
                {
                    _colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(_colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(_colliders[i].transform.position, out NavMeshHit hit, 2f, Agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, Agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (target.position - hit.position).normalized) < hideSensitivity)
                    {
                        Agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(_colliders[i].transform.position - (target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, Agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, Agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized) < hideSensitivity)
                            {
                                Agent.SetDestination(hit2.position);
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
            return Vector3.Distance(Agent.transform.position, a.transform.position).CompareTo(Vector3.Distance(Agent.transform.position, b.transform.position));
        }
    }

    #endregion 


    private void Patrol()
    {
        if(_canMove)
        {
            Agent.autoBraking = false;
            Agent.stoppingDistance = 0.1f;

            if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
            {
                GotoNetPoint();
            }
        }
    }

    private void GotoNetPoint()
    {
        
        Agent.speed = 3f;
        // Returns if no points have been set up
        if (patrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        Agent.destination = patrolPoints[_destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        _destPoint = (_destPoint + 1) % patrolPoints.Length;
    }

    private void GloryKill()
    {
        StopAttacking();
        Agent.radius = 1f;
        Agent.isStopped = true;
        return;
    }

    #endregion

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
        bool isRunning = false;

        if(!isRunning) 
        {
            isRunning = true;

            Collider[] rangeChecks = Physics.OverlapSphere(fov.position, radius, _targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - fov.position).normalized;

                if (Vector3.Angle(fov.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(fov.position, target.position);

                    if (!Physics.Raycast(fov.position, directionToTarget, distanceToTarget, _obstructionMask))
                    {
                        _canSeePlayer = true;
                        SetAttack();
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

        isRunning = false;

    }
    #endregion

    #region Health
    public void TakeDamage(int damage, WeaponType type)
    {
        //health -= (_damage + damageBoost);
       
        if (_health <= 0)
        {
            Die();
        }

        else if (_health > 0)
        {
            
            //GetPlayer();
            switch (type)
            {
                case WeaponType.Normal:
                    {
                        _health -= damage + damageBoost;

                        _damageEffectTime = 0.5f; 
                        StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Fire: //Q ability
                    {
                        _health -= damage + damageBoost;

                        StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Ice: //W ability
                    {
                        _health -= damage + damageBoost;
                        
                        if(_shooterScript.WUpgraded == true)
                        {
                            _damageEffectTime = 1f;
                            StartCoroutine(DamageOverTime(_damageOverTime, _durationOfDot));
                        }
                        else
                            _health -= damage + damageBoost;
                            Instantiate(targetEffect, transform.position, transform.rotation);
                        StartCoroutine(HitFlash());

                        break;
                    }

                case WeaponType.Dash: //E ability
                    {
                        _health -= damage + damageBoost;

                        StartCoroutine(HitFlash());

                        break; 
                    }

                case WeaponType.Thunder: //R ability
                    {
                        _health -= damage + damageBoost;

                        if(_shooterScript.RUpgraded == true)
                        {
                            StartCoroutine(Stfs(_stunnedTime));
                        }
                        else
                            StartCoroutine(HitFlash());

                        break; 
                    }

                default: break;
            }

            //StartCoroutine(DamageEffect()); 

            healthSlider.value = _health;

            //float randomFloat = UnityEngine.Random.value;
            /*
            if (randomFloat <= 0.1f)
            {
                //print("STARTED CHANCE");
                //print(randomFloat);

                StartCoroutine(STFS(stunnedTime));
            }
            
            //HealthCheck();

            // print("damage :" + health);

            if (_canAttack) 
            {
                _warn.CanAlertAi = true;
                SetAttack();
            }
            return;
        }
        // Debug.Log("enemy shot with " + (_damage + damageBoost) + " damage");
    }


    private void HealthCheck()
    {
        if(_health >= 15)
        {
            SetAttack();
            return;
        }
        else if(_health <20 )
        {
            _canSpecialAttack = false;
            return;
        }

        else if(_health <= 14)
        {
            SetGloryKill();
            return;
        }
    }


    private IEnumerator DamageEffect()
    {
        bool damageEffect = false;

        if (!damageEffect)
        {
            // control effect & stop
            damageEffect = true;

            //print("suffered hit");

            // STOP AGENT
            PauseAgent();

            // STOP ATTACK 
            _canAttack = false;

            yield return new WaitForSeconds(_damageEffectTime);

            // RESUME AGENT
            ResumeAgent();

            // Start || resume Attack 
            SetAttack();

            // control effect & stop
            damageEffect = false;

            //print("in coroutine 2");
        }


        //print("out of if");
    }


    private void Die()
    {
        Instantiate(death, transform.position, Quaternion.identity);


        if(_spawnHealth)
        {
            for (int i = 0; i < _healthItems; i++)
            {

                Vector3 spawnPosition = transform.position +
                    new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius), 0f,
                    UnityEngine.Random.Range(-_dropRadius, _dropRadius));

                Instantiate(_healthDrop, spawnPosition, Quaternion.identity);
            }
        }

        if (_spawnMana)
        {
            for (int i = 0; i < _manaItems; i++)
            {
                Vector3 spawnPosition = transform.position +
                      new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius), 0f,
                      UnityEngine.Random.Range(-_dropRadius, _dropRadius));

                Instantiate(_manaDrop, spawnPosition, Quaternion.identity);

            }
        }

        if (_gemSpawnOnDeath)
        { 
            Instantiate(_gemPrefab, transform.position, Quaternion.identity);
            Debug.Log("Spawned Gem");
        }

        _objectiveUiScript.IncreaseEnemyDefeatedCount();

        _valuesTexts.GetKill();

        Destroy(gameObject);

        Debug.Log("Enemy died");
    }
    #endregion

    #region Combat IEnumerators

    private IEnumerator HitFlash()
    {
        bool isrunning = false;

        if (!isrunning)
        {
            isrunning = true;
            Color color = _enemyMesh.material.color;

            _enemyMesh.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _enemyMesh.material.color = color;

            isrunning = false;
        }

    }

    private IEnumerator Stfs(float value)
    {
        bool stfsEffect = false;

        if (!stfsEffect)
        {
            stfsEffect = true;


            print("STARTED STFS COROUTINE SUCCESFULLY");

            _canAttack = false;
            _currentState = HandleState.Stoped;
            StopAgent();

            yield return new WaitForSeconds(value);


            _currentState = HandleState.None;

            //HandleStateAI(false);
            
            ResumeAgent();
            _canAttack = true;

            stfsEffect = false;
        }
        

        /*
        _canMove = false;

        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f); ;
        yield return new WaitForSeconds(value);

        GetComponent<Renderer>().material.color = originalColor;
        _canMove = true;
        
    }

    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _durationOfDot)
        {
            _health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(2.5f);
            elapsedTime += 2.5f;

        }

    }
    #endregion

    #region Actions Reset

    private void SetGuard()
    {
        stateAi = Ai.Guard;
        
    }
    private void SetPatrol()
    {
        stateAi = Ai.Patrol;
    }
    private void SetAttack()
    {
        stateAi = Ai.Attack;

        Agent.speed = 4f;
    }
    private void SetCover()
    {
        stateAi = Ai.Cover;
        return;
    }
    private void SetGloryKill()
    {
        stateAi = Ai.Glorykill;
        return;
    }

    #endregion

    #region Agents Info Exchange

    public void StartAttacking()
    {
       if(!_isAttacking) 
       {
            agentAi.StartAttacking();
            return;
       }
    }

    public void StopAttacking()
    {
        if(_isAttacking)
        {
            agentAi.StopAttacking();
            SetPatrol();
            return;
        }
        
    }
    #endregion

    #region GameState
    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
        {
            case GameState.Gameplay:
                {
                    
                    if(_currentState == HandleState.None)
                    {
                        _gamePlay = GameState.Gameplay;
                        ResumeAgent();
                    }
                    
                    break;
                }
            case GameState.Paused:
                {
                    
                    if (_currentState == HandleState.None)
                    {
                        _gamePlay = GameState.Paused;
                        PauseAgent();
                    }
                    break;
                }
        }

        //throw new NotImplementedException();
    }
    #endregion

    #region Destroy
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion

#if UNITY_EDITOR
    #region Editor Gizmos
    private void OnDrawGizmos()
    {




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
                    Handles.Label(fov.transform.position + Vector3.up, "Guard" + "  Gameplay: " + _gamePlay, green);
                    break;
                }
            case AI._PATROL:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "Patrol" + "  Gameplay: " + _gamePlay, blue);
                    break;
                }
            case AI._ATTACK:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "Attack" + "  Gameplay: " + _gamePlay + "Peform attack: " + _canPeformAttack, red);
                    break;
                }
            case AI._SEARCH:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "Search" + "  Gameplay: " + _gamePlay, yellow);
                    break;
                }
            case AI._COVER:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "Cover" + "  Gameplay: " + _gamePlay, cyan);
                    break;
                }
            case AI._GLORYKILL:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "Glory Kill" + "  Gameplay: " + _gamePlay);
                    break;
                }
            case AI._NONE:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "NONE" + "  Gameplay: " + _gamePlay);
                    break;
                }
            default:
                {
                    Handles.Label(fov.transform.position + Vector3.up, "NO STATE FOUND" + "  Gameplay: " + _gamePlay);
                    break;
                }
        }
        #endregion

    }
    #endregion
#endif
*/
}