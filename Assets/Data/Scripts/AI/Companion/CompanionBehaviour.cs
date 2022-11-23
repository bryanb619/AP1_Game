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
//using System.Collections;
//using UnityEngine.Animations;

// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{
    public Transform[] Pos;

    private NavMeshAgent _Companion;


    // References to enemies
    //private GameObject PlayerObject;

    // Patrol Points


    // Reference to the state machine
    private StateMachine stateMachine;


    // Create the FSM
    private void Start()
    {

        _Companion = GetComponent<NavMeshAgent>();
       

        // Create the states
        State IdleState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            Idle,
            () => Debug.Log("Leave On Guard state"));

        State FollowState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            Follow,
            () => Debug.Log("Leave Fight state"));



        // Add the transitions
        /*
        // Idle
        IdleState.AddTransition(
            new Transition(
                () => canSeePlayer == true, 
                () => Debug.Log("Player found!"),
                FollowState));

        // Follow
        FollowState.AddTransition(
           new Transition(
               () => canSeePlayer == true,
               () => Debug.Log("Player found!"),
               IdleState));
        
        */
        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        // FOVRoutine();

        Distance();

        Action actions = stateMachine.Update();
        actions?.Invoke();

    }


    private void Distance()
    {
        for (int i = 0; i < Pos.Length; i++)
        {
            var dist = Vector3.Distance(transform.position, Pos[i].transform.position);

            if (dist <= 0.1f)
            {

            }
            else
            {

            }
        }
    }

    // Chase the small enemy
    private void Idle()
    {
        



       
    }

    private void Follow()
    {





    }

}
