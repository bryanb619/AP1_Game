﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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

// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseBehaviour : MonoBehaviour
{


    private NavMeshAgent _Agent;
    private float _Health;

    // References to enemies
    private GameObject PlayerObject;

    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    [SerializeField] private Transform Target;
    public Transform playerTarget => Target;

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

    // Get references to enemies
    private void Awake()
    {
        PlayerObject = GameObject.Find("Player");
    }

    // Create the FSM
    private void Start()
    {
        _Health = 100f;

        _Agent = GetComponent<NavMeshAgent>();

        StartCoroutine(FOVRoutine());

        // Create the states
        State onGuardState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log("Leave On Guard state"));

        State ChaseState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            ChasePlayer,
            () => Debug.Log("Leave Fight state"));



        // Add the transitions
        
        onGuardState.AddTransition(
            new Transition(
                () => canSeePlayer == true, 
                () => Debug.Log("Player found!"),
                ChaseState));
        /*
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
      
        Action actions = stateMachine.Update();
        actions?.Invoke();

        if (canSeePlayer == true)
        {
            //print("CAN SEE PLAYER IS: " + canSeePlayer);
        }
    }


    // Chase the small enemy
    private void ChasePlayer()
    {
        transform.LookAt(playerTarget);

        _Agent.speed = 4f;
        

        if(_Agent.remainingDistance <= 2.4f)
        {
            _Agent.speed = 0f;
            //_Agent.SetDestination(transform.position);
            Atack();

        }
        else
        {
            _Agent.speed = 4f;
            _Agent.SetDestination(Target.position);
        }


        //print("ATTACK");
    }
    private void Atack()
    {
        print("ATTACK");
    }


    private void Patrol()
    {
        /*
        
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

        */
    }
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



    public void TakeDamage(int _damage)
    {
        _Health -= _damage;
        Debug.Log("enemy shot");
        if (_Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Instantiate(transform.position, Quaternion.identity);
        Destroy(gameObject);

        // call for AI event
        //DieEvent.Invoke();

        Debug.Log("Enemy died");
    }
}
