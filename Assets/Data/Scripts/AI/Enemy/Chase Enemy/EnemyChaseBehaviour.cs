/*  */

using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Runtime.CompilerServices;

/// <summary>
/// Enemy AI chase behaviour
/// Script controls all AI behaviours and actions
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseBehaviour : MonoBehaviour
{
    #region Variables
    //Gem spawn
    [Header("Gem Spawn")]
    [SerializeField] private bool gemSpawnOnDeath = true;
    [SerializeField] private GameObject gemPrefab;


    // Reference to the state machine
    private StateMachine stateMachine;

    //private MeleeAttack AIAttack;

    private NavMeshAgent _Agent;
    private NavMeshPath path;
    private float _Health;

    // References to player
    private GameObject PlayerObject;
    private Player_test _Player;


    // Patrol Points

    [Header("Patrol Settings")]
    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    [SerializeField] private Transform playerTarget;
    public Transform PlayerTarget => playerTarget;


    [Header("AI Attack Settings")]
    //[Range(0, 10)] 
    [SerializeField] private float minDist = 7f;

    [Range(0, 15)][SerializeField] private float minDistInCover = 12f;

    [SerializeField] private float minAttackDist = 3f;


    [Range(10, 150)]
    public float radius;
    //public float Radius => radius;
    [Range(50, 360)]
    public float angle;
    //public float Angle => angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV; // Enemy Editor FOV

    private bool canSeePlayer;
    public bool canSee => canSeePlayer;

    // Attack rate
    [Range(1, 5)] private float AttackRate = 2f;
    private float nextAttack = 0f;


    private Color originalColor;
    public int damage = 20;
    public int damageBoost = 0;



    // hide code
    [Header("Hide config")]
    private Collider[] Colliders = new Collider[10];

    [Range(-1, 1)]
    [Tooltip("Lower is a better hiding spot")]
    public float HideSensitivity = 0;
    [Range(0.01f, 1f)][SerializeField] private float UpdateFrequency = 0.25f;

    [SerializeField] private LayerMask HidableLayers;


    private float MinObstacleHeight = 0.5f;

    public SceneChecker LineOfSightChecker;

    private Coroutine MovementCoroutine;


    [Range(0, 10)][SerializeField] private float healthInCreasePerFrame;

    //private const float MAXHEALTH = 100f;

    private Vector3 previousPos;

    private float curSpeed;

   

    // state condition bools
    private bool InCoverState;
    private bool _returnPatrol;
    private bool _inSearch;

    // sub state condition bools
    private bool IsAttacking;
    private bool _underAttack;
    bool _inAttackRange;


    private float retreatDist = 2f; 

    private PredictionModel pathPrediction;

    private Transform aiTransform;


    // new cover code
    // The speed at which the AI character moves
    public float moveSpeed = 5f;

    // The distance at which the AI character starts fleeing from the player
    public float fleeDistance = 15f;

    // The current cover position the AI character is using
    private Transform currentCoverPosition;



    #endregion
    // test code 





    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");
        _Player = FindObjectOfType<Player_test>();

        LineOfSightChecker = GetComponentInChildren<SceneChecker>();


    }
    #region Void Start 

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
 
        #region  States 
        // Non Combat states
        State onGuardState = new State("",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log(""));

        State PatrolState = new State("Patroling",
            () => Debug.Log(""),
            Patrol,
            () => Debug.Log(""));

        // Combat states

        State ChaseState = new State("",
            () => Debug.Log("Fighting"),
            ChasePlayer,
            () => Debug.Log(""));

        State SearchState = new State("Searching",
           () => Debug.Log(""),
           Search,
           () => Debug.Log(""));


        State FindCover = new State("Help",
            () => Debug.Log(""),
            Cover,
            () => Debug.Log(""));

        #endregion

        #region Trasintion of states
        // Add the transitions
        onGuardState.AddTransition(
            new Transition(
                () => canSeePlayer == true,
                () => Debug.Log("Player found!"),
                ChaseState));

        ChaseState.AddTransition(
           new Transition(
               () => InCoverState == true,
               () => Debug.Log(""),
               FindCover));

        ChaseState.AddTransition(
           new Transition(
               () => _inSearch == true,
               () => Debug.Log(""),
               SearchState));

        SearchState.AddTransition(
           new Transition(
               () => _inSearch == false,
               () => Debug.Log(""),
               ChaseState));

        SearchState.AddTransition(
           new Transition(
               () => _returnPatrol == true,
               () => Debug.Log(""),
               PatrolState));

        FindCover.AddTransition(
           new Transition(
               () => InCoverState == false,
               () => Debug.Log(""),
               ChaseState));

        PatrolState.AddTransition(
           new Transition(
               () => canSeePlayer == true,
               () => Debug.Log(""),
               ChaseState));

        /*
        ChaseState.AddTransition(
           new Transition(
               () => canSeePlayer == false && InCombat == false,
               () => Debug.Log(""),
               PatrolState));

        PatrolState.AddTransition(
           new Transition(
               () => canSeePlayer == true && InCombat == false,
               () => Debug.Log(""),
               ChaseState));
        */

        #endregion

        aiTransform = transform;

        stateMachine = new StateMachine(PatrolState);
        _Agent = GetComponent<NavMeshAgent>();
        //path = new NavMeshPath();

        pathPrediction = new PredictionModel();

        StartCoroutine(FOVRoutine());

        _Health = 100;

        
    }
    #endregion

    // Request actions to the FSM and perform them
    private void Update()
    {

     

        MinimalCheck();
        HealthCheck();
        AISpeed();
        SearchCS();
        Action actions = stateMachine.Update();
        actions?.Invoke();




    }


    #region Condition checked in update

    private void MinimalCheck()
    {
        if ((playerTarget.transform.position - transform.position).magnitude < minDist)
        {
            transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
            _inAttackRange = true;
        }
        else
        {
            _inAttackRange = false;
        }
    }
    private void HealthCheck()
    {
        if (_Health <= 50)//&& _Health > 10) 
        {
            InCoverState = true;
            //HandleGainSight(PlayerTarget); 
        }

        else if (_Health >= 75)
        {
            InCoverState = false;
        }


    }

    private void AISpeed()
    {
        Vector3 curMove = transform.position - previousPos;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPos = transform.position;
    }

    private void SearchCS()
    {
        
    }
    #endregion



    /*
    private void HandleLoseSight(Transform Target)
    {

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        playerTarget = Target;

    }
    */

    #region AI Actions

    private void Patrol()
    {
        _returnPatrol = false;
        _Agent.autoBraking = false;
        _Agent.stoppingDistance = 0f;

        if (!_Agent.pathPending && _Agent.remainingDistance < 0.5f)
        {
            GotoNetPoint();
        }

    }

    private void GotoNetPoint()
    {
        _Agent.autoBraking = false;

        _Agent.speed = 1.4f;
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        _Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

    // Chase the small enemy
    private void ChasePlayer()
    {

        //WarnOtherAi();

        if(_underAttack == true) 
        {
           // QuickCover();
        }

        if(_underAttack == false)
        {
            transform.LookAt(new Vector3(0, playerTarget.position.y, 0));

            _Agent.speed = 5f;
            _Agent.acceleration = 11f;

            _Agent.SetDestination(playerTarget.position);

            Attack();

            if (canSeePlayer == false)
            {
                _inSearch = true;
            }
        }


        

        //print("ATTACK");
    }

    private void WarnOtherAi()
    {
        float detectRadius = 10.0f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRadius);
        foreach (Collider collider in colliders)
        {
            // check if the GameObject attached to the collider is an enemy of the same type
            if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyChaseBehaviour>() != null)
            {
                
            }
        }

    }

    private void Attack()
    {
        if (_Agent.remainingDistance <= 3)
        {
            _Agent.stoppingDistance = 2.7f;

            if (_inAttackRange == true)
            {

                if (Time.time > nextAttack)
                {
                    //transform.LookAt(playerTarget);

                    nextAttack = Time.time + AttackRate;
                    //_Player.TakeDamage(damage);
                }
                else
                {
                    IsAttacking = false;
                }

            }
        }



    }
    private void QuickCover()
    {
        Vector3 retreatPoint = transform.position - transform.forward * retreatDist;
        _Agent.SetDestination(retreatPoint);

        // Make the character move towards the destination
        _Agent.isStopped = false;

    }
    

    private void Search()
    {
        

        if (_Agent.remainingDistance <= _Agent.stoppingDistance)
        {

            if(canSeePlayer)
            {
                _inSearch= false;
            }

            
            // Get the player's last position (their destination)
            Vector3 lastPosition = _Agent.destination;


            // * play animation ( Move FOV HEAD in Y rotation) and initiate again patrol state

            //Debug.Log("Player's last position: " + lastPosition);

            //GetPath();
            PathPredict();
             



        }
        
    }

    private void PathPredict()
    {
        //print("predicting");
        // Predict the target's path using the prediction model
        Vector3[] predictedPath = pathPrediction.PredictPath(PlayerObject);

        // Set the AI agent's destination to be the predicted position of the target at a certain point in the future
        _Agent.destination = predictedPath[predictedPath.Length - 1];
    }

    private void GetPath()
    {
        //print(" GetPath");
        // Assume that target is a GameObject with a Transform component
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();

        _Agent.destination = playerTarget.transform.position;
        NavMeshPath path = new NavMeshPath();
        _Agent.CalculatePath(playerTarget.transform.position, path);
       

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            // Use the path to guide the AI's movement
            print("i see you again");
        }
        else
        {
            
            // Modify the target position or find a new target
        }

    }



    private void Cover()
    {
        const float MAXHEALTH = 100f;


        _Agent.speed = 5f;
        _Agent.stoppingDistance = 1f;
        _Agent.radius = 1f;

        HandleGainSight(PlayerTarget);
       // GetCover();

        if (curSpeed <= 0.5 && IsAttacking == false && _Health > 10)
        {
            _Health = Mathf.Clamp(_Health + (healthInCreasePerFrame * Time.deltaTime), 0.0f, MAXHEALTH);
            //Debug.Log("Chase health: " + _Health);
        }
        Attack();

        
    }

    private void HandleGainSight(Transform Target)
    {

        _Agent.radius = 1f;

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //playerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }
    

    #endregion


    #region AI Health 
    public void TakeDamage(int _damage)
    {
        print(_Health);
        

        if (_Health <= 0)
        {
            Die();
        }
        if (_Health > 0)
        {
            //_underAttack= true;
            transform.LookAt(new Vector3(0, playerTarget.position.y, 0));
            QuickCover();
            StartCoroutine(HitFlash());
            
        }
        _Health -= _damage + damageBoost;
        //Debug.Log("enemy shot" + _Health);
    }

    

    private void Die()
    {

        if (gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);


        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

       // Debug.Log("Enemy died");
    }
    #endregion


    #region Coroutines
    #region Cover Routine
    
    private IEnumerator Hide(Transform Target)
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateFrequency);
        while (true)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(_Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

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
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, 2f, _Agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, _Agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSensitivity)
                    {
                        _Agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, _Agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, _Agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSensitivity)
                            {
                                _Agent.SetDestination(hit2.position);
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
            return Vector3.Distance(_Agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(_Agent.transform.position, B.transform.position));
        }
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
    

    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.magenta;
    }

    #endregion

}
