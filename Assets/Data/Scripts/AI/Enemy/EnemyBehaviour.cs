#region Libs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;

using UnityEditor; // comment this on build

#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    #region  Variables

    // Reference to AI data
    [SerializeField] private AIRangedData data;

    // Reference to the state machine
    private StateMachine stateMachine;

    // Reference to the NavMeshAgent
    internal NavMeshAgent agent;

    // Reference to the Outline component
    [SerializeField] private Outline outlineDeactivation;

    [SerializeField] private GameObject _death;

    // AI Set states
    private enum AI {_GUARD, _PATROL, _ATTACK, _COVER, _SEARCH, _GLORYKILL, _NONE}

    private AI _stateAI;

    private WarningSystemAI _warn; 

    [SerializeField] private Agents _agentAI;

    private Color originalColor;
    public int damageBoost = 0;

    GemManager gemManager;

    private bool gemSpawnOnDeath;

    private float health;

    // References to enemies
    public GameObject PlayerObject;

    private Transform _playerTarget;
    public Transform PlayerTarget => _playerTarget;

    private PlayerMovement _player; 

    private float minDist = 5f;

    // AI SPEED
    private float curSpeed;
    private Vector3 previousPos;

    private float AttackRequiredDistance = 8f;

    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;


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


    // Attack 

    [SerializeField]
    private Transform _shootPos;

    private GameObject gemPrefab;

    private GameObject bullet, specialBullet; 

    private float fireRate = 2f;
    private float nextFire = 0f;

    private float percentage; 


    // special ability 

    private const float ABILITY_MAX_VALUE = 100F;

    private float currentAbilityValue;

    private float abilityIncreasePerFrame;

    private int specialDamage;


    private GameObject s_damageEffect; 


    // hide code
    [Header("Hide config")]
    private Collider[] Colliders = new Collider[10];

    [Range(-1, 1)]
    public float HideSensitivity = 0;
    [Range(0.01f, 1f)][SerializeField] private float UpdateFrequency = 0.65f;

    [SerializeField] private LayerMask HidableLayers;

    [Range(0, 5f)]
    private float MinObstacleHeight = 0f;

    public SceneChecker LineOfSightChecker;

    private Coroutine MovementCoroutine;

    private float fleeDistance; 


    // UI
    [SerializeField] private Slider _healthSlider;

    [SerializeField] private Slider _abilitySlider;

    // animation
    private Animator _animator;

    // fire damage variables
    private float damagePerSecondFire = 2f;
    private float durationOfFireDamage = 10f; 

    // states & actions
    private bool _canGloryKill;

    private bool _gamePlay;

    private bool _canMove;

    private bool _canSpecialAttack = false; 

    private bool _canAttack;

    private bool _isAttacking;

    private bool _canPeformAttack;


    // performance

    private AIHandler _handlerAI;

    private bool _deactivateAI; 


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
        _canMove = true;
        _canAttack = true;
        _isAttacking = false;
    }

    #region Components Sync
    private void GetComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        _warn = GetComponent<WarningSystemAI>();
        _handlerAI = GetComponent<AIHandler>();

        _agentAI = GetComponentInChildren<Agents>();
        _animator = GetComponentInChildren<Animator>();
        _healthSlider = GetComponentInChildren<Slider>();


        LineOfSightChecker = GetComponentInChildren<SceneChecker>();

        

       
        _player = FindObjectOfType<PlayerMovement>();
        PlayerObject = GameObject.Find("Player");

        _playerTarget = PlayerObject.transform;

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

        s_damageEffect = data.S_damageEffect;

        specialDamage = data.SpecialDamage;

        // projectiles //

        bullet = data.N_projectile;
        specialBullet = data.S_projectile;

        // cover //
        fleeDistance = data.FleeDistance; 

        
        // GEM //

        gemPrefab = data.Gem;

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
        State onGuardState = new State(("Guard"),
            null);

        State PatrolState = new State("On Patrol", 
            Patrol);

        State ChaseState = new State("Fight",
            ChasePlayer);

        State CoverState = new State("Cover",
           Cover);

        State GloryKillState = new State("Glory Kill",
       GloryKill);

   

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

        //CoverState.AddTransition(new Transition(() => _canAttack == true && canSeePlayer == false, ()=> Debug.Log("Cover State"), PatrolState));
        //CoverState.AddTransition(new Transition(() => _canAttack == true && canSeePlayer == true, () => Debug.Log("Cover State"), ChaseState));

        //ChaseState.AddTransition(new Transition(() => _canAttack == false, () => Debug.Log("Cover State"), CoverState));

        // Create the state machine
        stateMachine = new StateMachine(PatrolState);

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
                    StopAgent(); 
                    break;
                }
            case false:
                {
                    switch (_gamePlay)
                    {
                        case true:
                            {
                                outlineDeactivation.enabled = false;
                                ResumeAgent();

                                MinimalCheck(); // Tester

                                StartCoroutine(FOVRoutine());

                                AISpeed();

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

    private void MinimalCheck()
    {
        if (!_canGloryKill)
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

        if (curSpeed > 0.3)
        {
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }

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


        SetAttack(); 

        agent.speed = 4f;

        //print(_canSpecialAttack);


        switch(_canSpecialAttack)
        {
            case true:
                {
                    SpecialAttack();
                    break;
                }
            case false:
                {
                    
                    if ((_playerTarget.transform.position - transform.position).magnitude >= AttackRequiredDistance)
                    {
                        //transform.LookAt(_playerTarget.position);
                       

                        //transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));

                        agent.speed = 0;
                        StartAttacking();
                        if(_canAttack) { Attack(); }
                        
                        // se estiver atacando por x tempo


                        // mudar posição 

                    }

                    else if ((_playerTarget.transform.position - transform.position).magnitude < AttackRequiredDistance && !_canSpecialAttack) //|| currentAbilityValue < ABILITY_MAX_VALUE)
                    {
                        GetDistance();

                    }

                    CoolDoownPower();
                    break; 
                }
        }
        //print("ATTACK");
    }

    private void SpecialAttack()
    {
        transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));
        agent.SetDestination(_playerTarget.position);

        agent.stoppingDistance = 3.8f; 


        if((_playerTarget.transform.position - transform.position).magnitude <= 4f)
        {
             // look at ignoring player Y AXIS

            //Debug.Log(DebugColor + "Special attack" + closeColor);
            //print(currentAbilityValue);

            string DebugColor = "<size=14><color=orange>";
            string closeColor = "</color></size>";
            Debug.Log(DebugColor + "Special Attack" + closeColor);

            _player.TakeDamage(specialDamage);

            currentAbilityValue = 0;
            _abilitySlider.value = currentAbilityValue;
            _canSpecialAttack = false;
            StartCoroutine(SpecialAttackTimer());
            return; 
        }
    }

    private void CoolDoownPower()
    {
        
        if (currentAbilityValue >= ABILITY_MAX_VALUE)
        {
            _canSpecialAttack = true;
        }
        else
        {
            _canSpecialAttack = false;
        }

        currentAbilityValue = Mathf.Clamp(currentAbilityValue + (abilityIncreasePerFrame * Time.deltaTime), 0.0f, ABILITY_MAX_VALUE);

        _abilitySlider.value = currentAbilityValue;
        //print(currentAbilityValue);
    }

    private void Attack()
    {
        

        if (Time.time > nextFire)
        {
            transform.LookAt(new Vector3(_playerTarget.position.x, 0, _playerTarget.position.z));

            float randomFloat = UnityEngine.Random.value;

            //print("percentage is: "+randomPercentage);

            

            if(UnityEngine.Random.value < percentage)
            {
                _animator.SetBool("isAttacking", true);

                string DebugAttack = "<size=12><color=yellow>";
                string closeAttack = "</color></size>";
                Debug.Log(DebugAttack + "Attack 2: " + closeAttack + gameObject);

                Instantiate(specialBullet, _shootPos.position, _shootPos.rotation);
                

                StartCoroutine(AttackTimer()); 
            }
            else if(_canPeformAttack)
            {
                _animator.SetBool("isAttacking", true);

                string DebugAttack = "<size=12><color=green>";
                string closeAttack = "</color></size>";
                Debug.Log(DebugAttack + "Attack: " + closeAttack + gameObject);

                Instantiate(bullet, _shootPos.position, _shootPos.rotation);
            }
            nextFire = Time.time + fireRate;
        }
        else
        {
            _animator.SetBool("isAttacking", false);
            
        }
    }

    private IEnumerator SpecialAttackTimer()
    {
        _canAttack = false;
        yield return new WaitForSeconds(3f);
        _canAttack = true;
    }

    private IEnumerator AttackTimer()
    {
        _canPeformAttack = false;
         yield return new WaitForSeconds(2f);
        _canPeformAttack = true;


    }
    private void GetDistance()
    {
        agent.speed = 5f;
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
            // ALERT AI OF player presence
            _warn.canAlertAI = true;
            SetAttack(); 
            //GetPlayer();
            if(_canAttack) 
            {
                SetAttack(); 
            }

            switch (_type)
            {
                case WeaponType.Normal:
                    {

                        health -= _damage + damageBoost;

                        StartCoroutine(HitFlash());
                        break;
                    }
                case WeaponType.Ice:
                    {

                        health -= _damage + damageBoost;

                        StartCoroutine(STFS(5F));
                        break;
                    }
                case WeaponType.Fire:
                    {

                        health -= _damage + damageBoost;

                        StartCoroutine(DamageOverTime(damagePerSecondFire, durationOfFireDamage));
                        break;
                    }
                case WeaponType.Thunder: 
                    {
                        health -= _damage + damageBoost;
                        StartCoroutine(HitFlash());

                        break; 
                    }

                case WeaponType.Dash: 
                    {
                        health -= _damage + damageBoost;
                        StartCoroutine(HitFlash());

                        break; 
                    }

                default : { break; }
            }

            _healthSlider.value = health;
            HealthCheck();

            return;

            /*
            if (_Type == WeaponType.Normal)           
            {
  
                health -= _damage + damageBoost;
               
                //QuickCover();
                StartCoroutine(HitFlash());


            }
            else if (_Type == WeaponType.Ice)
            {

                // STOP FOR 5 seconds
                StartCoroutine(STFS(5F));

            }
            else if (_Type == WeaponType.Thunder)
            {
                health -= _damage + damageBoost;

                //QuickCover();
                StartCoroutine(HitFlash());
            }

            else if (_Type == WeaponType.Fire)
            {
                StartCoroutine(DamageOverTime(damagePerSecondFire, durationOfFireDamage));
            }
            else if (_Type == WeaponType.Dash)
            {
                health -= _damage + damageBoost;

                StartCoroutine(HitFlash());
            }
            */

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

    private void Die()
    {
        Instantiate(_death, transform.position, Quaternion.identity);   

        if(gemSpawnOnDeath)
        { 
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
            Debug.Log("Spawned Gem");
        }
        Destroy(gameObject);

        Debug.Log("Enemy died");
    }
    #endregion

    #region Combat IEnumerators
    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.gray;
    }

    private IEnumerator STFS(float value)
    {

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
        while (elapsedTime < durationOfFireDamage)
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
        return;
    }
    private void SetPatrol()
    {
        _stateAI = AI._PATROL;
        return;

    }
    private void SetAttack()
    {
        _stateAI = AI._ATTACK;
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
            _canAttack = false;
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


