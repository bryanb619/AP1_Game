using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;

//using UnityEngine.Animations;

// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{


    //[SerializeField]private GameObject _EPI; // Enemy presence Image
    private mini _MiniMapCollor;

    [SerializeField] internal NavMeshAgent Companion;
    [SerializeField] private Transform Target;
    public Transform playerTarget => Target;

    [HideInInspector] public bool _playerIsMoving;

    private bool _StartFollow;

    //private PlayerMovement _Player;
    [Header("Mesh Configuration")]
    [SerializeField] private MeshRenderer CompanionMesh;
    [SerializeField] Material normal, AlphaLow;

    [SerializeField] private Transform AlphaPoint;

    private float minDist = 0.4f;
    // Reference to the state machine
    private LibGameAI.FSMs.StateMachine stateMachine;

    private bool _enemyIS;
    public bool canSee => _enemyIS;

    private bool _gameplay; 

    private GameState _gameState;

    [Range(10, 150)]
    public float radius;
    [Range(50, 360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [SerializeField] private Transform FOV;
    public Transform EEFOV => FOV; // Enemy Editor FOV
    float rotationSpeed = 10.0f;


    [SerializeField] private Camera mainCamera;
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                {
                    _gameplay = false;
                    break;
                }
            case GameState.Gameplay:
                {
                    _gameplay = true;
                    break;
                }
        }
    }
    // Create the FSM
    private void Start()
    {

        switch (_gameState)
        {
            case GameState.Paused:
                {
                    _gameplay = false;
                    break;
                }
            case GameState.Gameplay:
                {
                    _gameplay = true;
                    break;
                }
        }

        CompanionMesh = GetComponent<MeshRenderer>();
        _MiniMapCollor = FindObjectOfType<mini>();

        _playerIsMoving = false;
      
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
        if(_gameplay) 
        {
            StartCoroutine(FOVRoutine());

            LookAtUpdate();

            CheckMoveBool();
            CheckEnemy();
            AlphaUpdate();


            Action actions = stateMachine.Update();
            actions?.Invoke();
        }
    }

    private void LookAtUpdate()
    {
        // Get the cursor position in screen space
        //Vector3 cursorPosition = Input.mousePosition;

        // Convert the cursor position to world space
        //cursorPosition.z = mainCamera.transform.position.y;
        //cursorPosition = mainCamera.ScreenToWorldPoint(cursorPosition);

        // Make the object follow the cursor position
        //transform.position = cursorPosition;

        // Rotate the object in the y-axis based on the cursor position
        //Vector3 direction = cursorPosition - transform.position;
        //float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        float angle = Vector3.SignedAngle(transform.up, ray.direction, transform.forward);

        // Update the rotation of the transform
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Get the target position to rotate towards
            Vector3 targetPos = hit.point;

            // Ignore the y-axis to only rotate around the vertical axis
            targetPos.y = transform.position.y;

            // Calculate the direction to the target
            Vector3 UpdateDirection = targetPos - transform.position;

            // Calculate the rotation angle around the vertical axis
            float Updateangle = Mathf.Atan2(UpdateDirection.x, UpdateDirection.z) * Mathf.Rad2Deg;

            // Set the new rotation
            transform.rotation = Quaternion.Euler(0f, Updateangle, 0f);
        }
        /*
        if (Input.GetMouseButton(0))
        {
            // Get the direction from the companion to the mouse position
            Vector3 shootDirection = cursorPosition - transform.position;
            shootDirection.y = 0f;
            shootDirection.Normalize();

            // Calculate the angle between the companion's forward vector and the shoot direction
            float shootAngle = Vector3.SignedAngle(transform.forward, shootDirection, Vector3.up);

            // Rotate the companion towards the shoot direction
            transform.Rotate(0f, shootAngle, 0f);
        }
        */

        /*        RaycastHit HitInfo;
        Ray RayCast;

        RayCast = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            transform.LookAt(HitInfo.point);
        }
        else
        {
            transform.LookAt(transform.forward);
        }
        */
    }

    private void CheckMoveBool()
    {


        if (_playerIsMoving == true)
        {
            _StartFollow = true;
        }
        else
        {
            _StartFollow = false;
        }
    }


    // Chase the small enemy
    private void Idle()
    {

        // player is not moving (agent stop)

        //print("doing nothing now");
        Companion.speed = 2F;

        // follow only camera movement

        if (Companion.remainingDistance <= 3f)
        {
            CameraUpdatePos();
        }

        else if (Companion.remainingDistance >= 6F)
        {
            KetChup();
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
        Companion.speed = 10F;
        // print("follow!!");
        Companion.SetDestination(Target.position);

        if (Companion.remainingDistance >= 4F)
        {
            KetChup();
        }
        else if (Companion.remainingDistance >= 6f)
        {

            transform.position = Target.transform.position;

        }
    }


    private void KetChup()
    {
        Companion.acceleration = 12;
        Companion.speed = 15F;
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
                    _enemyIS = true;
                else
                    _enemyIS = false;
            }
            else
                _enemyIS = false;
        }
        else if (_enemyIS)
            _enemyIS = false;
    }

    private void AlphaUpdate()
    {
        /*
        float maxDistance = 5.0f; 

        float distance = Vector3.Distance(transform.position, AlphaPoint.position);
        lerpProgres = Mathf.Clamp01(distance / maxDistance);
        CompanionMesh.material.Lerp(normal, AlphaLow, transitionSpeed);
        */
        if ((AlphaPoint.transform.position - transform.position).magnitude < minDist)
        {
            Setlow();

           
        }
        else
        {
            SetHigh();
        }
    }

    private void CheckEnemy()
    {

        if (_enemyIS)
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

    private void Setlow()
    {
        // change to transparent material version
        CompanionMesh.material = AlphaLow;

    }

    private void SetHigh()
    {
        // change to normal material
        CompanionMesh.material = normal;
    }
 


}
