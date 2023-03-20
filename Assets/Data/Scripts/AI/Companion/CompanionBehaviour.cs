using System;
using System.Collections;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime;

//using UnityEngine.Animations;

// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{
    #region Variables
    private enum CompanionState { _idle, _follow, _rotate}
    private CompanionState _StateAI;  

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
    private StateMachine stateMachine;

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

    [SerializeField] private LayerMask _attackLayers;


    [SerializeField] private Camera mainCamera;

    [SerializeField] Transform companionAim;

    private bool _canRotate; 
    [SerializeField] private GameObject target;
    [SerializeField] private float degreesPerSecond = 45;

    #endregion

    #region Awake
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }
    #endregion

    #region Start
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
        //mainCamera = Camera.main;
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

        State RotateState = new State("",
            () => Debug.Log(""),
            RotateAroundPlayer,
            () => Debug.Log(""));



        // Add the transitions


        // Idle -> Follow
        IdleState.AddTransition(
            new Transition(
                () => _StateAI == CompanionState._follow,
                () => Debug.Log(""),
                FollowState));

        // Follow -> Idle
        FollowState.AddTransition(
           new Transition(
               () => _StateAI == CompanionState._idle,
               () => Debug.Log(""),
               IdleState));


        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    #endregion

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        if(_gameplay) 
        {
            ResumeAgent();
            StartCoroutine(FOVRoutine());
            Companion.updateRotation = false; 

            Aim();

            CheckMoveBool();
            CheckEnemy();
            //AlphaUpdate();
            //RotateTimer();

            Action actions = stateMachine.Update();
            actions?.Invoke();
        }
        else if(!_gameplay)
        {
            AgentPause();
        }
    }
    #endregion

    private void LookAtUpdate()
    {
       
        /*
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));

        // Calculate the direction from the player to the mouse position
        Vector3 direction = mousePos - transform.position;
        direction.y = 0f; // Ignore the Y component of the direction

        // Rotate the player towards the mouse position
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Cast a ray from the camera position to the mouse position
        Ray ray = new Ray(mainCamera.transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MaxHit))
        {
            // Calculate the cursor offset based on the hit distance
            cursorOffset = hit.distance / mainCamera.transform.position.y;

            // Debug draw the adjusted ray
            Debug.DrawRay(mainCamera.transform.position, direction * (hit.distance + cursorOffset), Color.red);
        }
        else
        {
            // If the ray doesn't hit anything, use a default offset value
            //cursorOffset = 1f;

            // Debug draw the unadjusted ray
            Debug.DrawRay(mainCamera.transform.position, direction * 10f, Color.red);
        }



        /*
       //Get the mouse position in the game world
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));

        // Calculate the direction from the player to the mouse position
        Vector3 direction = mousePos - transform.position;
        direction.y = 0f; // Ignore the Y component of the direction

        // Rotate the player towards the mouse position

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            companionAim.position = mousePos;   
        }
        Debug.DrawRay(transform.position, direction, Color.yellow);
        */
    }

    #region Raycast aim mouse Update
    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // You might want to delete this line.
            // Ignore the height difference.
            direction.y = 0;

            // Make the transform look in the direction.
            transform.forward = direction;
            
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _attackLayers))
        {
            companionAim.transform.position = hitInfo.point;
            return (success: true, position: hitInfo.point);
               
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    /*
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

    /*

    //if (Input.GetMouseButton(0))
    if(Input.GetMouseButton(0))
    {

        RaycastHit Mousehit;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            Vector3 shootDirection = hit.transform.position - transform.position;
            shootDirection.y = 0f;
            shootDirection.Normalize();

            // Calculate the angle between the companion's forward vector and the shoot direction
            float shootAngle = Vector3.SignedAngle(transform.forward, shootDirection, Vector3.up);

            // Rotate the companion towards the shoot direction
            transform.Rotate(0f, shootAngle, 0f);
        }

        /*
        // Get the direction from the companion to the mouse position
        Vector3 shootDirection = targetPos - transform.position;
        shootDirection.y = 0f;
        shootDirection.Normalize();

        // Calculate the angle between the companion's forward vector and the shoot direction
        float shootAngle = Vector3.SignedAngle(transform.forward, shootDirection, Vector3.up);

        // Rotate the companion towards the shoot direction
        transform.Rotate(0f, shootAngle, 0f);


    }
     */
    /*
    private void NewAim()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {

            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.magenta);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            companionAim.transform.position = pointToLook;
        }
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // You might want to delete this line.
            // Ignore the height difference.
            direction.y = 0;

            // Make the transform look in the direction.
            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _attackLayers)) 
        {
            return(success: true, position: hitInfo.point); 
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }
        
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

    #endregion

    #region AI Actions
    private void CheckMoveBool()
    {
        //print(_playerIsMoving); 

        if (_playerIsMoving)
        {
            //_StartFollow = true;
            _StateAI = CompanionState._follow;
        }
        else if(!_playerIsMoving)
        {
            // _StartFollow = false;
            _StateAI = CompanionState._idle;
        }
    }


    #region Idle State
    // Chase the small enemy
    private void Idle()
    {
        // player is not moving (agent stop)

        //print("doing nothing now");
        //Companion.isStopped = true;
        Companion.SetDestination(Target.position); 
       
        if (Companion.remainingDistance >= 3f)
        {

            DashUpate();

        }
        // follow only camera movement


    }

    private void DashUpate()
    {
        Companion.acceleration = 10F;
        Companion.speed = 12F;
    }

    private void RotateTimer()
    {
        if(_canRotate) 
        {
            float elapsed = 0F;

            elapsed += Time.deltaTime;
            print(elapsed); 
            if(elapsed >= 5f)
            {
                _StateAI = CompanionState._rotate; 
            }

        }
        

        
    }

    private void RotateAroundPlayer()
    {
        //transform.RotateAround(target.transform.position, Vector3.forward, degreesPerSecond * Time.deltaTime);
    }


    private void CameraUpdatePos()
    {
        Companion.isStopped = false;
        Companion.SetDestination(Target.position);
    }

    #endregion
    #endregion

    #region Follow State
    private void Follow()
    {
        Companion.isStopped = false;
        Companion.angularSpeed = 0;

        Companion.SetDestination(Target.position);

        if (Companion.remainingDistance >= 4F)
        {
            KetChup();
        }
      
       
    }

    private void FollowTargetNormal()
    {
        Companion.speed = 3f; 
        Companion.acceleration = 8f; 
    }

    private void KetChup()
    {
        Companion.acceleration = 12F;
        Companion.speed = 10F;
     
    }

    #endregion

    #region Enemy detection
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

    private void CheckEnemy()
    {

        if (_enemyIS)
        {
            //_EPI.SetActive(true);
            //_MiniMapCollor.SetCollorRed();
        }
        else
        {
            //_EPI.SetActive(false);
            //_MiniMapCollor.SetCollorDefault();
        }
    }
    #endregion

    #region Alpha update
    private void AlphaUpdate()
    {
        /*
        float maxDistance = 5.0f; 

        float distance = Vector3.Distance(transform.position, AlphaPoint.position);
        lerpProgres = Mathf.Clamp01(distance / maxDistance);
        CompanionMesh.material.Lerp(normal, AlphaLow, transitionSpeed);
        */
        /*
        if ((AlphaPoint.transform.position - transform.position).magnitude < minDist)
        {
            Setlow();

           
        }
        else
        {
            SetHigh();
        }
        */
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
    #endregion

    #region Gamplay State
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
    private void ResumeAgent()
    {
        if(_StateAI == CompanionState._follow) 
        {
           Companion.isStopped = true;
        }
        else 
        {
            Companion.isStopped = false;    
        }
    }

    private void AgentPause()
    {
        Companion.isStopped = true;
    }
    #endregion

    #region Destroy
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    #endregion

}
