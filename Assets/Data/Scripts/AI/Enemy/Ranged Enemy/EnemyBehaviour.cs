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


// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{

    private Color originalColor;
    public int damageBoost = 0;

    GemManager gemManager;

    [SerializeField] private bool gemSpawnOnDeath;
    private NavMeshAgent _Agent;
    private float _Health;

    // References to enemies
    private GameObject PlayerObject;

    [SerializeField] private Transform PlayerTarget;
    public Transform playerTarget => PlayerTarget;

    private float minDist = 7f;

    private float curSpeed;
    private Vector3 previousPos;

    private bool InDanger, gloryKillCheck;

    private float AttackRequiredDistance = 8f;



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



    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");

        LineOfSightChecker = GetComponentInChildren<SceneChecker>();

    }

    // Create the FSM
    private void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());

        canSeePlayer = false;
        _Health = 100f;

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


        PatrolState.AddTransition(
           new Transition(
               () => canSeePlayer == true,
               () => Debug.Log(""),
               ChaseState));
        /* //
        onGuardState.AddTransition(
            new Transition(
                () =>
                    (bigEnemy.transform.position - transform.position).magnitude
                    < minDistanceToBigEnemy,
                () => Debug.Log("I just saw a big enemy!"),
                runAwayState));
        fightState.AddTransition(
            new Transition(
                () => URandom.value < 0.001f ||
                    (bigEnemy.transform.position - transform.position).magnitude
                        < minDistanceToBigEnemy,
                () => Debug.Log("Losing a fight!"),
                runAwayState));
        runAwayState.AddTransition(
            new Transition(
                () => (smallEnemy.transform.position - transform.position).magnitude
                        > minDistanceToSmallEnemy
                    &&
                    (bigEnemy.transform.position - transform.position).magnitude
                        > minDistanceToBigEnemy,
                () => Debug.Log("I barely escaped!"),
                onGuardState));
        */

        // Create the state machine
        stateMachine = new StateMachine(PatrolState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        if (gloryKillCheck)
            GloryKill();
        else
        {
            MinimalCheck(); // Tester

            Action actions = stateMachine.Update();
            actions?.Invoke();

            Vector3 curMove = transform.position - previousPos;
            curSpeed = curMove.magnitude / Time.deltaTime;
            previousPos = transform.position;
        }
    }

    private void GloryKill()
    {
        _Agent.isStopped = true;
    }

    private void HealthCheck()
    {
        if (_Health < 20.1f)
            gloryKillCheck = true;
    }

    private void MinimalCheck()
    {
        if ((playerTarget.transform.position - transform.position).magnitude < minDist)
        {
            transform.LookAt(playerTarget.position);
        }
    }


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
        transform.LookAt(playerTarget);

        _Agent.speed = 4f;
        _Agent.SetDestination(PlayerTarget.position);

        // ADD Last known POS
        //
        // !

        if ((playerTarget.transform.position - transform.position).magnitude >= AttackRequiredDistance)
        {
            _Agent.speed = 0;
            Attack();
        }

        if ((playerTarget.transform.position - transform.position).magnitude < AttackRequiredDistance)
        {

            GetDistance();
        }



        //print("ATTACK");
    }

    private void Attack()
    {

        transform.LookAt(playerTarget);

        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            Instantiate(bullet, _shootPos.position, _shootPos.rotation);
        }
    }
    

    private void GetDistance()
    {
        _Agent.speed = 5f;
        

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

            int hits = Physics.OverlapSphereNonAlloc(_Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

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


    private void Patrol()
    {
        _Agent.autoBraking = false;
        _Agent.stoppingDistance = 0f;

        if (!_Agent.pathPending && _Agent.remainingDistance < 0.5f)
        {
            GotoNetPoint();
        }

    }


    private void GotoNetPoint()
    {
        
        _Agent.speed = 3f;
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        _Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
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


    public void TakeDamage(int _damage)
    {
       



        if (_Health <= 0)
        {
            Die();
        }

        
        if (_Health > 0)
        {
            transform.LookAt(playerTarget.position);
            StartCoroutine(HitFlash());
        }
        _Health -= (_damage + damageBoost);
        // Debug.Log("enemy shot with " + (_damage + damageBoost) + " damage");


    }

    private void Die()
    {
        if(gemSpawnOnDeath)
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

        Debug.Log("Enemy died");
    }

    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.gray;
    }

}
