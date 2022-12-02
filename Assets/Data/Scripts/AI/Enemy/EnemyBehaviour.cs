/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Animations;
using System.Runtime.CompilerServices;
using static UnityEngine.GraphicsBuffer;

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

    [SerializeField] private Transform Target;
    public Transform playerTarget => Target;

    private float minDist = 7f;


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

    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");
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
        stateMachine = new StateMachine(onGuardState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        MinimalCheck(); // Tester
       
        Action actions = stateMachine.Update();
        actions?.Invoke();
       
    }

    private void MinimalCheck()
    {
        if((playerTarget.transform.position - transform.position).magnitude < minDist)
        {
            transform.LookAt(playerTarget.position);
        }
    }


    // Chase 
    private void ChasePlayer()
    {
        transform.LookAt(playerTarget);

        _Agent.speed = 4f;
        _Agent.SetDestination(Target.position);
        
        // ADD Last known POS!



        // shoot
        if (_Agent.remainingDistance <= 10f && canSee == true)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                
                Instantiate(bullet, _shootPos.position, _shootPos.rotation);
            }

        }



        if (_Agent.remainingDistance <= 5f)
        {
            _Agent.speed = 0f;
        }

        //print("ATTACK");
    }


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
