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

// Request Nav Mesh as default
[RequireComponent(typeof(NavMeshAgent))]
// The script that controls an agent using an FSM
public class CompanionBehaviour : MonoBehaviour
{
    
    // Reference to the state machine
    private StateMachine stateMachine;


    private NavMeshAgent Agent;
    [SerializeField]
    private Player Player;
    [SerializeField]
    
    [Header("Idle Configs")]
   
    [Range(0, 10f)]
    private float RotationSpeed = 2f;
    [Header("Follow Configs")]
    [SerializeField]
    private float FollowRadius = 2f;

    // Get references to enemies
    private void Awake()
    {
      
    }

    // Create the FSM
    private void Start()
    {
        // Create the states
        State onGuardState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log("Leave On Guard state"));

        State fightState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            ChaseSmallEnemy,
            () => Debug.Log("Leave Fight state"));

        State runAwayState = new State("Runaway",
            () => Debug.Log("Enter Runaway state"),
            RunAway,
            () => Debug.Log("Leaving Runaway state"));

      

        // Add the transitions
        
        

        // Create the state machine
        stateMachine = new StateMachine(onGuardState);
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

       
    }

    // Runaway from the closest enemy
    private void RunAway()
    {
        
    }

    public void StateUpdate()
    {
        Debug.Log("Update called");
    }
}
