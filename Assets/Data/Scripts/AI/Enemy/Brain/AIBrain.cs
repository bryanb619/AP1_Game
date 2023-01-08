using System;
using UnityEngine;
using LibGameAI.FSMs;


/// <summary>
/// AI Brain
/// Script controls all AI behaviours and actions
/// </summary>
///
[CreateAssetMenu(menuName ="Akarya/AI/AI Brain")]
public class AIBrain : ScriptableObject
{ 

    #region Scripts and AI action
    private StateMachine stateMachine;
    //private FOV aiFov;

    #endregion
    #region state condition bools
    public bool _inCoverState;
    public bool _returnPatrol;
    public bool _inSearch;
    public bool _canSeePlayer;

    // sub bools

    public bool _canPatrol;

   

    #endregion

  

    #region Void Start 
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
            () => Debug.Log("Engaged"),
            EngagePlayer,
            () => Debug.Log(""));

        State SearchState = new State("Searching 4 player",
           () => Debug.Log(""),
           Search,
           () => Debug.Log(""));


        State FindCover = new State("need cover",
            () => Debug.Log(""),
            Cover,
            () => Debug.Log(""));

        #endregion

        #region Trasintion of states
        // Add the transitions
        onGuardState.AddTransition(
            new Transition(
                () => _canSeePlayer == true,
                () => Debug.Log("Player found!"),
                ChaseState));

        ChaseState.AddTransition(
           new Transition(
               () => _inCoverState == true,
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
               () => _inCoverState == false,
               () => Debug.Log(""),
               ChaseState));

        PatrolState.AddTransition(
           new Transition(
               () => _canSeePlayer == true,
               () => Debug.Log(""),
               ChaseState));


        #endregion

        stateMachine = new StateMachine(PatrolState);
      
    }
    #endregion

    // Request actions to the FSM and perform them
    public void Update()
    {
   
        Action actions = stateMachine.Update();
        actions?.Invoke();

        ConditionCheck();
    }

    private void ConditionCheck()
    {
        /*
        if(aiFov.CanSee)
        {
            _canSeePlayer = true; 
        }

        */
    }

    #region AI Actions

    private void Patrol()
    {
        
        
    }

    private void EngagePlayer()
    {
       
    } 

    private void Search()
    {
        
    }

    private void Cover()
    {
       
    }

    #endregion
}
