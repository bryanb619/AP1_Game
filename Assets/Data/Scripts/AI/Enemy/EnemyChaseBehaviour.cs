/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
//using URandom = UnityEngine.Random;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Animations;

// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseBehaviour : MonoBehaviour
{

    private Color originalColor;
    public int damage = 20;
    public int damageBoost = 0;

    private NavMeshAgent _Agent;
    private int _Health;

    // References to enemies
    private GameObject PlayerObject;

    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    [SerializeField] private Transform Target;
    public Transform playerTarget => Target;

    private float minDist = 7f;

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


    // Attack settings

    private Player_test _Player;

    private float AttackRate = 2f;
    private float nextAttack = 0f;




    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");
        
    }

    // Create the FSM
    private void Start()
    {
        _Health = 100;

        _Agent = GetComponent<NavMeshAgent>();
        _Player = FindObjectOfType<Player_test>();

        StartCoroutine(FOVRoutine());

        // Create the states
        State onGuardState = new State("",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log(""));

        State ChaseState = new State("",
            () => Debug.Log("Enter Fight state"),
            ChasePlayer,
            () => Debug.Log(""));

        State PatrolState = new State("no visual",
            () => Debug.Log("Enter Fight state"),
            Patrol,
            () => Debug.Log("Leave Fight state"));



        // Add the transitions

        onGuardState.AddTransition(
            new Transition(
                () => canSeePlayer == true , 
                () => Debug.Log("Player found!"),
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

        // Create the state machine
        stateMachine = new StateMachine(onGuardState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        MinimalCheck();
        Action actions = stateMachine.Update();
        actions?.Invoke();

  
    }

    private void MinimalCheck()
    {
        if ((playerTarget.transform.position - transform.position).magnitude < minDist)
        {
            transform.LookAt(playerTarget.position);
        }
    }


    // Chase the small enemy
    private void ChasePlayer()
    {
        transform.LookAt(playerTarget);

        _Agent.speed = 5f;
        _Agent.acceleration = 13f;
        _Agent.SetDestination(Target.position);


       if(_Agent.remainingDistance <= 2.1f)
       {
            Attack();
       }



        //print("ATTACK");
    }
    private void Attack()
    {

        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + AttackRate;
            _Player.TakeDamage(damage);

        }
    }


    private void Patrol()
    {


        _Agent.autoBraking = false;
        _Agent.stoppingDistance = 0f;

        if (!_Agent.pathPending && _Agent.remainingDistance < 0.5f)
            GotoNetPoint();
    }

    private void GotoNetPoint()
    {
        _Agent.autoBraking = false;

        _Agent.speed = 5f;
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
        transform.LookAt(playerTarget.position);

        if (_Health <= 0)
        {
            Die();
        }
        if (_Health > 0)
        {
            StartCoroutine(HitFlash());
        }

        _Health -= _damage + damageBoost;
        Debug.Log("enemy shot" + _Health);
    }

    private void Die()
    {
        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

       // Debug.Log("Enemy died");
    }


    IEnumerator HitFlash()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.gray;
    }
}
