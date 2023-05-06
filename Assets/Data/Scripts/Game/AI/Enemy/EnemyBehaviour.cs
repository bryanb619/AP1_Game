/*Authors 
 * Steven Hall & Diogo Freire 
 * 
 * */

#region Libs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;

using UnityEditor; // comment this on build
using TMPro;
using System.Runtime.CompilerServices;

#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    #region  Variables

    // States ------------------------------------------------------------------------------------------------------------->

                            private enum AI                             { _GUARD, _PATROL, _ATTACK, _COVER, _SEARCH, _GLORYKILL, _NONE }
        [SerializeField]    private AI                                  _stateAI;

                            private enum HandleState                    { _STOPED, _NONE }
                            private HandleState                         _currentState;

                            private GameState                           _gamePlay;


    // Components ------------------------------------------------------------------------------------------------------------->
        
                            // Reference to AI data
        [SerializeField]    private AIRangedData                        data;

                            // Reference to the state machine
                            private StateMachine                        stateMachine;

                            // Reference to the NavMeshAgent
                            internal NavMeshAgent                       agent;

        // Reference to the Outline component
        [SerializeField]    private Outline                             outlineDeactivation;

                            private WarningSystemAI                     _warn;

        [SerializeField]    private Agents                              _agentAI;

                            private AIHandler                           _handlerAI;
                            private bool                                _deactivateAI;

                            public TMP_Text                             _dizzyText;
                            private bool                                _activeDizzy;

                            private GemManager                          gemManager;

                            private SkinnedMeshRenderer                 enemyMesh;


        // Combat  ------------------------------------------------------------------------------------------------------------->

        // Attack 

        [SerializeField]
        private Transform                           _shootPos;

        private GameObject                          bullet, randomBullet, specialPower;

        private float                               fireRate = 2f;
        private float                               nextFire = 0f;
        private float                               percentage;



        // special ability 

        private const float                         ABILITY_MAX_VALUE = 100F;

        private float                               currentAbilityValue;

        private float                               abilityIncreasePerFrame;

        // Effects

        [SerializeField]
        private GameObject                          _targetEffect;


    // Drops & Death ------------------------------------------------------------------------------------------------------------->
        [SerializeField] 
        private GameObject                          _death;

        private bool                                gemSpawnOnDeath;
        private GameObject                          gemPrefab;

        private bool                                _spawnHealth;
        private int                                 _healthItems;
        private GameObject                          _healthDrop;

        private bool                                _spawnMana; 
        private int                                 _manaItems;
        private GameObject                          _manaDrop;

        private int                                 _dropRadius; 


    // Health ------------------------------------------------------------------------------------------------------------->

        // UI

        [Header("UI ")]

        [SerializeField] 
        private Slider                              _healthSlider;
        private float                               health;


    private Color                                                       originalColor;
    public int                                                          damageBoost = 0;



    // References to enemies
    public GameObject                                                   PlayerObject;

    private Transform                                                   _playerTarget;
    public Transform                                                    PlayerTarget => _playerTarget;

    private PlayerHealth                                                _player; 

    private float                                                       minDist = 3f;

    private Shooter                                                     shooterScript;

    // AI SPEED
    private float                                                       curSpeed;
    private Vector3                                                     previousPos;

    private float                                                       AttackRequiredDistance = 6.5f; // 6.5

    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[]                                _PatrolPoints;


    private bool                                                        _fov; 
    //[Range(10, 150)]
    private float                                                       radius;
    public float                                                        Radius => radius;
    //[Range(50, 360)]
    private float                                                       angle;
    public float                                                        Angle => angle;

    private LayerMask                                                   targetMask;
    private LayerMask                                                   obstructionMask;
    [SerializeField] private Transform                                  FOV;
    public Transform                                                    EEFOV => FOV; // Enemy Editor FOV

    private bool                                                        canSeePlayer;
    public bool                                                         canSee => canSeePlayer;

    // COMBAT //

    private float                                                       damageEffectTime;


   

    // hide code
    [Header("Hide config")]
    private Collider[]                                                  Colliders = new Collider[10];

    [Range(-1, 1)]
    public float HideSensitivity = 0;
    [Range(0.01f, 1f)][SerializeField] private float                    UpdateFrequency = 0.65f;

    [SerializeField] private LayerMask                                  HidableLayers;

    [Range(0, 5f)]
    private float                                                       MinObstacleHeight = 0f;

    public SceneChecker                                                 LineOfSightChecker;

    private Coroutine                                                   MovementCoroutine;


  

    [SerializeField] private Slider                                     _abilitySlider;

    private ValuesTextsScript                                           valuesTexts;

    // animation
    private Animator                                                    _animator;

    // damage over time variables
    private float                                                       damageOverTime = 2f;
    private float                                                       durationOfDOT = 10f;

    // stunned variables
    private float                                                       stunnedTime = 1.5f;

    // states & actions
    private bool                                                        _canGloryKill;



    private bool                                                        _canMove;

    private bool                                                        _canSpecialAttack = false; 
        
    private bool                                                        _canAttack;

    private bool                                                        _isAttacking;

    private bool                                                        _canPeformAttack;



    // performance



    // Debug // 

    [SerializeField] private bool                                       _showExtraGizmos; 


    #endregion

    #region Awake
    // Get references to enemies
    private void Awake()
    { 
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
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
        _dizzyText.enabled  = false;
        _currentState       = HandleState._NONE; 
    }

    #region Components Sync
    private void GetComponents()
    {
        agent               = GetComponent<NavMeshAgent>();
        _warn               = GetComponent<WarningSystemAI>();
        _handlerAI          = GetComponent<AIHandler>();

        _agentAI            = GetComponentInChildren<Agents>();
        _animator           = GetComponentInChildren<Animator>();
        _healthSlider       = GetComponentInChildren<Slider>();
        enemyMesh           = GetComponentInChildren<SkinnedMeshRenderer>();

        LineOfSightChecker  = GetComponentInChildren<SceneChecker>();

        
        _player             = FindObjectOfType<PlayerHealth>();
        PlayerObject        = GameObject.Find("Player");
        _playerTarget       = PlayerObject.transform;

        shooterScript       = PlayerObject.GetComponent<Shooter>();

        valuesTexts         = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
    }
    #endregion

    #region Profile Sync
    private void GetProfile()
    {
       
        // HEALTH //
        health = data.Health;
       
        // ATTACK //
        fireRate = data.AttackRate;

        minDist = data.MinDist;

        percentage = data.Percentage; 

        // Special attack Ability

        currentAbilityValue = data.CurrentAbilityValue;

        abilityIncreasePerFrame = data.AbilityIncreasePerFrame;


        //specialDamage = data.SpecialDamage;

        // projectiles //

        bullet          = data.N_projectile;
        randomBullet    = data.R_projectile;
        specialPower    = data.S_Projectile;

        // cover //
        //fleeDistance = data.FleeDistance; 

        
        // Death & Loot //

        gemPrefab = data.Gem;

        _spawnHealth = data.SpawnHealth;    
        _healthDrop = data.HealthDrop;
        _healthItems = data.HealthItems;    

        _spawnMana = data.SpawnMana;
        _manaDrop = data.ManaDrop;
        _manaItems = data.ManaItems;  
        
        _dropRadius = data.DropRadius;  

        // FOV //

        radius = data.Radius;
        
        angle = data.Angle;

        targetMask = data.TargetMask;

        obstructionMask = data.ObstructionMask; 

        // UI //
        _healthSlider.value = health;
        _abilitySlider.value = currentAbilityValue; 

    }
    #endregion

    #region States
    private void GetStates()
    {
        
        // Create the states
        State onGuardState = new State("Guard" ,null);

        State PatrolState = new State("On Patrol", Patrol);

        State ChaseState = new State("Fight",ChasePlayer);

        State CoverState = new State("Cover",Cover);

        State GloryKillState = new State("Glory Kill",GloryKill);

        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                () => canSeePlayer == true,
                //() => Debug.Log(" GUARD -> CHASE"),
                ChaseState));

        // CHASE->PATROL
        ChaseState.AddTransition(
            new Transition(
                () => _stateAI == AI._PATROL,
                //() => Debug.Log("CHASE -> PATROL"),
                PatrolState));

       // CHASE -> GLORY KILL
        ChaseState.AddTransition(
            new Transition(
                () => _canGloryKill == true,
               // () => Debug.Log("CHASE -> GLORY KILL"),
                GloryKillState));

        //  PATROL -> CHASE 
        PatrolState.AddTransition(
           new Transition(
               () => _stateAI == AI._ATTACK,
               //() => Debug.Log("PATROL -> CHASE"),
               ChaseState));

        //state machine
        stateMachine = new StateMachine(PatrolState);


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
        UpdateAI();
    }
    #endregion

    #region AI update
    private void UpdateAI()
    {
        switch(_handlerAI.AgentOperate)
        {
            case true:
                {
                    if(_gamePlay == GameState.Gameplay)
                    {
                        outlineDeactivation.enabled = false;

                        MinimalCheck(); // Tester
                        if(_fov)
                        {
                            StartCoroutine(FOVRoutine());
                        }
                        

                        AISpeed();

                        Dizzy(); 

                        Action actions = stateMachine.Update();
                        actions?.Invoke();

                        if(_currentState == HandleState._NONE)
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
        
        agent.isStopped = false;
        return;
    }

    private void PauseAgent()
    {
        //Agent.speed = 0f; 
        //Agent.Stop(); 
        
        agent.velocity = Vector3.zero; 
        agent.isStopped = true;
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
        if (!_canGloryKill && _canAttack)
        {
            if ((_playerTarget.transform.position - transform.position).magnitude < minDist && _canAttack)
            {
                SetAttack();
                return;
            }
        }
       
    }

    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;

        if (curSpeed > 0.2)
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
        _dizzyText.enabled = true;

        _dizzyText.ForceMeshUpdate();
        
        var textInfo = _dizzyText.textInfo;

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

            _dizzyText.UpdateGeometry(meshInfo.mesh, i); 
        }


    }

    private void DisableDizzy()
    {
        _dizzyText.enabled = false;
        return; 
    }


    void OnPlayerWarning(Vector3 Target)
    {
        if(_canAttack) 
        {
            SetAttack();
            return;
        }
        
    }
    #endregion

    #region AI ACTIONS
    private void HandleGainSight(Transform Target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        _playerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }


    // Chase 
    private void ChasePlayer()
    {
        //StartAttacking();

        switch(_canSpecialAttack)
        {
            case true:
                {
                    

                    if ((_playerTarget.transform.position - transform.position).magnitude >= AttackRequiredDistance)  //
                    {
                        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
                        PauseAgent();

                        //StartAttacking();
                        if (_canAttack) { SpecialAttack();}
                    }

                    else if ((_playerTarget.transform.position - transform.position).magnitude < AttackRequiredDistance && !_canSpecialAttack) //|| currentAbilityValue < ABILITY_MAX_VALUE)
                    {
                        if(_canAttack && _currentState == HandleState._NONE) 
                        { 
                            ResumeAgent();
                            GetDistance(9F);
                        }

                        
                    }

                    break;
                }
            case false:
                {
                    

                    if ((_playerTarget.transform.position - transform.position).magnitude >= AttackRequiredDistance)
                    {
                        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
                        PauseAgent();
                        //StartAttacking();
                        if(_canAttack && !_canSpecialAttack) { Attack(); }
                    }

                    else if ((_playerTarget.transform.position - transform.position).magnitude < AttackRequiredDistance && !_canSpecialAttack) //|| currentAbilityValue < ABILITY_MAX_VALUE)
                    {
                        if (_canAttack && _currentState == HandleState._NONE)
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
        

        if (Time.time > nextFire)
        {
            //transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));

            //float randomFloat = UnityEngine.Random.value;

            //print("percentage is: "+randomPercentage);

            if(UnityEngine.Random.value < percentage && _canPeformAttack)
            {
                _animator.SetBool("isAttacking", true);

                string DebugAttack = "<size=12><color=yellow>";
                string closeAttack = "</color></size>";
                Debug.Log(DebugAttack + "Attack 2: " + closeAttack);

                Instantiate(randomBullet, _shootPos.position, _shootPos.rotation);
                StartCoroutine(AttackTimer()); 
                
            }
            else if(_canPeformAttack)
            {
                _animator.SetBool("isAttacking", true);

                string DebugAttack = "<size=12><color=green>";
                string closeAttack = "</color></size>";
                Debug.Log(DebugAttack + "Attack: " + closeAttack);

                Instantiate(bullet, _shootPos.position, _shootPos.rotation);
                StartCoroutine(AttackTimer());

            }
            nextFire = Time.time + fireRate;
        }
        else
        {
            _animator.SetBool("isAttacking", false);
            
        }
    }

    private void SpecialAttack()
    {
        PauseAgent();

        string DebugAttack = "<size=12><color=purple>";
        string closeAttack = "</color></size>";
        Debug.Log(DebugAttack + "Special Attack: " + closeAttack);

        Instantiate(specialPower, _shootPos.position, _shootPos.rotation);

        currentAbilityValue = 0;
        _abilitySlider.value = currentAbilityValue;
        _canSpecialAttack = false;
        _animator.SetBool("isAttacking", false);
        StartCoroutine(SpecialAttackTimer());

        /*
        float _attackRange = 3f; 

        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
        //agent.SetDestination(_playerTarget.position);

        //agent.stoppingDistance = 3.8f;

        Collider[] hitEnemies = Physics.OverlapSphere(_shootPos.position, _attackRange, targetMask);

        foreach (Collider collider in hitEnemies)
        {
            //Debug.Log(DebugColor + "Special attack" + closeColor);
            //print(currentAbilityValue);



            string DebugColor = "<size=14><color=orange>";
            string closeColor = "</color></size>";
            Debug.Log(DebugColor + "Special Attack" + closeColor);

            _player.TakeDamage(specialDamage);
            Instantiate(s_damageEffect, _shootPos.position, Quaternion.identity);

            currentAbilityValue = 0;
            _abilitySlider.value = currentAbilityValue;
            _canSpecialAttack = false;
            StartCoroutine(SpecialAttackTimer());
        }
        */
        /*
        if ((_playerTarget.transform.position - transform.position).magnitude <= 4f)
        {
             // look at ignoring player Y AXIS

            //Debug.Log(DebugColor + "Special attack" + closeColor);
            //print(currentAbilityValue);



            string DebugColor = "<size=14><color=orange>";
            string closeColor = "</color></size>";
            Debug.Log(DebugColor + "Special Attack" + closeColor);

            _player.TakeDamage(specialDamage);
            Instantiate(s_damageEffect, _shootPos.position, Quaternion.identity);   

            currentAbilityValue = 0;
            _abilitySlider.value = currentAbilityValue;
            _canSpecialAttack = false;
            StartCoroutine(SpecialAttackTimer());
            return; 
        }
        */
    }


    private void CoolDoownPower()
    {

        if (currentAbilityValue >= ABILITY_MAX_VALUE)
        {
            _canSpecialAttack = true;
        }
        else
        {
            currentAbilityValue = Mathf.Clamp(currentAbilityValue + (abilityIncreasePerFrame * Time.deltaTime), 0.0f, ABILITY_MAX_VALUE);   
        }

        _abilitySlider.value = currentAbilityValue;
        //print(currentAbilityValue);
    }

    private IEnumerator SpecialAttackTimer()
    {
        bool ISRUNING = false; 

        if(!ISRUNING) 
        {
            ISRUNING = true;
            _canPeformAttack = false;
            _canAttack = false;
            _activeDizzy = true; 
            yield return new WaitForSeconds(4f);
            _activeDizzy = false;
            _canAttack = true;
            _canPeformAttack = true;
            ResumeAgent();  
            ISRUNING = false;
        }
        
    }

    private IEnumerator AttackTimer()
    {
        bool ISRUNING = false;

        if (!ISRUNING)
        {
            ISRUNING = true;
            _canPeformAttack = false;
            yield return new WaitForSeconds(1f);
            _canPeformAttack = true;
            ISRUNING = false;
        }


    }
    private void GetDistance(float SPEED)
    {
        agent.speed = SPEED;
        agent.acceleration = 12;
        

        if (curSpeed <= 1 && canSee)          
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

    private IEnumerator Hide(Transform Target)
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateFrequency);
        while (true)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(Colliders[i].transform.position, Target.position) < AttackRequiredDistance || Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(Colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, 2f, agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSensitivity)
                    {
                        agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSensitivity)
                            {
                                agent.SetDestination(hit2.position);
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
            return Vector3.Distance(agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(agent.transform.position, B.transform.position));
        }
    }

    #endregion 


    private void Patrol()
    {
        if(_canMove)
        {
            agent.autoBraking = false;
            agent.stoppingDistance = 0.1f;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNetPoint();
            }
        }
    }

    private void GotoNetPoint()
    {
        
        agent.speed = 3f;
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

    private void GloryKill()
    {
        StopAttacking();
        agent.radius = 1f;
        agent.isStopped = true;
        return;
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

    #region Health
    public void TakeDamage(int _damage, WeaponType _type)
    {
        //health -= (_damage + damageBoost);
       
        if (health <= 0)
        {
            Die();
        }

        else if (health > 0)
        {
            
            //GetPlayer();
            switch (_type)
            {
                case WeaponType.Normal:
                    {
                        health -= _damage + damageBoost;

                        damageEffectTime = 0.5f; 
                        StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Fire: //Q ability
                    {
                        health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Ice: //W ability
                    {
                        health -= _damage + damageBoost;
                        
                        if(shooterScript.wUpgraded == true)
                        {
                            damageEffectTime = 1f;
                            StartCoroutine(DamageOverTime(damageOverTime, durationOfDOT));
                        }
                        else
                            health -= _damage + damageBoost;
                            Instantiate(_targetEffect, transform.position, transform.rotation);
                        StartCoroutine(HitFlash());

                        break;
                    }

                case WeaponType.Dash: //E ability
                    {
                        health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());

                        break; 
                    }

                case WeaponType.Thunder: //R ability
                    {
                        health -= _damage + damageBoost;

                        if(shooterScript.rUpgraded == true)
                        {
                            StartCoroutine(STFS(stunnedTime));
                        }
                        else
                            StartCoroutine(HitFlash());

                        break; 
                    }

                default: break;
            }

            //StartCoroutine(DamageEffect()); 

            _healthSlider.value = health;

            //float randomFloat = UnityEngine.Random.value;
            /*
            if (randomFloat <= 0.1f)
            {
                //print("STARTED CHANCE");
                //print(randomFloat);

                StartCoroutine(STFS(stunnedTime));
            }
            */
            //HealthCheck();

            // print("damage :" + health);

            if (_canAttack) 
            {
                _warn.canAlertAI = true;
                SetAttack();
            }
            return;
        }
        // Debug.Log("enemy shot with " + (_damage + damageBoost) + " damage");
    }


    private void HealthCheck()
    {
        if(health >= 15)
        {
            SetAttack();
            return;
        }
        else if(health <20 )
        {
            _canSpecialAttack = false;
            return;
        }

        else if(health <= 14)
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

            yield return new WaitForSeconds(damageEffectTime);

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
        Instantiate(_death, transform.position, Quaternion.identity);


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

        if (gemSpawnOnDeath)
        { 
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
            Debug.Log("Spawned Gem");
        }
        Destroy(gameObject);

        valuesTexts.GetKill();

        Debug.Log("Enemy died");
    }
    #endregion

    #region Combat IEnumerators

    private IEnumerator HitFlash()
    {
        bool ISRUNNING = false;

        if (!ISRUNNING)
        {
            ISRUNNING = true;
            Color COLOR = enemyMesh.material.color;

            enemyMesh.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            enemyMesh.material.color = COLOR;

            ISRUNNING = false;
        }

    }

    private IEnumerator STFS(float value)
    {
        bool STFS_EFFECT = false;

        if (!STFS_EFFECT)
        {
            STFS_EFFECT = true;


            print("STARTED STFS COROUTINE SUCCESFULLY");

            _canAttack = false;
            _currentState = HandleState._STOPED;
            StopAgent();

            yield return new WaitForSeconds(value);


            _currentState = HandleState._NONE;

            //HandleStateAI(false);
            
            ResumeAgent();
            _canAttack = true;

            STFS_EFFECT = false;
        }
        

        /*
        _canMove = false;

        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f); ;
        yield return new WaitForSeconds(value);

        GetComponent<Renderer>().material.color = originalColor;
        _canMove = true;
        */
    }

    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
        float elapsedTime = 0f;
        while (elapsedTime < durationOfDOT)
        {
            health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(2.5f);
            elapsedTime += 2.5f;

        }

    }
    #endregion

    #region Actions Reset

    private void SetGuard()
    {
        _stateAI = AI._GUARD;
        _fov = true;
        return;
    }
    private void SetPatrol()
    {
        _stateAI = AI._PATROL;
        _fov = true; 
        return;

    }
    private void SetAttack()
    {
        _stateAI = AI._ATTACK;
        _fov = false;

        agent.speed = 4f;
        return;
    }
    private void SetCover()
    {
        _stateAI = AI._COVER;
        return;
    }
    private void SetGloryKill()
    {
        _stateAI = AI._GLORYKILL;
        return;
    }

    #endregion

    #region Agents Info Exchange

    public void StartAttacking()
    {
       if(!_isAttacking) 
       {
            _agentAI.StartAttacking();
            return;
       }
    }

    public void StopAttacking()
    {
        if(_isAttacking)
        {
            _agentAI.StopAttacking();
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
                    
                    if(_currentState == HandleState._NONE)
                    {
                        _gamePlay = GameState.Gameplay;
                        ResumeAgent();
                    }
                    
                    break;
                }
            case GameState.Paused:
                {
                    
                    if (_currentState == HandleState._NONE)
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
                    Handles.Label(FOV.transform.position + Vector3.up, "Guard" + "  Gameplay: " + _gamePlay, green);
                    break;
                }
            case AI._PATROL:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Patrol" + "  Gameplay: " + _gamePlay, blue);
                    break;
                }
            case AI._ATTACK:
                {
                    Handles.Label(FOV.transform.position + Vector3.up, "Attack" + "  Gameplay: " + _gamePlay + "Peform attack: " + _canPeformAttack, red);
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