/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;
using UnityEditor;


// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    private enum AI
    {
        _GUARD,
        _PATROL,
        _ATTACK,
        _COVER,
        _SEARCH,
        _GLORYKILL,
        _NONE
    }

    private AI _stateAI; 

    private Color originalColor;
    public int damageBoost = 0;

    GemManager gemManager;

    [SerializeField] private bool gemSpawnOnDeath;
    internal NavMeshAgent Agent;
    private float health;

    // References to enemies
    private GameObject PlayerObject;

    [SerializeField] private Transform PlayerTarget;
    public Transform playerTarget => PlayerTarget;

    private float minDist = 7f;

    private float curSpeed;
    private Vector3 previousPos;

    private bool InDanger;

    private float AttackRequiredDistance = 8f;

    private float randomPercentage;

    private float maxPercentage = 75f; 


    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    // Reference to the state machine
    private StateMachine stateMachine;

    [Range(10, 150)]
    public float radius;
    [Range(50, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV; // Enemy Editor FOV

    private bool canSeePlayer;
    public bool canSee => canSeePlayer;

    [SerializeField]
    private Transform _shootPos;

    [SerializeField] private GameObject bullet, gemPrefab;

    private float fireRate = 2f;
    private float nextFire = 0f;


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

    // fire damage variables
    private float damagePerSecondFire = 2f;
    private float durationOfFireDamage = 10f; 

    private bool _canGloryKill;

    private bool _gamePlay;

    private bool _canMove;


   

    // Get references to enemies
    private void Awake()
    { 
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    // Create the FSM
    private void Start()
    {

        randomPercentage = UnityEngine.Random.Range(0f, 60f) * 100f;

        print(randomPercentage); 
        _canMove = true;

        canSeePlayer = false;
        health = 100f;

        Agent = GetComponent<NavMeshAgent>();
        LineOfSightChecker = GetComponentInChildren<SceneChecker>();

        PlayerObject = GameObject.Find("Player");
        //StartCoroutine(FOVRoutine());
       
        // Create the states
        State onGuardState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log("Leave On Guard state"));

        State ChaseState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            ChasePlayer,
            () => Debug.Log("Leave Fight state"));


        State PatrolState = new State("no visual",
            () => Debug.Log("Enter Fight state"),
            Patrol,
            () => Debug.Log("Leave Fight state"));

        State GloryKillState = new State("Glory Kill State",
            () => Debug.Log("Entered glory kill state"),
            GloryKill,
            () => Debug.Log("Left Glory Kill State"));


    
        // Add the transitions

        onGuardState.AddTransition(
            new Transition(
                () => canSeePlayer == true,
                () => Debug.Log(""),
                ChaseState));

        ChaseState.AddTransition(
            new Transition(
                () => canSeePlayer == false,
                () => Debug.Log(""),
                PatrolState));

        ChaseState.AddTransition(
            new Transition(
                () => _canGloryKill == true,
                () => Debug.Log(""),
                GloryKillState));   

        PatrolState.AddTransition(
           new Transition(
               () => canSeePlayer == true,
               () => Debug.Log(""),
               ChaseState));

        
       

        // Create the state machine
        stateMachine = new StateMachine(PatrolState);
    }

    private void FixedUpdate()
    {
        
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        UpdateAI();
    }

    private void UpdateAI()
    {
        switch(_gamePlay)
        {
            case true:
                {
                    ResumeAgent();
                    break;
                }
            case false:
                {
                    PauseAgent();
                    break;
                }  
        }
        
    }

    private void ResumeAgent()
    {
        //Agent.Resume();
        Agent.isStopped = false;

        MinimalCheck(); // Tester

        StartCoroutine(FOVRoutine());

        Action actions = stateMachine.Update();
        actions?.Invoke();

        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;
    }

    private void PauseAgent()
    {
        //Agent.speed = 0f; 
        //Agent.Stop();
        Agent.isStopped = true;
         
        StopAllCoroutines();
    }




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



    private void MinimalCheck()
    {
        if (_canGloryKill == false)
        {
            if ((playerTarget.transform.position - transform.position).magnitude < minDist)
            {
                //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
                transform.LookAt(playerTarget.position);
            }
        }
       
    }

    void OnPlayerWarning(Vector3 Target)
    {
        // The player has been detected within the warning radius!
        // Do something to react to this, such as chasing the player or going into alert mode.
        GetPlayer();

    }

    public void GetPlayer()
    {
        if(_canMove && _gamePlay)
        {
            transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
        }
        
    }

    #region AI ACTIONS
    private void HandleGainSight(Transform Target)
    {

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        PlayerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }


    // Chase 
    private void ChasePlayer()
    {
        if(_canMove)
        {
            //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
            transform.LookAt(playerTarget.position);
            //transform.LookAt(new Vector3(0, playerTarget.position.y, 0));

            Agent.speed = 4f;
            Agent.SetDestination(PlayerTarget.position);


            if ((playerTarget.transform.position - transform.position).magnitude >= AttackRequiredDistance)
            {
                Agent.speed = 0;
                Attack();

                // se estiver atacando por x tempo


                // mudar posição 

            }

            else if ((playerTarget.transform.position - transform.position).magnitude < AttackRequiredDistance)
            {
                GetDistance();
            }
        }
        



        //print("ATTACK");
    }

    private void Attack()
    {

        //transform.LookAt(playerTarget);
        transform.LookAt(playerTarget);

        if (Time.time > nextFire)
        {
            randomPercentage =  UnityEngine.Random.Range(0f, maxPercentage) * 100f;

            if(randomPercentage >= 50)
            {
                Instantiate(bullet, _shootPos.position, _shootPos.rotation);
            }
            nextFire = Time.time + fireRate;


        }
    }
    

    private void GetDistance()
    {
        Agent.speed = 5f;
        Agent.acceleration = 12;
        

        if (curSpeed <= 1 && canSee)
        {

            Attack();
            //Debug.Log("Chase health: " + _Health);
        }

        HandleGainSight(PlayerTarget);

    }
    
    #region hide Routine

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


    private void Patrol()
    {
        if(_canMove)
        {
            Agent.autoBraking = false;
            Agent.stoppingDistance = 0f;

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
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

    private void GloryKill()
    {
        Agent.radius = 1f;
        Agent.isStopped = true;
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
                    canSeePlayer = true;
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


    public void TakeDamage(int _damage, WeaponType _Type)
    {
        health -= (_damage + damageBoost);

        if (health <= 0)
        {
            Die();
        }

        if (health > 0)
        {
            // ALERT AI OF player presence
            WarningSystemAI warn;
            warn = GetComponent<WarningSystemAI>();
            warn.canAlertAI = true;
            GetPlayer();

            if (_Type == WeaponType.Normal)
            {
                //if (_Health <= 20)
                //{
                //_canGloryKill = true;
                //}
                health -= _damage + damageBoost;
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
                health -= _damage + damageBoost;

                StartCoroutine(HitFlash());
            }
        }

        // Debug.Log("enemy shot with " + (_damage + damageBoost) + " damage");
    }

    private void Die()
    {
        if(gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

        Debug.Log("Enemy died");
    }


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
    }
    private void SetPatrol()
    {
        _stateAI = AI._PATROL;
    }
    private void SetAttack()
    {
        _stateAI = AI._ATTACK;
    }
    private void SetCover()
    {
        _stateAI = AI._COVER;
    }
    private void SetSearch()
    {
        _stateAI= AI._SEARCH;   
    }
    private void SetGloryKill()
    {
        _stateAI = AI._GLORYKILL;
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

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

    */
}
