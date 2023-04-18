#region Library
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;

using UnityEditor; // comment this before build
using FMODUnity;

#endregion

public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables

    // States ------------------------------------------------------------------------------------->

            // AI states
            private enum AI                     {_GUARD, _PATROL, _ATTACK, _COVER, _GLORYKILL, _NONE}
            private AI                          _stateAI;
            
            // Handle state avoids conflict with _gameState variable
            private enum HandleState            {_STOPED, _NONE}
            private HandleState                 _currentState;

            // Game State
            private GameState                   _gameState;

    // Components --------------------------------------------------------------------------------->

    [Header("AI Profile")] 

        [SerializeField]
            // AI profile data
            private AIChaseData                 data;
      
            // FSM 
            private StateMachine                stateMachine;  
            // AI Path solution
            private NavMeshAgent                agent;
            // AI Warning other agents system
            private WarningSystemAI             _warn;
            // AI controller for performance AI actions
            private AIController                _controller;
            private AIHandler                   _hanlderAI;
            // AI Mesh
        [SerializeField]
            private SkinnedMeshRenderer         enemyMesh;
            // color
            private Color                       originalColor;

    // References to player //
            private GameObject                  playerObject;
            private PlayerHealth                _Player;
    
            private Transform                   playerTarget;
            public Transform                    PlayerTarget => playerTarget;
            private Shooter                     shooterScript;

    // Handling --------------------------------------------------------------------------------->

            // defines when agent is active
            private bool                        _deactivateAI;

    // AI ACTIONS -------------------------------------AI ACTIONS------------------------------>

    // Patrol //

    [Header("Patrol")]

            private int                         destPoint = 0;
        [SerializeField] 
            private Transform[]                 _patrolPoints;

    // Cover // --------------------------------------------------------------------------------->
            private Collider[]                  Colliders = new Collider[10];

            //Lower is a better hiding spot
            private float                       HideSensitivity = 0;
            private float                       UpdateFrequency =  0.25f;
            private float                       minDistInCover;
            private LayerMask                   HidableLayers;
            private SceneChecker                LineOfSightChecker;
            private Coroutine                   MovementCoroutine;

    // Combat // --------------------------------------------------------------------------------->

    [Header("Attack")]
          
        [SerializeField]
            // attack position
            private Transform                   _attackPoint;
            // attack range
            private float                       _attackRange;
            // attack rate of AI 
            private float                       attackRate;
            // time out between attacks
            private float                       _attackTimer;
            // attack special effects
            private GameObject                  _attackEffect, _s_attackEffect, _specialAttackEffect;
           
            // stoping distance from target
            private float                       stopDistance;
                
            private float                       nextAttack;
            private float                       _attackSpeed;

            private bool                        _canAttack;
            private bool                        _canPerformAttack = true; 
            private bool                        _isAttacking;

            private float                       minDist;

            private Vector3                     targetPosition;

            // random attack
            private float                       percentage;
            private int                         b_damage; 

            // special attack
            private const float                 ABILITY_MAX_VALUE = 100F;
            private float                       currentAbilityValue;
            private float                       abilityIncreasePerFrame;
            private int                         specialDamage;
            private bool                        _canSpecialAttack;

    // FOV --------------------------------------------------------------------------------->

        [SerializeField]
            private Transform                   _fov;
            public Transform                    EEFOV => _fov; // Enemy Editor FOV

            private bool                        _useFOV;
            private float                       radius;
            public float                        Radius => radius;
            private float                       angle;
            public float                        Angle => angle;
            private LayerMask                   targetMask;
            private LayerMask                   obstructionMask;
    
            private bool                        canSeePlayer;
            public bool                         canSee => canSeePlayer;


    // Combat Events --------------------------------------------------------------------->

            // Health //
            private float                       _health;
            private int                         MAXHEALTH = 100;
            private float                       healthInCreasePerFrame;

            // Death //
            private GameObject                  _death;

            // Drops & Loot //
            private bool                        _spawnHealth;
            private int                         _healthDrop;
            private GameObject                  _manaObject;
            private int                         _manaDrop;

        
            private GameObject                  _demns;
            private float                       dropRadius = 2f;

            private bool                        gemSpawnOnDeath;
            private GameObject                  gemPrefab;

            // damage //
            private float                       damageOverTime = 2f;
            private float                       durationOfDOT = 10f;

            private int                         damage;
            internal int                        damageBoost;
            private float                       damageEffectTime = 0.5f;

            // Stunned             
            private float                       stunnedTime;
            //private float                     stunnedFrequency;
            private float                       stunedChance;

    // Sound FMOD --------------------------------------------------------------------------------->
            
            // grunt sounds
            private EventReference              _gruntSound_a;
            private EventReference              _soundGrunt_b;

            // special attack sound
            private EventReference              _screamSound;

            // attack sounds
            private EventReference              soundAttack_a;
            private EventReference              soundAttack_b;


    // UI --------------------------------------------------------------------------------->

    [Header("UI Sliders")]

        [SerializeField] 
            private Slider                      _healthSlider;

        [SerializeField] 
            private Slider                      _AbilitySlider; 

            private TextMeshProUGUI             damageText;

            private ValuesTextsScript           valuesTexts;


    // Animator --------------------------------------------------------------------------------->
            private Animator                    _animator;


    // DEBUG --------------------------------------------------------------------------------->
    [Header("Debug")]

        [SerializeField]
            private bool                        _showExtraGizmos, _showLabelGizmos;


            private Vector3 previousPos;
            private float curSpeed;

    // weakness
    //private bool                      _iceWeak, _fireWeak, _thunderWeak;
    #endregion

    #region Awake 
    // Get references to enemies
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

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
        AbilityCheck();

        _currentState = HandleState._NONE; 
        _canAttack = true;
        //_useFOV = true;
    }

    #region Components Sync
    private void GetComponents()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        //path = new NavMeshPath();

        _warn = GetComponent<WarningSystemAI>();

        _hanlderAI = GetComponent<AIHandler>();

        LineOfSightChecker = GetComponentInChildren<SceneChecker>();

        _animator = GetComponentInChildren<Animator>();
        //_healthSlider = GetComponentInChildren<Slider>();

        damageText = GetComponentInChildren<TextMeshProUGUI>();

        _controller = GetComponentInParent<AIController>();


        playerObject = GameObject.Find("Player");
        playerTarget = playerObject.transform;

        targetPosition = playerTarget.position;

        shooterScript = playerObject.GetComponent<Shooter>();

        //playerTarget = GetComponent<Transform>();

        _Player = FindObjectOfType<PlayerHealth>();

        valuesTexts = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {

        // COMBAT ------------------------------------------------------->

        stopDistance                = data.StopDistance;

        _attackEffect               = data.AttackEffect;    
         _attackRange               = data.AttackDist;
        _attackTimer                = data.AttackTimer;
        _attackSpeed                = data.AttackSpeed;
        damage                      = data.Damage;
        attackRate                  = data.AttackRate;
        
        minDist                     = data.MinDist;

        // ** ATTACK 2 (CHANCE) **
    
        percentage                  = data.Percentage;   
        b_damage                    = data.B_damage;   
        _s_attackEffect             = data.S_attackEffect;

        // ** SPECIAL ATTACK **
        abilityIncreasePerFrame     = data.AbilityIncreasePerFrame;
        currentAbilityValue         = data.CurrentAbilityValue;
        _specialAttackEffect        = data.SpecialAttackEffect;    
        specialDamage               = data.S_damage;

        // FOV ------------------------------------------------------->
        radius                      = data.Radius;
        angle                       = data.Angle;
        targetMask                  = data.TargetMask;
        obstructionMask             = data.ObstructionMask;

        // Cover ------------------------------------------------------->
        HidableLayers               = data.HidableLayers;
        //minDistInCover            = data.MindistIncover;

        // Health ------------------------------------------------------->

        _health                     = data.Health;
        _healthSlider.value         = _health;
        healthInCreasePerFrame      = data.HealthRegen;
        _AbilitySlider.value        = currentAbilityValue;

        // Death & Damage ------------------------------------------------------->

        // death
        _death                      = data.DeathEffect;
        damageEffectTime            = data.DamageTime;

        // stunned
        stunnedTime                 = data.StunnedTime;
        stunedChance                = data.SunnedChance;

        // Loot ------------------------------------------------------->
        gemSpawnOnDeath             = data.GemSpawnOnDeath;
        gemPrefab                   = data.Gem;



        _spawnHealth                = data.SpawnHealth;
        _healthDrop                 = data.HealthUnits;
        _demns                      = data.HealthDrop;

        _manaObject                 = data.ManaDrop; 


        // Sound

        _screamSound                 = data.Scream; 

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
          

        State PatrolState = new State("Patrol",
            Patrol);//,
    

        // Combat states

        State ChaseState = new State("Chase",
            ChasePlayer);//,
     
        State FindCover = new State("Cover",
            Cover);

        State GloryKillState = new State("Glory Kill",
            null);

        #endregion

        // Trasitions --------------------------------------------------->

        #region Trasition of states
        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                () => _stateAI == AI._ATTACK,
                ChaseState));
     
        // PATROL -> CHASE
        PatrolState.AddTransition( 
            new Transition(
               () => _stateAI == AI._ATTACK,
               ChaseState));

        // CHASE --------------------------------------------->

        // CHASE -> PATROL
        ChaseState.AddTransition(
           new Transition(
               () => _stateAI == AI._PATROL, // SEEK SOLUTION
               //() => Debug.Log("CHASE -> PATROL"),
               PatrolState));

        //CHASE -> Glory Kill 
        ChaseState.AddTransition
            (new Transition(
                () => _stateAI == AI._GLORYKILL,
                GloryKillState));

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

        stateMachine = new StateMachine(PatrolState);
    }
    #endregion


    #endregion

    #region Ability state starting check
    private void AbilityCheck()
    {
        if (currentAbilityValue >= ABILITY_MAX_VALUE) { 
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
        switch(_hanlderAI.AgentOperate)
        {
            case true:
                {
                    _deactivateAI = false;
                    break; 
                }
            case false: 
                {
                    _deactivateAI = true;
                    break; 
                }
        }


        if (_gameState == GameState.Gameplay && !_deactivateAI) //|| !_deactivateAI)
        {
            MinimalCheck();

            HealthCheck();

            AISpeed();

            Action actions = stateMachine.Update();
            actions?.Invoke();

            if (_useFOV)
            {
                StartFOV();
            }
        }
        else if (_deactivateAI)
        {
            //StopAgent();
            return;
        }
    }
    #endregion

    #region Agent State
    private void ResumeAgent()
    {
        if (_currentState == HandleState._NONE)
        {
            if (agent.enabled)
            {
                // resume agent
                HandleStateAI(false);

            }
        }        
    }

    private void PauseAgent()
    {
        if (_currentState == HandleState._NONE)
        {
            if (agent.enabled)
            {
                //stop agent
                HandleStateAI(true);
            }
        }
       
            
    }

    private void StopAgent()
    {
        StopAllCoroutines();
        _useFOV = false;    

        if (agent.enabled)
        {
            // fully stop agent
            HandleStateAI(true);
            return;
        }
    }
    #endregion

    #region FOV
    private void StartFOV()
    {
        StartCoroutine(FOVRoutine());
    }

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
        Collider[] rangeChecks = Physics.OverlapSphere(_fov.position, radius, targetMask);


        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - _fov.position).normalized;

            if (Vector3.Angle(_fov.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(_fov.position, target.position);

                if (!Physics.Raycast(_fov.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if(_canAttack)
                    {
                        SetAttack(); 
                        canSeePlayer = true;
                        return; 
                    }
                    
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

    #region Distance Check
    private void MinimalCheck()
    {
        
        if ((playerTarget.transform.position - transform.position).magnitude < minDist && _canAttack)
        {
            SetAttack();
            
        }

    }

    #endregion

    #region Health Check 
    private void HealthCheck()
    {

        if (_health <= 10)
        {
            SetGloryKill();
            return;
        }

        /*
        else if (_health <= 50)//&& _Health > 10) 
        {
            SetCover();
            return;
        }
        */
    }
    #endregion

    #region Speed
    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;

        if(curSpeed >= 0.8f)
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
       if(agent.enabled) 
       {
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !canSeePlayer)
            {
                SetPatrol();
                GotoNetPoint();
            }
       }
    }

    private void GotoNetPoint()
    {

        // Returns if no points have been set up
        if (_patrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = _patrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _patrolPoints.Length;
    }
    #endregion

    #region Chase & Attack

    // Chase enemy
    private void ChasePlayer()
    {
        if(_canAttack) 
        {
            // get player direction
            Vector3 DIRECTION = playerTarget.position - transform.position;
            //rotation 
            Quaternion DISAREDROT = Quaternion.LookRotation(new Vector3(DIRECTION.x, 0f, DIRECTION.z));

            // apply rotation to AI 
            transform.rotation = DISAREDROT;

            // set destination
            agent.SetDestination(playerTarget.position);


            switch (_canSpecialAttack)
            {
                case false:
                    {
                        Attack(); // call attack and random attack
                        
                        if (currentAbilityValue <= ABILITY_MAX_VALUE)
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
    }

    // * Attack & Chance Attack -------------------------------->
    private void Attack()
    {
        switch (_canPerformAttack)
        {
            case true:
                {
                    //if ((playerTarget.transform.position - transform.position).magnitude <= 6F)
                    if (agent.remainingDistance <= stopDistance)
                    {
                        PauseAgent();
                        


                        if (Time.time > nextAttack)
                        {

                            //print("2nd step");
                            float randomFloat = UnityEngine.Random.value;


                            if (UnityEngine.Random.value < percentage && _canPerformAttack)
                            {
                                //print("chose random");

                                Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

                                foreach (Collider collider in hitEnemies)
                                {
                                    string DebugColor = "<size=12><color=yellow>";
                                    string closeColor = "</color></size>";

                                    Debug.Log(DebugColor + "Attack 2" + closeColor);

                                    _Player.TakeDamage(b_damage);
                                    Instantiate(_s_attackEffect, _attackPoint.transform.position, Quaternion.identity);
                                }


                                //StartCoroutine(AttackTimer());
                            }

                            else
                            {

                                Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

                                foreach (Collider collider in hitEnemies)
                                {
                                    string DebugColor = "<size=12><color=green>";
                                    string closeColor = "</color></size>";
                                    Debug.Log(DebugColor + "Attack" + closeColor);
                                    _Player.TakeDamage(damage);

                                    Instantiate(_attackEffect, _attackPoint.transform.position, Quaternion.identity);
                                }

                                //StartCoroutine(AttackTimer());

                            }

                            _canPerformAttack = false;
                            nextAttack = Time.time + attackRate;
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

    // * Special Attack -------------------------------->
    private void HabilityAttack()
    {
        
        agent.speed = 25f;
        agent.acceleration = 14f;


        //if ((playerTarget.transform.position - transform.position).magnitude <= 6F) // define min distance

        if (agent.remainingDistance <= stopDistance)
        {
            PauseAgent();
            
            Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

            foreach (Collider collider in hitEnemies)
            {
                RuntimeManager.PlayOneShot(_screamSound);

                string DebugColor = "<size=14><color=orange>";
                string closeColor = "</color></size>";
                Debug.Log(DebugColor + "Special attack" + closeColor);


                currentAbilityValue = 0;
                //_AbilitySlider.value = currentAbilityValue;

                _Player.TakeDamage(specialDamage);

                
                Instantiate(_specialAttackEffect, _attackPoint.transform.position, Quaternion.identity);

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

    private void Cooldown()
    {
        currentAbilityValue = Mathf.Clamp(currentAbilityValue + (abilityIncreasePerFrame * Time.deltaTime), 0.0f, ABILITY_MAX_VALUE);
        _AbilitySlider.value = currentAbilityValue;


        if (currentAbilityValue >= ABILITY_MAX_VALUE) { _canSpecialAttack = true; return; }

        //else { _canSpecialAttack = false; return; }
    }

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


    private IEnumerator timeBetweenAttack()
    {

        yield return new WaitForSeconds(0.4f);

        _animator.SetBool("specialAttack", false);
        _animator.SetBool("walk", true);

    }

    private void HandleStateAI(bool stop)
    {
        switch (stop)
        {
            case true:
                {
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true; 
                    break;
                }
            case false:
                {
                    agent.isStopped = false;
                    break;
                }
        }
    }

    #endregion

    #region Cover
    private void Cover()
    {
        HandleGainSight(PlayerTarget);

        //print(curSpeed); 
        if (curSpeed <= 0.5 && _health >= 16)
        {
            _health = Mathf.Clamp(_health + (healthInCreasePerFrame * Time.deltaTime), 0.0f, MAXHEALTH);
            //Debug.Log("Chase health: " + _health);

            _healthSlider.value = _health;


            if ( _health >= 50 && currentAbilityValue >= 65) { _canAttack = true; return;}

            else {_canAttack = false; return;}

        }
        else if(_health < 15) { SetGloryKill();}

        //else if(_canAttack){ SetAttack();}
    }

    private void HandleGainSight(Transform Target)
    {
        //agent.radius = 1f;
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
            return;
        }
        //playerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }

    #region COVER Coroutine

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

    #endregion

    #endregion

    #endregion

    #region AI States Set

    private void SetPatrol()
    {
        //print("patrol FIRED");
        _stateAI = AI._PATROL;

        agent.autoBraking = false;
        agent.updateRotation = true;

        agent.stoppingDistance = 0.1f;
        agent.speed = 1f;

        if(_hanlderAI.AgentOperate)
        {
            _useFOV = true;
        }
       
        return;
    }

    private void SetAttack()
    {
        // Agent configuration
      
        if(agent.enabled) 
        {
            //agent.speed = attackSpeed;
            //agent.stoppingDistance = 3.9f;
            agent.speed = _attackSpeed;

            agent.angularSpeed = 0f;
            agent.updateRotation = false;

            //StartAttacking();
            _canAttack = true;
            _stateAI = AI._ATTACK;
            _useFOV = false;
            return;
        }
        
    }

    private void SetStunned()
    {
        // start coroutine 
    }
    private void SetCover()
    {
        _canAttack = false;
        agent.updateRotation = true;
 

        agent.radius = 1f;

        agent.speed = 4f;
        agent.stoppingDistance = 0.3f;

        _stateAI = AI._COVER;

       

        //_useFOV = true;

        return;
    }

    private void SetGloryKill()
    {
        _stateAI = AI._GLORYKILL;
        _useFOV = false;
        return;
    }

    #endregion

    #region AI Health 
    public void TakeDamage(int _damage, WeaponType _Type)
    {
        
        if (_health <= 0)
        {
            //_stateAI = AI._NONE; 
            Die();
        }

        else if (_health > 0)
        {
            switch (_Type)
            {

                case WeaponType.Normal:
                    {
                        _health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());

                        //damageEffectTime = 2f; 
                        break;
                    }

                case WeaponType.Ice:
                    {
                        if (shooterScript.wUpgraded == true)
                        {
                            StartCoroutine(DamageOverTime(damageOverTime, durationOfDOT));
                        } 
                        else
                            StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Fire:
                    {
                        _health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());

                        //damageEffectTime = 2.3f;

                        break;
                    }

                case WeaponType.Thunder:
                    {
                        _health -= _damage + damageBoost;

                        if (shooterScript.rUpgraded == true)
                            StartCoroutine(STFS(stunnedTime));
                        else
                            StartCoroutine(HitFlash());
                        break;
                    }

                case WeaponType.Dash:
                    {
                        _health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());
                        break;
                    }

                 
            }


            //_health -= _damage + damageBoost;

            float randomFloat = UnityEngine.Random.value;

            if(randomFloat <= stunedChance) 
            {
                //print("STARTED CHANCE");
                //print(randomFloat);

                StartCoroutine(STFS(stunnedTime)); 
            }

            damageText.text = _damage.ToString();

            StartCoroutine(DamageTextDisappear());

            // ADD SPECIAL EFFECT WHEN WE SHOOT WE CAUSE THE 

            //StartCoroutine(DamageEffect());

            _healthSlider.value = _health;

            
            //Debug.Log("enemy shot" + _health);

            if (_canAttack)
            {
                _warn.canAlertAI = true;
                //SetAttack();
                return;
            }
        }
    }

    private void Die()
    {
        if(_spawnHealth)
        {
            for (int i = 0; i < _healthDrop; i++)
            {

                Vector3 spawnPosition = transform.position +
                    new Vector3(UnityEngine.Random.Range(-dropRadius, dropRadius), 0f,
                    UnityEngine.Random.Range(-dropRadius, dropRadius));

                Instantiate(_demns, spawnPosition, Quaternion.identity);
            }


            for(int i = 0; i < _manaDrop; i++)
            {

            }
        }
        

        if (gemSpawnOnDeath)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }

        Instantiate(_death, transform.position, Quaternion.identity);

        valuesTexts.GetKill();

        Destroy(this.gameObject);
    }
    #endregion

    #region Receive Warning
    void OnPlayerWarning(Vector3 Target)
    {
        if (_canAttack)
        {
            SetAttack();

            return;
        }

    }
    #endregion

    #region Control & Visual Coroutines
    IEnumerator DamageTextDisappear()
    {
        yield return new WaitForSeconds(2f);
        damageText.text = " ";
    }

    // stop agent for seconds
    private IEnumerator STFS(float value)
    {
        // Start bool at false to make sure coroutine is not being run more than once
        bool STFS_EFFECT = false;

        switch(STFS_EFFECT)
        {
            case false:
                {
                    STFS_EFFECT = true;

                    

                    print("STARTED STFS COROUTINE SUCCESFULLY");

                    _canAttack = false;
                    _currentState = HandleState._STOPED;

                    HandleStateAI(true);

                    //originalColor = GetComponent<Renderer>().material.color;
                    //GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);

                    yield return new WaitForSeconds(value);

                    //GetComponent<Renderer>().material.color = originalColor;

                    _currentState = HandleState._NONE;

                    HandleStateAI(false);
                    _canAttack = true;

                    STFS_EFFECT = false;


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
            yield return new WaitForSeconds(2.5f);
            elapsedTime += 2.5f;

        }

    }

    IEnumerator HitFlash()
    {
        //originalColor = GetComponent<Renderer>().material.color;
        originalColor = enemyMesh.material.color;

        enemyMesh.material.color = Color.red;
        //GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        //GetComponent<Renderer>().material.color = originalColor;
        enemyMesh.material.color = originalColor;
    }

    IEnumerator SpecialAttackCollor()
    {

        originalColor = enemyMesh.material.color;


        enemyMesh.material.color = Color.red;
        yield return new WaitForSeconds(1);
        enemyMesh.material.color = originalColor;

        enemyMesh.material.color = Color.red;
        yield return new WaitForSeconds(1.2f);
        enemyMesh.material.color = originalColor;

        enemyMesh.material.color = Color.red;
        yield return new WaitForSeconds(1.5f);
        enemyMesh.material.color = originalColor;

        enemyMesh.material.color = Color.red;
        yield return new WaitForSeconds(2f);
        enemyMesh.material.color = originalColor;

        enemyMesh.material.color = Color.red;
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

        if (_showExtraGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDist);

        }

        #region AI State Label 
        if (_showLabelGizmos)
        {
            switch (_stateAI)
            {
                case AI._GUARD:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "Guard" + "  Gameplay: " + _gameState, green);
                        break;
                    }
                case AI._PATROL:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "Patrol" + "  Gameplay: " + _gameState, blue);
                        break;
                    }
                case AI._ATTACK:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "Attack" + "  Gameplay: " + _gameState + _currentState, red);
                        break;
                    }

                case AI._COVER:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "Cover" + "  Gameplay: " + _gameState, cyan);
                        break;
                    }
                case AI._GLORYKILL:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "Glory Kill" + "  Gameplay: " + _gameState);
                        break;
                    }
                case AI._NONE:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "NONE" + "  Gameplay: " + _gameState);
                        break;
                    }
                default:
                    {
                        Handles.Label(_fov.transform.position + Vector3.up * 2f, "NO STATE FOUND" + "  Gameplay: " + _gameState);
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