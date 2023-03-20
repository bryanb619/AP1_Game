using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;

//using UnityEditor; // comment this before build


public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables

    private enum AI                             { _GUARD, _PATROL, _ATTACK, _COVER, _GLORYKILL, _NONE }

    private AI                                  _stateAI;

    private GameState                           _gameState;   

    [Header("AI Profile")]
    [SerializeField] private AIChaseData        data;

    private bool gemSpawnOnDeath =              true;
    private GameObject gemPrefab;


    // Reference to the state machine
    private StateMachine                        stateMachine;

    // Reference to the NAV MESH AGENT
    private NavMeshAgent                        agent;

    private Agents                              _agentAI; 


    // References to player
    [SerializeField] private GameObject         PlayerObject;
    private PlayerMovement _Player;

    [SerializeField] private Transform          playerTarget;
    public Transform PlayerTarget =>            playerTarget;

    private WarningSystemAI                     _warn;

    // Patrol Points

    [Header("Patrol")]
    private int destPoint = 0;
    [SerializeField] private Transform[]        _patrolPoints;

    // FOV
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
    private const int                           MAXHEALTH = 100;

    private float                               healthInCreasePerFrame;
    internal int                                damageBoost;

    // Get collor
    private Color                               originalColor;


    // damage
    private float                               damagePerSecondFire = 2f;
    private float                               durationOfFireDamage = 10f;

    private int                                 damage; 

    // Speed
    private Vector3                             previousPos;
    private float                               curSpeed;


    // weakness
    private bool                                _iceWeak, _fireWeak, _thunderWeak;


    // attack
    private float                               attackRate;
    private float                               nextAttack;

    private bool                                _canAttack = true;

    private bool                                _isAttacking;

    private float                               minDist;
    private float                               attackDist;


    // special ability 

    private const float                         ABILITY_MAX_VALUE = 100F;

    private float                               currentAbilityValue;

    private float                               abilityIncreasePerFrame; 
       


    // UI 
    [SerializeField] private Slider            _healthSlider;

    [SerializeField] private Slider            _AbilitySlider; 

    private TextMeshProUGUI                     damageText;


    #endregion

    #region Awake 
    // Get references to enemies
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        damageText = GetComponentInChildren<TextMeshProUGUI>();

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
        _stateAI = AI._PATROL; 

        GetComponents();
        GetProfile();
        GetStates();

        _healthSlider.value = _health;

        _AbilitySlider.value = currentAbilityValue; 
    }

    #region Components Sync
    private void GetComponents()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        //path = new NavMeshPath();

  
        _warn = GetComponent<WarningSystemAI>();

        LineOfSightChecker = GetComponentInChildren<SceneChecker>();
        //_healthSlider = GetComponentInChildren<Slider>();


        PlayerObject = GameObject.Find("Player");
        playerTarget = PlayerObject.transform;

        //playerTarget = GetComponent<Transform>();


        _Player = FindObjectOfType<PlayerMovement>();



    }
    #endregion

    #region profile Sync
    private void GetProfile()
    {

        // attack 
        minDist = data.MinDist;
        attackRate = data.AttackRate;
        attackDist = data.AttackDist; 
        
        // special attack
        abilityIncreasePerFrame = data.AbilityIncreasePerFrame;
        currentAbilityValue = data.CurrentAbilityValue; 

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
        // Weakness
        _iceWeak = data.Ice;
        _fireWeak = data.Fire;
        _thunderWeak = data.Thunder;
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
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log(""));

        State PatrolState = new State("Patrol",
            () => Debug.Log(""),
            Patrol,
            () => Debug.Log(""));

        // Combat states

        State ChaseState = new State("",
            () => Debug.Log("Chase"),
            ChasePlayer,
            () => Debug.Log(""));

        State FindCover = new State("Cover",
            () => Debug.Log(""),
            Cover,
            () => Debug.Log(""));

        State GloryKillState = new State("Glory Kill State",
            () => Debug.Log("Entered glory kill state"),
            null,
            () => Debug.Log("Left Glory Kill State"));

        #endregion


        #region Trasintion of states
        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                //canSeePlayer == true
                () => _stateAI == AI._ATTACK,
                () => Debug.Log("GUARD -> CHASE"),
                ChaseState));

        // CHASE -> COVER
        ChaseState.AddTransition(
           new Transition(
               //InCoverState == true
               () => _stateAI == AI._COVER,
               () => Debug.Log("CHASE -> COVER"),
               FindCover));

        // COVER -> CHASE
        FindCover.AddTransition(
           new Transition(
               () => //InCoverState == false 
                     _stateAI == AI._ATTACK,
               () => Debug.Log("COVER -> CHASE"),
               ChaseState));


        // PATROL -> CHASE
        PatrolState.AddTransition(
           new Transition(
               () => _stateAI == AI._ATTACK, // SEEK SOLUTION
               () => Debug.Log("PATROL -> CHASE"),
               ChaseState));

        // CHASE -> PATROL
        ChaseState.AddTransition(
           new Transition(
               () => _stateAI == AI._PATROL, // SEEK SOLUTION
               () => Debug.Log("CHASE -> PATROL"),
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
        if(_gameState == GameState.Gameplay)
        {
            ResumeAgent();

            CanFOV();

            MinimalCheck();

            HealthCheck();

            AISpeed();

            Action actions = stateMachine.Update();
            actions?.Invoke();
        }
        else if( _gameState == GameState.Paused)
        {
            PauseAgent();
        }
    }
    #endregion

    #region Agent State
    private void ResumeAgent()
    {
        if(canSeePlayer) 
        {
            SetAttack(); 
        }
        //Agent.Resume();
        agent.isStopped = false;
    }

    private void PauseAgent()
    {
        //Agent.speed = 0f; 
        //Agent.Stop(); 
        agent.isStopped = true;
        StopAllCoroutines();
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
        /*
        if ((playerTarget.transform.position - transform.position).magnitude < minDist)
        {
            //transform.LookAt(new Vector3(0,playerTarget.position.y, 0));
            //transform.LookAt(playerTarget);
            //Agent.SetDestination(playerTarget.position);
            SetAttack();
            
        }
        else
        {
           
        }
        */
    }

    #endregion

    #region Health Check 
    private void HealthCheck()
    {
        if (_health <= 15)
        {
            SetGloryKill();
        }


        else if (_health <= 50)//&& _Health > 10) 
        {
            SetCover();
        }

        else if (_health >= 75 && _stateAI == AI._COVER)
        {
            SetAttack();
        }


    }
    #endregion

    #region Speed
    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;
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

    #region Chase // Attack
    // Chase the small enemy
    private void ChasePlayer()
    {
        if ((playerTarget.transform.position - transform.position).magnitude < 3.9f)
        {
            agent.speed = 3.0f;
            Attack();
        }
        else
        {
            agent.speed = 4.0f;
            agent.SetDestination(playerTarget.position);
        }


        if (currentAbilityValue <= ABILITY_MAX_VALUE)
        {
                float INCREASEPERFRAMCE = 5F;

                print(currentAbilityValue);

                currentAbilityValue = Mathf.Clamp(currentAbilityValue + (INCREASEPERFRAMCE * Time.deltaTime), 0.0f, ABILITY_MAX_VALUE);

                _AbilitySlider.value = currentAbilityValue;
        }
        

    }

    private IEnumerator SearchTimer()
    {
        yield return new WaitForSeconds(5f);
        SetPatrol(); 
    }


    private void Attack()
    {

        if(currentAbilityValue >= ABILITY_MAX_VALUE )
        {
            print("special ability");

           // _Player.TakeDamage(25);

            currentAbilityValue = 0;
            _AbilitySlider.value = currentAbilityValue;
            
        }
        else if (Time.time > nextAttack)
        {
            //float randomPercentage = UnityEngine.Random.Range(0f, 100f);
            _Player.TakeDamage(10);
            print("DAMAGE DONE BY CHASE AI");

            nextAttack = Time.time + attackRate;
        }





    }
    #endregion

    #region Cover
    private void Cover()
    {
        agent.speed = 5f;
        agent.stoppingDistance = 1f;
        agent.radius = 1f;

        HandleGainSight(PlayerTarget);
        // GetCover();

        if (curSpeed <= 0.5 && !_isAttacking && _health >= 16)
        {
            _health = Mathf.Clamp(_health + (healthInCreasePerFrame * Time.deltaTime), 0.0f, MAXHEALTH);
            //Debug.Log("Chase health: " + _Health);
        }
        else
        {
            Attack();
        }

    }

    private void HandleGainSight(Transform Target)
    {

        agent.radius = 1f;

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
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
        print("patrol FIRED");
        _stateAI = AI._PATROL;

        agent.autoBraking = false;

        agent.stoppingDistance = 0.1f;
        agent.speed = 1f;
    }

    private void SetAttack()
    {
        //float xRotation = 0f;
        // Get the target position
        //Vector3 targetPos = playerTarget.position;

        // Ignore the Y component of the target position
        //targetPos.y = playerTarget.position.y;

        //ransform.LookAt(targetPos, Vector3.up);

        //transform.Rotate(Vector3.right, xRotation); 

        print("ATTACK FIRED");
        //print(agent.remainingDistance); 
        _stateAI = AI._ATTACK;

        // Agent configuration
        agent.speed = 4f; 
        agent.stoppingDistance = 3.9f;
        agent.angularSpeed = 0f;

    }
    private void SetCover()
    {
        _stateAI = AI._COVER;
      
    }

    private void SetGloryKill()
    {
        _stateAI = AI._GLORYKILL;
    }

    #endregion

    #region AI Health 
    public void TakeDamage(int _damage, WeaponType _Type)
    {
        
        _health -= _damage + damageBoost;

        damageText.text = _damage.ToString();

        StartCoroutine(DamageTextDisappear());

        if (_health <= 0)
        {
            Die();
        }
        else if (_health > 0)
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
                _health -= _damage + damageBoost;

                SetAttack(); 
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
                _health -= _damage + damageBoost;

                StartCoroutine(HitFlash());
            }
        }


        Debug.Log("enemy shot" + _health);
    }

    private void Die()
    {

        if (gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);

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
        }
    }

    public void StopAttacking()
    {
        if (_isAttacking)
        {
            _agentAI.StopAttacking();
            _canAttack = false;
        }

    }
    #endregion

    #region Camera rendering
    private void OnBecameVisible()
    {
        ResumeAgent(); 
    }
    private void OnBecameInvisible() 
    {
        PauseAgent();
    }
    #endregion

    #region Script destroy actions 
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    #endregion

    /*
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
                    Handles.Label(FOV.transform.position + Vector3.up, "Attack" + "  Gameplay: " , red);
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
        #endregion
#endif
    }
    #endregion
    */

}
