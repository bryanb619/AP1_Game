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

// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{


    private NavMeshAgent _Agent;
    public float _Health;

    // References to enemies
    private GameObject smallEnemy;

    // Patrol Points

    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    [SerializeField] private Transform Target;

    // Reference to the state machine
    private StateMachine stateMachine;

    // Get references to enemies
    private void Awake()
    {
        
    }

    // Create the FSM
    private void Start()
    {
        _Health = 100f;

        _Agent = GetComponent<NavMeshAgent>();

        // Create the states
        State onGuardState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log("Leave On Guard state"));

        State OnPatrolState = new State("On Patrol",
            () => Debug.Log("In Patrol state"),
            Patrol,
            () => Debug.Log("Left Patrol State"));

        State fightState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            ChaseSmallEnemy,
            () => Debug.Log("Leave Fight state"));

        State runAwayState = new State("Runaway",
            () => Debug.Log("Enter Runaway state"),
            RunAway,
            () => Debug.Log("Leaving Runaway state"));

        // Add the transitions
        /*
        onGuardState.AddTransition(
            new Transition(
                () =>
                    (smallEnemy.transform.position - transform.position).magnitude
                    < minDistanceToSmallEnemy,
                () => Debug.Log("I just saw a small enemy!"),
                fightState));
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
        stateMachine = new StateMachine(OnPatrolState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        Action actions = stateMachine.Update();
        actions?.Invoke();
    }

    // Chase the small enemy
    private void ChaseSmallEnemy()
    {
        _Agent.SetDestination(Target.position);
    }


    private void Patrol()
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

    // Runaway from the closest enemy
    private void RunAway()
    {
       
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
