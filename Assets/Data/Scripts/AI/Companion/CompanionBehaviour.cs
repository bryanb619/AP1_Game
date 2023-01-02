using System;
using System.Collections;
using UnityEngine;
//using URandom = UnityEngine.Random;
using LibGameAI.FSMs;
using UnityEngine.AI;

//using UnityEngine.Animations;

// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{
    //public Transform[] Pos;
    //float dist;

    //[SerializeField]private GameObject _EPI; // Enemy presence Image
    private mini _MiniMapCollor;

    [SerializeField]private NavMeshAgent Companion;
    [SerializeField] private Transform Target;
    public Transform playerTarget => Target;
    [HideInInspector] public bool _playerIsMoving;
    [HideInInspector] public bool _PlayerIsDashing;
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

    //private MeshRenderer mesh;
    [SerializeField] private MeshRenderer companionMesh; 

    private Color Newcolor;
    public float aFloat;

    // Create the FSM
    private void Start()
    {
        
        //_EPI = GameObject.Find("Minimap Border");
        _MiniMapCollor = FindObjectOfType<mini>();


        _playerIsMoving = false;



        // EPI

        StartCoroutine(FOVRoutine());

        


        // Create the states
        State IdleState = new State("",
            () => Debug.Log("Idle state"),
            Idle,
            () => Debug.Log(""));

        State FollowState = new State("",
            () => Debug.Log(""),
            Follow,
            () => Debug.Log(""));



        // Add the transitions
        
        // Idle
        IdleState.AddTransition(
            new Transition(
                () => _StartFollow == true, 
                () => Debug.Log(""),
                FollowState));

        // Follow
        FollowState.AddTransition(
           new Transition(
               () => _StartFollow == false,
               () => Debug.Log(""),
               IdleState));
        
        
        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        //Distance();
        //_Player.InMotion();

        LookAtUpdate();
        CheckMoveBool();
        CheckEnemy();
        

        Action actions = stateMachine.Update();
        actions?.Invoke();

    }
    /*
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
    */
    
    private void CheckMoveBool()
    {
        
        //Debug.Log(PlayerIsMoving);


        if (_playerIsMoving == true)
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
            //_EPI.SetActive(true);
            _MiniMapCollor.SetCollorRed();
        }
        else
        {
            //_EPI.SetActive(false);
            _MiniMapCollor.SetCollorDefault();
        }
    }



    // Chase the small enemy
    private void Idle()
    {

        // player is not moving (agent stop)

        //print("doing nothing now");
        Companion.speed = 2F;

        // follow only camera movement

        if(Companion.remainingDistance <= 3f)
        {
            CameraUpdatePos();
        }

        else if(Companion.remainingDistance >= 6F)
        {
            KetChup();
        }


       
    }

    private void LookAtUpdate()
    {
        RaycastHit HitInfo;
        Ray RayCast;

        RayCast = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0.5f));
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
            {
                transform.LookAt(HitInfo.point);
            }
            else
            {
                transform.LookAt(transform.forward);
            }
    }

    private void CameraUpdatePos()
    {
        //
        Companion.SetDestination(Target.position);

    }

    private void Follow()
    {

        // follow player and camera movement
        Companion.speed = 8F;
       // print("follow!!");
        Companion.SetDestination(Target.position);

        if (Companion.remainingDistance >= 5F)
        {
            KetChup();
        }
        if(Companion.remainingDistance >= 8)
        {

            transform.position = Target.transform.position;
            
        }
    }


    private void KetChup()
    {
        Companion.acceleration = 12;
        Companion.speed = 13F;
        Companion.SetDestination(Target.position);

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



    public void Setlow()
    {

        print("Set low");
        companionMesh.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);

    }

    public void SetHigh()
    {
        //print("set high"); 
        companionMesh.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);

    }



}
