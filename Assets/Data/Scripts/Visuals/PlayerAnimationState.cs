using System;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;

/*
public class PlayerAnimationState : MonoBehaviour
{
    private StateMachine stateMachine;

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        /
        // Create the states
        State IdleState = new State("",
            () => Debug.Log("Idle state"),
            Idle,
            () => Debug.Log(""));

        State RunState = new State("",
            () => Debug.Log("Run state"),
            Run,
            () => Debug.Log(""));

       
        stateMachine = new StateMachine(IdleState);

        agent= GetComponentInParent<NavMeshAgent>();    
    }

    // Update is called once per frame
    void Update()
    {
        Action actions = stateMachine.Update();
        actions?.Invoke();
    }

    private void Idle()
    {

    }

    private void Run()
    {

    }
}*/
