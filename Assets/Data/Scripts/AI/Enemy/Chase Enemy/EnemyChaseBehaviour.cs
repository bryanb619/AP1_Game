/*  */

using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;

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

    private NavMeshAgent Agent;
    private NavMeshPath path;
    private float _Health;

    // References to player
    private GameObject PlayerObject;
    private PlayerMovement _Player;


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

    private bool _canAttack; 
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
    private bool _canGloryKill; 

    // sub state condition bools
    private bool IsAttacking;
    private bool _underAttack;
    bool _inAttackRange;


    private float retreatDist = 2f; 

    private PredictionModel pathPrediction;

    private Transform aiTransform;


    [SerializeField] private SpawnArea patrolZone;
    public SpawnArea PatrolZone { get => patrolZone; set => patrolZone = value; }

    // new cover code
    // The speed at which the AI character moves
    public float moveSpeed = 5f;

    // The distance at which the AI character starts fleeing from the player
    public float fleeDistance = 15f;

    // The current cover position the AI character is using
    private Transform currentCoverPosition;


    public float aiPointRadius;
    #endregion
    // test code 

    private Vector3 dest = Vector3.zero;



    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");
        _Player = FindObjectOfType<PlayerMovement>();

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

        State GloryKillState = new State("Glory Kill State",
            () => Debug.Log("Entered glory kill state"),
            GloryKill,
            () => Debug.Log("Left Glory Kill State"));

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

        ChaseState.AddTransition(
            new Transition(
                () => _canGloryKill == true,
                () => Debug.Log(""),
                GloryKillState));

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

        FindCover.AddTransition(
          new Transition(
              () => _canGloryKill == true,
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
        Agent = GetComponent<NavMeshAgent>();
        //path = new NavMeshPath();

        pathPrediction = new PredictionModel();

        StartCoroutine(FOVRoutine());

        _Health = 100;
        _canAttack = true;

        
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
        if(_canGloryKill == false)
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
        Agent.autoBraking = false;
        Agent.stoppingDistance = 0.2f;
        Agent.speed = 1f;
        Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
        {
            GotoNetPoint();
        }

    }

    private void GotoNetPoint()
    {
       
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

    // Chase the small enemy
    private void ChasePlayer()
    {
        
        //WarnOtherAi();

        if (_underAttack == true) 
        {
            RandomMovement();
        }

        if(_underAttack == false)
        {
            transform.LookAt(new Vector3(0, playerTarget.position.y, 0));

            Agent.speed = 5f;
            Agent.acceleration = 11f;

            Agent.SetDestination(playerTarget.position);

            Attack();

            if (canSeePlayer == false)
            {
                _inSearch = true;
            }
        }


        

        //print("ATTACK");
    }

    private void RandomMovement()
    {
        
        if (Agent.remainingDistance >= 0.1f) return;

        do
        {
            float rotationValue = UnityEngine.Random.Range(0, 359);
            float rangeValue = UnityEngine.Random.Range(2f, 5f);
            //this.transform.localEulerAngles = this.transform.localEulerAngles.y(rotationValue);
            Vector3 direction = gameObject.transform.forward.normalized;
            dest = transform.position + direction * rangeValue;
        } while (!PatrolZone.InArea(dest));


        Agent.SetDestination(dest);
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
        if (Agent.remainingDistance <= 3)
        {
            Agent.stoppingDistance = 2.7f;

            if (_inAttackRange == true)
            {

                if (Time.time > nextAttack)
                {

                    print("player attacked");
                    //transform.LookAt(playerTarget);

                    nextAttack = Time.time + AttackRate;
                    _Player.TakeDamage(damage);
                    _canAttack = false;
                }
                else
                {
                    IsAttacking = false;
                }

            }
        }
       


    }

    public void GetPlayer()
    {

    }

    private void QuickCover()
    {
        Vector3 retreatPoint = transform.position - transform.forward * retreatDist;
        Agent.SetDestination(retreatPoint);

        // Make the character move towards the destination
        Agent.isStopped = false;

    }
    

    private void Search()
    {
        

        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {

            if(canSeePlayer)
            {
                _inSearch= false;
            }

            
            // Get the player's last position (their destination)
            Vector3 lastPosition = Agent.destination;


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
        Agent.destination = predictedPath[predictedPath.Length - 1];
    }

    private void GetPath()
    {
        //print(" GetPath");
        // Assume that target is a GameObject with a Transform component
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();

        Agent.destination = playerTarget.transform.position;
        NavMeshPath path = new NavMeshPath();
        Agent.CalculatePath(playerTarget.transform.position, path);
       

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


        Agent.speed = 5f;
        Agent.stoppingDistance = 1f;
        Agent.radius = 1f;

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

        Agent.radius = 1f;

        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        //playerTarget = Target;

        MovementCoroutine = StartCoroutine(Hide(Target));
    }

    private void GloryKill()
    {
        Agent.radius = 1f;
        Agent.isStopped = true;
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
            if (_Health <= 20)
            {
                _canGloryKill = true;
            }
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

            int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

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
