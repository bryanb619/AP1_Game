/*  */

using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;

/// <summary>
/// AI Brain
/// Script controls all AI behaviours and actions
/// </summary>
/// 
public class EnemyChaseBrain : MonoBehaviour
{
    #region Variables
    

    // Reference to the state machine
    private StateMachine stateMachine;



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



    private Color originalColor;
    public int damage = 20;
    public int damageBoost = 0;


  
    // state condition bools
    private bool InCoverState;
    private bool _returnPatrol;
    private bool _inSearch;

    // sub state condition bools
    private bool IsAttacking;
    private bool _underAttack;
    bool _inAttackRange;


    #endregion



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

        PatrolState.AddTransition(
           new Transition(
               () => canSeePlayer == true,
               () => Debug.Log(""),
               ChaseState));


        #endregion

        stateMachine = new StateMachine(PatrolState);
       
        StartCoroutine(FOVRoutine());


        
    }
    #endregion

    // Request actions to the FSM and perform them
    private void Update()
    {
        Action actions = stateMachine.Update();
        actions?.Invoke();
    }


    #region AI Actions

    private void Patrol()
    {
        

    }

    private void GotoNetPoint()
    {
       
    }

    // Chase the small enemy
    private void ChasePlayer()
    {
        
        

        //print("ATTACK");
    }
    private void Attack()
    {
       



    }
    private void QuickCover()
    {
       
        
       


    }
    

    private void Search()
    {
        

        
        
    }

    private void PathPredict()
    {
       
    }

    private void GetPath()
    {
        

    }



    private void Cover()
    {
       

    }
   

    private void HandleGainSight(Transform Target)
    {

        
    }
    #endregion



    public void TakeDamage(int _damage)
    {
        

        
    }

    

    private void Die()
    {

        
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
    
}
