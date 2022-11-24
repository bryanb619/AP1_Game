using System;
using UnityEngine;
//using URandom = UnityEngine.Random;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;
//using UnityEngine.Animations;

// The script that controls an agent using an FSM
//[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{
    public Transform[] Pos;
    //float dist;

    [SerializeField]private GameObject _EPI; // Enemy presence Image

    [SerializeField]private NavMeshAgent _Companion;
    [SerializeField] private Transform _Target;

    public bool PlayerIsMoving;
    private bool _StartFollow;

    //private Player_test _Player;


    // Reference to the state machine
    private StateMachine stateMachine;

    private bool EnemyIS;
    public bool canSee => EnemyIS;
    [Range(10, 150)]
    public float radius;
    [Range(50, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV; // Enemy Editor FOV

    // Create the FSM
    private void Start()
    {
        _Companion = GetComponent<NavMeshAgent>();
        //_Player = FindObjectOfType<Player_test>();
        PlayerIsMoving = false;

        // EPI

        StartCoroutine(FOVRoutine());




        // Create the states
        LibGameAI.FSMs.State IdleState = new State("On Idle",
            () => Debug.Log("Idle state"),
            Idle,
            () => Debug.Log("Left On Guard state"));

        State FollowState = new State("Follow",
            () => Debug.Log("Follow state"),
            Follow,
            () => Debug.Log("Left Follow state"));



        // Add the transitions
        
        // Idle
        IdleState.AddTransition(
            new Transition(
                () => _StartFollow == true, 
                () => Debug.Log("Player found!"),
                FollowState));

        // Follow
        FollowState.AddTransition(
           new Transition(
               () => _StartFollow == false,
               () => Debug.Log("Player found!"),
               IdleState));
        
        
        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        //Distance();
        //_Player.InMotion();

        CheckMoveBool();
        CheckEnemy();

        LookAtPlayerAim();



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
    
    private void CheckMoveBool()
    {
        
        //Debug.Log(PlayerIsMoving);


        if (PlayerIsMoving == true)
        {
            _StartFollow = true;
        }
        else
        {
            _StartFollow = false;
        }

    }
    private void CheckEnemy()
    {

        if(EnemyIS)
        {
            _EPI.SetActive(true);
        }
        else
        {
            _EPI.SetActive(false);
        }
    }



    // Chase the small enemy
    private void Idle()
    {

        // player is not moving (agent stop)

        //print("doing nothing now");
        _Companion.speed = 2F;

        // follow only camera movement

        if(_Companion.remainingDistance <= 3f)
        {
            CameraUpdatePos();
        }


       
    }

    private void CameraUpdatePos()
    {
        //
        _Companion.SetDestination(_Target.position);

    }

    private void Follow()
    {

        // follow player and camera movement
        _Companion.speed = 8F;
       // print("follow!!");
        _Companion.SetDestination(_Target.position);


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
                    EnemyIS = true;
                else
                    EnemyIS = false;
            }
            else
                EnemyIS = false;
        }
        else if (EnemyIS)
            EnemyIS = false;
    }

    private void LookAtPlayerAim()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 15))
        {
            Vector3 hitPosition = hit.point;
            transform.LookAt(hitPosition);
        }
    }

}
