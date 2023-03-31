#region Library
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;

using UnityEditor; // comment this before build
using System.Runtime.CompilerServices;
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

    [Header("AI List")]
    [SerializeField] private Agents             _agentAI; 


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

    private float                               healthInCreasePerFrame;
    internal int                                damageBoost;

    // Get collor
    private Color                               originalColor;


    // damage
    //private float                               damagePerSecondFire = 2f;
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

    private GameObject                          _attackEffect;

    private float                               _attackRange; 

    private float                               attackDistanceOfsset;
    private float                               stopDistance; 

    private float                               attackRate;
    private float                               nextAttack;

    private bool                                _canAttack;

    private bool                                _canPerformAttack = true; 

    private bool                                _isAttacking;

    private float                               minDist;


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

        GetComponents();
        GetProfile();
        GetStates();

        _canAttack = true;
        _useFOV = true; 

        if(currentAbilityValue > ABILITY_MAX_VALUE) { _canSpecialAttack = true;}

        
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

            // attack
        minDist = data.MinDist;
        attackRate = data.AttackRate;

        attackDistanceOfsset = data.AttackDistOffset;
        stopDistance = data.StopDistance;

  
        damage = data.Damage;
        //cooldownSpeed = data.CooldownSpeed;


            // atack N2
        percentage = data.Percentage;   

        b_damage = data.B_damage;   
        
        // special attack
        abilityIncreasePerFrame = data.AbilityIncreasePerFrame;
        currentAbilityValue = data.CurrentAbilityValue; 

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
               //() => Debug.Log("COVER -> CHASE"),
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

        switch (_deactivateAI)
        {
            case false:
                {
                    if (_gameState == GameState.Gameplay)
                    {
                        ResumeAgent();

                        if(_useFOV) { CanFOV(); }
                        
                        MinimalCheck();

                        HealthCheck();

                        AISpeed();

                        Action actions = stateMachine.Update();
                        actions?.Invoke();
                    }
                    else if (_gameState == GameState.Paused)
                    {
                        PauseAgent();
                        return; 
                    }
                    break;
                }
            case true:
                {
                    StopAgent();
                    break; 
                }
        }




        
    }
    #endregion

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
        agent.isStopped = true;
        return; 
    }

    private void StopAgent()
    {
        agent.isStopped = true;

        StopAllCoroutines();
        return; 
    }

    #endregion

    #region FOV
    private void CanFOV()
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
        print("FOV IN USE");
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

    #region Distance Check
    private void MinimalCheck()
    {
        
        if ((playerTarget.transform.position - transform.position).magnitude < minDist)
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
       
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !canSeePlayer)
        {
            SetPatrol(); 
            GotoNetPoint();
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
        switch(_canSpecialAttack)
        {
            case false:
                {
                    
                    if ((playerTarget.transform.position - transform.position).magnitude < attackDistanceOfsset)
                    {
                        //print("1s step");
                        Attack();
                    }

                    else
                    {
                        agent.speed = 3.0f;
                        agent.SetDestination(playerTarget.position);
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

                    if ((playerTarget.transform.position - transform.position).magnitude < attackDistanceOfsset)
                    {
                        HabilityAttack();
                    }
                    break;
                }
        }
    }
    private void Attack()
    {

        //_canPerformAttack = true;

        transform.LookAt(new Vector3(playerTarget.position.x, 0, playerTarget.position.z));

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

                            string DebugColor = "<size=12><color=yellow>";
                            string closeColor = "</color></size>";

                            Debug.Log(DebugColor + "Attack 2" + closeColor);

                            
                            Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

                            foreach (Collider collider in hitEnemies)
                            {
                                _Player.TakeDamage(b_damage);
                            }

                            Instantiate(_attackEffect, _attackPoint.transform.position, Quaternion.identity);
                            //StartCoroutine(AttackTimer());
                        }

                        else
                        {

                            //print("chose normal");
                            string DebugColor = "<size=12><color=green>";
                            string closeColor = "</color></size>";
                            Debug.Log(DebugColor + "Attack" + closeColor);


                            Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, targetMask);

                            foreach (Collider collider in hitEnemies)
                            {
                                _Player.TakeDamage(damage);
                            }

                            Instantiate(_attackEffect, _attackPoint.transform.position, Quaternion.identity);

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
    }

    private void HabilityAttack()
    {    
        string DebugColor = "<size=14><color=orange>";
        string closeColor = "</color></size>";
        Debug.Log(DebugColor + "Special attack" + closeColor);

       
        currentAbilityValue = 0;
        _AbilitySlider.value = currentAbilityValue;

        _Player.TakeDamage(specialDamage);
        _canSpecialAttack = false;
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
      
        //agent.speed = attackSpeed;
        agent.stoppingDistance = 3.9f;

        
        //StartAttacking();
        _canAttack = true;
        _stateAI = AI._ATTACK;
        _useFOV = false;
        return;
    }
    private void SetCover()
    {
        _canAttack = false;

        _stateAI = AI._COVER;

        agent.radius = 1f;
        agent.speed = 5f;
        agent.stoppingDistance = 1.3f;

        _useFOV = true;

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

            _healthSlider.value = _health;

            Debug.Log("enemy shot" + _health);

            if (_canAttack)
            {
                _warn.canAlertAI = true;
                SetAttack();
                return;
            }
        }
    }

    private void Die()
    {
        Instantiate(_death, transform.position, Quaternion.identity);

        if (gemSpawnOnDeath)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }
            
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

    private IEnumerator STFS(float value)
    {

        PauseAgent();
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);
        yield return new WaitForSeconds(value);

        GetComponent<Renderer>().material.color = originalColor;
        ResumeAgent();
    }

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
                    _gameState= GameState.Paused;   
                    break;
                }
        }

        //throw new NotImplementedException();
    }
    #endregion

    #region Agents Info Exchange

    public void StartAttacking()
    {
        if (!_isAttacking)
        {
            _agentAI.StartAttacking();
            return;
        }
    }

    public void StopAttacking()
    {
        if (_isAttacking)
        {
            _agentAI.StopAttacking();
            return;
            // _canAttack = false;
        }

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

        if(showExtraGizmos)
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
            Gizmos.DrawSphere(_attackPoint.position, 1.3f);


            #region AI State Label 
            switch (_stateAI)
            {
                case AI._GUARD:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Guard" + "  Gameplay: ", green);
                        break;
                    }
                case AI._PATROL:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Patrol" + "  Gameplay: ", blue);
                        break;
                    }
                case AI._ATTACK:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Attack" + "  Gameplay: ", red);
                        break;
                    }

                case AI._COVER:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Cover" + "  Gameplay: ", cyan);
                        break;
                    }
                case AI._GLORYKILL:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "Glory Kill" + "  Gameplay: ");
                        break;
                    }
                case AI._NONE:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "NONE" + "  Gameplay: ");
                        break;
                    }
                default:
                    {
                        Handles.Label(FOV.transform.position + Vector3.up, "NO STATE FOUND" + "  Gameplay: ");
                        break;
                    }
            }
        }

        #endregion
#endif
    }
    #endregion
}