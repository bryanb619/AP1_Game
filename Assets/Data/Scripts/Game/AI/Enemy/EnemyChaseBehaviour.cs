#region Library
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;

using UnityEditor; // comment this before build
using NUnit.Framework;
using System.Collections.Generic;
#endregion

public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables

    private enum AI                             { _GUARD, _PATROL, _ATTACK, _COVER, _GLORYKILL, _NONE }

    private AI                                  _stateAI;

    private GameState                           _gameState;

    private StateMachine                        stateMachine;



    [Header("AI Profile")]
    [SerializeField] private AIChaseData        data;

    private bool                                gemSpawnOnDeath = true;
    private GameObject                          gemPrefab;

    


    // Reference to the state machine
    

    // Reference to the NAV MESH AGENT
    private NavMeshAgent                        agent;


    private AIController                        _controller;


    // References to player
    private GameObject                          playerObject;
    private PlayerMovement                      _Player;

    private Transform                           playerTarget;
    public Transform                            PlayerTarget => playerTarget;

    private WarningSystemAI                     _warn;

    // Patrol Points

    [Header("Patrol")]
    private int destPoint = 0;
    [SerializeField] private Transform[]        _patrolPoints;

    // FOV
    private bool                                _useFOV; 

    private float                               radius;
    public float                                Radius => radius;

    private float                               angle;
    public float                                Angle => angle;

    private LayerMask                           targetMask;
    private LayerMask                           obstructionMask;

    [SerializeField] private Transform          FOV;
    public Transform                            EEFOV => FOV; // Enemy Editor FOV

    private bool                                canSeePlayer;
    public bool                                 canSee => canSeePlayer;
        
    // Cover
    private Collider[]                          Colliders = new Collider[10];

    //Lower is a better hiding spot
    private float                               HideSensitivity = 0;

    private float                               UpdateFrequency =  0.25f;

    private float                               minDistInCover;

    private LayerMask                           HidableLayers;

    private SceneChecker                        LineOfSightChecker;

    private Coroutine                           MovementCoroutine;

    // Health
    private float                               _health;
    private int                                 MAXHEALTH = 100;


    // Death // 

    bool                                        canSpawnHealth = true;

    int                                         demns = 4;

    private List<GameObject>                     _instances = new List<GameObject>();

    [SerializeField] private GameObject         _demns; 


    private float                               healthInCreasePerFrame;
    internal int                                damageBoost;

    // Get collor
    private Color                               originalColor;

    // damage
    //private float                             damagePerSecondFire = 2f;
    private float                               durationOfFireDamage = 10f;

    private int                                 damage; 



    // GET AI Speed
    private Vector3                             previousPos;
    private float                               curSpeed;


    // weakness
    //private bool                                _iceWeak, _fireWeak, _thunderWeak;


    // Combat //

    [Header("Attack")]
    [SerializeField] private Transform          _attackPoint;

    private GameObject                          _death;

    private GameObject                          _attackEffect, _s_attackEffect, _specialAttackEffect;

    private float                               _attackRange; 

    private float                               stopDistance; 

    private float                               attackRate;

    private float                               nextAttack;

    private float                               _attackSpeed;

    private bool                                _canAttack;

    private bool                                _canPerformAttack = true; 

    private bool                                _isAttacking;

    private float                               minDist;

    private float                               damageEffectTime; 


    private float                               _attackTimer; 

    // Attack 2 //

    private float                               percentage;

    private int                                 b_damage; 

    // special ability 

    private const float                         ABILITY_MAX_VALUE = 100F;

    private float                               currentAbilityValue;

    private float                               abilityIncreasePerFrame;

    private int                                 specialDamage;

    private bool                                _canSpecialAttack; 


    // UI 
    [Header("UI Sliders")]
    [SerializeField] private Slider            _healthSlider;

    [SerializeField] private Slider            _AbilitySlider; 

    private TextMeshProUGUI                     damageText;

    
    // animation

    private Animator                            _animator;


    // Performance 


    private AIHandler                           _hanlderAI;

    private bool                                _deactivateAI;

    [Header("Debug")]
    [SerializeField] private bool               showExtraGizmos; 


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
        //_stateAI = AI._PATROL; 

        _canAttack = true;
        _useFOV = true;


        GetComponents();
        GetProfile();
        GetStates();

       
        if(currentAbilityValue >= ABILITY_MAX_VALUE) { _canSpecialAttack = true;}
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

        //playerTarget = GetComponent<Transform>();


        _Player = FindObjectOfType<PlayerMovement>();



    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {


        // COMBAT //

        _death = data.DeathEffect;

        _attackEffect = data.AttackEffect;    

         _attackRange = data.AttackDist;

        _attackTimer = data.AttackTimer;

        _attackSpeed = data.AttackSpeed;

        damageEffectTime = data.DamageTime; 

         // attack
        minDist = data.MinDist;
        attackRate = data.AttackRate;

        //attackDistanceOfsset = data.AttackDistOffset;
        stopDistance = data.StopDistance;

  
        damage = data.Damage;
        //cooldownSpeed = data.CooldownSpeed;


            // atack N2
        percentage = data.Percentage;   

        b_damage = data.B_damage;   

        _s_attackEffect = data.S_attackEffect;  
        
        // special attack
        abilityIncreasePerFrame = data.AbilityIncreasePerFrame;
        currentAbilityValue = data.CurrentAbilityValue;
        
        _specialAttackEffect = data.SpecialAttackEffect;    

        specialDamage = data.S_damage; 

        // FOV
        radius = data.Radius;
        angle = data.Angle;
        targetMask = data.TargetMask;
        obstructionMask = data.ObstructionMask;

        // Cover
        HidableLayers = data.HidableLayers;
        minDistInCover = data.MindistIncover;

        // Health
        _health = data.Health;

        _healthSlider.value = _health;

        healthInCreasePerFrame = data.HealthRegen;

        _AbilitySlider.value = currentAbilityValue;

        // Weakness
        //_iceWeak = data.Ice;
        //_fireWeak = data.Fire;
        //_thunderWeak = data.Thunder;

        // Gem
        gemSpawnOnDeath = data.GemSpawnOnDeath;
        gemPrefab = data.Gem;

        //
        damage = data.Damage;

        radius = data.Radius;
        angle = data.Angle;
        targetMask = data.TargetMask;
        obstructionMask = data.ObstructionMask;

    }

    #region  States Sync 
    private void GetStates()
    {

        #region States
        // Non Combat states
        State onGuardState = new State("GUARD",
            //() => Debug.Log("Enter On Guard state"),
            null);//,
            //() => Debug.Log(""));

        State PatrolState = new State("Patrol",
            //() => Debug.Log(""),
            Patrol);//,
            //() => Debug.Log(""));

        // Combat states

        State ChaseState = new State("",
            //() => Debug.Log("Chase"),
            ChasePlayer);//,
            //() => Debug.Log(""));

        State FindCover = new State("Cover",
            //() => Debug.Log(""),
            Cover);//,
            //() => Debug.Log(""));

        State GloryKillState = new State("Glory Kill State",
            //() => Debug.Log("Entered glory kill state"),
            null);//,
            //() => Debug.Log("Left Glory Kill State"));

        #endregion


        #region Trasintion of states
        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                //canSeePlayer == true
                () => _stateAI == AI._ATTACK,
                //() => Debug.Log("GUARD -> CHASE"),
                ChaseState));

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


        // PATROL -> CHASE
        PatrolState.AddTransition(
           new Transition(
               () => _stateAI == AI._ATTACK, // SEEK SOLUTION
               //() => Debug.Log("PATROL -> CHASE"),
               ChaseState));

        // CHASE -> PATROL
        ChaseState.AddTransition(
           new Transition(
               () => _stateAI == AI._PATROL, // SEEK SOLUTION
               //() => Debug.Log("CHASE -> PATROL"),
               PatrolState));

        #endregion

        stateMachine = new StateMachine(PatrolState);
    }
    #endregion


    #endregion

    #endregion

    #region Update

    #region Update void

    // Update is called once per frame
    void Update()
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


        if (_gameState == GameState.Gameplay)
        {
            ResumeAgent();

            MinimalCheck();

            HealthCheck();

            AISpeed();

            Action actions = stateMachine.Update();
            actions?.Invoke();

            if (_useFOV)
            {
                StartFOV();
            }


            if (canSpawnHealth && _stateAI == AI._NONE)
            {
                // int
                
            }
            /*
            if (_stateAI == AI._ATTACK)
            {
                AddAttacker();
                return;
            }
            else
            {
                RemoveAttacker();
                return;
            }
            */

        }
        else if (_gameState == GameState.Paused || _deactivateAI)
        {
            PauseAgent();
            return;
        }


       
    }
    #endregion

    #region Agent State
    private void ResumeAgent()
    {
        if(agent.enabled)
        {
            agent.updateRotation = true;
            agent.isStopped = false;
            return;
        }
       
    }

    private void PauseAgent()
    {
        
        if (agent.enabled)
        {
            agent.updateRotation = false;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            return;
        }
            //Agent.speed = 0f; 
            //Agent.Stop(); 
            
    }

    private void StopAgent()
    {
        agent.isStopped = true;

        StopAllCoroutines();
        return; 
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

        if (_health <= 15)
        {
            SetGloryKill();
            return;
        }


        else if (_health <= 50)//&& _Health > 10) 
        {
            SetCover();
            return;
        }
    }
    #endregion

    #region Speed
    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;

        if(curSpeed >= 0.01f)
        {
            _animator.SetBool("walk", true);
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
    // Chase the small enemy
    private void ChasePlayer()
    {

        if(_canAttack) 
        {
            transform.LookAt(new Vector3(playerTarget.position.x, 0, playerTarget.position.z));

            switch (_canSpecialAttack)
            {
                case false:
                    {
                        agent.SetDestination(playerTarget.position);

                        Attack();

                        
                        if (agent.remainingDistance <= stopDistance)
                        {
                            agent.speed = 0f; 
                            return;
                        }

        
                        if (currentAbilityValue <= ABILITY_MAX_VALUE)
                        {
                            Cooldown();
                            return;
                        }

                        break;

                    }
                case true:
                    {
                        agent.speed = 4.0f;
                        agent.SetDestination(playerTarget.position);
                        transform.LookAt(new Vector3(playerTarget.position.x, 0, playerTarget.position.z));

                       
                        HabilityAttack();
                       
                        break;
                    }
            }
        }
    }
    private void Attack()
    {
        switch (_canPerformAttack)
        {
            case true:
                {
                    //agent.speed = 2.5f;
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

                            //print("chose normal");
                        


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

    private void HabilityAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

        foreach (Collider collider in hitEnemies)
        {
            string DebugColor = "<size=14><color=orange>";
            string closeColor = "</color></size>";
            Debug.Log(DebugColor + "Special attack" + closeColor);


            currentAbilityValue = 0;
            _AbilitySlider.value = currentAbilityValue;

            _Player.TakeDamage(specialDamage);

            Instantiate(_specialAttackEffect, _attackPoint.transform.position, Quaternion.identity);
            _canSpecialAttack = false;


        }
    }
    

    private void Cooldown()
    {
        currentAbilityValue = Mathf.Clamp(currentAbilityValue + (abilityIncreasePerFrame * Time.deltaTime), 0.0f, ABILITY_MAX_VALUE);
        _AbilitySlider.value = currentAbilityValue;

        if(currentAbilityValue >= ABILITY_MAX_VALUE) { _canSpecialAttack = true; return;}
        else {_canSpecialAttack = false;  return;}
    }


    private IEnumerator AttackTimer()
    {

        _canPerformAttack = false;
        yield return new WaitForSeconds(_attackTimer);
 
        _canPerformAttack = true;
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

        agent.stoppingDistance = 0.1f;
        agent.speed = 1f;

        _useFOV = true;
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

            //StartAttacking();
            _canAttack = true;
            _stateAI = AI._ATTACK;
            _useFOV = false;
            return;
        }
        
    }
    private void SetCover()
    {
        _canAttack = false;

        _stateAI = AI._COVER;

        agent.radius = 1f;
        agent.speed = 5f;
        agent.stoppingDistance = 1.3f;

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
            _stateAI = AI._NONE; 
            Die();
        }

        else if (_health > 0)
        {
            
            switch (_Type)
            {

                case WeaponType.Normal:
                    {
                        _health -= _damage + damageBoost;

                        //print("normal damage");
                        StartCoroutine(HitFlash());

                        //damageEffectTime = 2f; 
                        break;
                    }
                case WeaponType.Ice:
                    {
                        StartCoroutine(STFS(5F));

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

            damageText.text = _damage.ToString();

            StartCoroutine(DamageTextDisappear());

            // ADD SPECIAL EFFECT WHEN WE SHOOT WE CAUSE THE 
       
            StartCoroutine(DamageEffect());

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

    
    private IEnumerator DamageEffect() 
    {
        bool damageEffect = false;
        

        if(!damageEffect) 
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

        for (int i = 0; i < demns; i++)
        {

            float RANDOMNUMBER = UnityEngine.Random.Range(0,360);

            Vector3 temp = transform.rotation.eulerAngles;

            temp.y = RANDOMNUMBER; 

            GameObject instance = Instantiate(_demns, transform.position,Quaternion.Euler(temp));

            _instances.Add(instance);
        }

        if (gemSpawnOnDeath)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }

        Instantiate(_death, transform.position, Quaternion.identity);

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

    #region Visual Coroutines
    IEnumerator DamageTextDisappear()
    {
        yield return new WaitForSeconds(2f);
        damageText.text = " ";
    }

    // stop time for seconds
    private IEnumerator STFS(float value)
    {

        PauseAgent();
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);
        yield return new WaitForSeconds(value);

        GetComponent<Renderer>().material.color = originalColor;
        ResumeAgent();
    }

    // Damage over time
    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
        float elapsedTime = 0f;
        while (elapsedTime < durationOfFireDamage)
        {
            _health -= damagePerSecond;
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

    #region Game state reception

    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
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

        #region Gizmos code
        if (showExtraGizmos)
        {
           
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


            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDist);


            #region AI State Label 
            switch (_stateAI)
            {
                case AI._GUARD:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Guard" + "  Gameplay: " + _gameState, green);
                        break;
                    }
                case AI._PATROL:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Patrol" + "  Gameplay: " + _gameState, blue);
                        break;
                    }
                case AI._ATTACK:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Attack" + "  Gameplay: " + _gameState, red);
                        break;
                    }

                case AI._COVER:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Cover" + "  Gameplay: " + _gameState, cyan);
                        break;
                    }
                case AI._GLORYKILL:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Glory Kill" + "  Gameplay: " + _gameState);
                        break;
                    }
                case AI._NONE:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "NONE" + "  Gameplay: " + _gameState);
                        break;
                    }
                default:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "NO STATE FOUND" + "  Gameplay: " + _gameState);
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