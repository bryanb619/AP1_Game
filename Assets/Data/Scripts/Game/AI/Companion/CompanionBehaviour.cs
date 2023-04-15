using System;
using UnityEngine;
using LibGameAI.FSMs;
using UnityEngine.AI;
using System.Collections;

//using UnityEngine.Animations;

// The script that controls an agent using an FSM
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionBehaviour : MonoBehaviour
{

    //public float floatStrength = 1f;
    //public float targetHeight = 1f;


    //public float damping = 9.99f;
    //public float floatSpeed = 1f;

    //public float minY = 1.6f;
    //public float maxY = 2.1f;
    public float speed = 0.3f;

    private float t = 0f;

    public Transform pointA;
    public Transform pointB;

    private Vector3 targetPosition;

    #region Variables
    public enum CompanionState { _idle, _follow, _rotate}
    
    public CompanionState _StateAI;  


    //[SerializeField]private GameObject _EPI; // Enemy presence Image
    //private mini _MiniMapCollor;

    internal NavMeshAgent Companion;

    // NEW CODE
    //[SerializeField]
    //private float followsRadius = 2f;

    private PlayerMovement player;

    private Transform Target;
    public Transform playerTarget => Target;

    [SerializeField] private Transform floatPos; 

    private CompanionSpawn point;

    private MeshRenderer CompanionMesh;

    [SerializeField] private Material AlphaLow, normal;




    //[HideInInspector] public bool _playerIsMoving;

    //private bool _StartFollow;

    //private PlayerMovement _Player;
    //[Header("Mesh Configuration")]

    //[SerializeField] private MeshRenderer CompanionMesh;
    //[SerializeField] Material normal, AlphaLow;

    //[SerializeField] private Transform AlphaPoint;

    //private float minDist = 0.4f;
    // Reference to the state machine
    private StateMachine stateMachine;

    //private bool _enemyIS;
    //public bool canSee => _enemyIS;

    private bool _gameplay; 
    public bool gameplay => _gameplay;  

    protected private GameState _gameState;

    //[Range(10, 150)]
    //public float radius;
    //[Range(50, 360)]
    //public float angle;

    //public LayerMask targetMask;
    //public LayerMask obstructionMask;
    //[SerializeField] private Transform FOV;
    //public Transform EEFOV => FOV; // Enemy Editor FOV

    [SerializeField] private LayerMask _attackLayers;

     private Camera mainCamera;

    #endregion

    //public float floatingHeight = 0.5f;
    //public float floatingSpeed = 1.0f;

    private Rigidbody _rb;



    //private Vector3 startingPosition;
    //public float floatingMagnitude = 0.5f;

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


        _StateAI = CompanionState._idle;

        Companion = GetComponent<NavMeshAgent>();

        //_rb = GetComponent<Rigidbody>();

        Companion.angularSpeed = 0;

        CompanionMesh = GetComponent<MeshRenderer>();   

        mainCamera = FindObjectOfType<Camera>();

        point = FindObjectOfType<CompanionSpawn>();

        Target = point.transform;


        player = FindObjectOfType<PlayerMovement>();

        targetPosition = transform.position;


        //startingPosition = transform.position;

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

        //CompanionMesh = GetComponent<MeshRenderer>();
        //_MiniMapCollor = FindObjectOfType<mini>();
        //mainCamera = Camera.main;
        //_playerIsMoving = false;

       
        // Create the states

        State IdleState = new State("Companion Idle",Idle);

        State FollowState = new State(("Companion Follow"), Follow); 
            

        /*
        State RotateState = new State("",
            () => Debug.Log(""),
            RotateAroundPlayer,
            () => Debug.Log(""));

        */

        // Add the transitions


        // Idle -> Follow
        IdleState.AddTransition(
            new Transition(
                () => _StateAI == CompanionState._follow,
                //() => Debug.Log("Idle -> Follow"),
                FollowState));

        // Follow -> Idle
        FollowState.AddTransition(
           new Transition(
               () => _StateAI == CompanionState._idle,
               //() => Debug.Log("Follow -> Idle"),
               IdleState));

        // Create the state machine
        stateMachine = new StateMachine(IdleState);
    }

    #endregion

    private void FixedUpdate()
    {
        if (_gameplay) //&& _StateAI == CompanionState._idle) 
        {

            //_rb.AddForce(Vector3.up * floatStrength);
            //_rb.AddForce(-_rb.velocity * damping);
            //Vector3 upForce = Vector3.up * _rb.mass * Physics.gravity.magnitude * (floatingHeight / 2 - transform.position.y);

            //_rb.AddForce(upForce, ForceMode.Acceleration);
            //float distance = targetHeight - transform.position.y;
            //_rb.AddForce(Vector3.up * distance * floatStrength);
        }
        else
        {
            //_rb.Sleep();
        }
    }

    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        if(_gameplay) 
        {
            ResumeAgent();
            
            //StartCoroutine(FOVRoutine());

            //CheckDist(); 

            Aim();
            
            // print(Companion.velocity +" Companion Velocity");
            //print(Companion.speed + " Companion Speed");
            //print(Companion.acceleration + " Companion Acceleration");

            CheckMoveBool();
            //CheckEnemy();
            //AlphaUpdate();
            //RotateTimer();

            Action actions = stateMachine.Update();
            actions?.Invoke();

        }
        else if(!_gameplay)
        {
            AgentPause();
            return;
        }
    }
    #endregion

    private void CheckDist()
    {
        if ((Target.position - transform.position).magnitude >= 1f)
        {
            //_StateAI = CompanionState._follow;
            return; 
        }
        else
        {
            //_StateAI = CompanionState._idle;
            return; 
        }
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
            //companionAim.transform.position = hitInfo.point;
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
        
        if (player.IsMoving && (Target.position - transform.position).magnitude >= 1f)
        {
            //_StartFollow = true;
            _StateAI = CompanionState._follow;
            return;
        }
        else if(!player.IsMoving && (Target.position - transform.position).magnitude <= 0.8f)
        {
            // _StartFollow = false;
            _StateAI = CompanionState._idle;
            return; 
        }
        
    }

    #region Idle State
    // Chase the small enemy
    private void Idle()
    {


        //Companion.updatePosition = false;
        StartCoroutine((floatStart()));

        FloatCompanion();

        //float floatingOffset = Mathf.Sin(Time.time * floatingSpeed) * floatingMagnitude;
        //Vector3 newPosition = startingPosition + new Vector3(0f, floatingOffset, 0f);
        //transform.position = newPosition;

    }


    private IEnumerator floatStart()
    {
        Companion.velocity = Vector3.zero;

        Companion.isStopped = true;

        yield return new WaitForSeconds(2f);
        
    }

    private void FloatCompanion()
    {
        //float newY = UnityEngine.Random.Range(minY, maxY);
        //Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);
        //transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);


        t += Time.deltaTime * speed;
        float newY = Mathf.SmoothStep(pointA.position.y, pointB.position.y, t);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);


        if (t >= 1f)
        {
            t = 0f;
            Transform temp = pointA;
            pointA = pointB;
            pointB = temp;
        }


        /*
        if (Vector3.Distance(transform.position, floatPos.position) < 0.1f)
        {
            float newY = UnityEngine.Random.Range(minY, maxY);
            floatPos.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        transform.position = Vector3.Lerp(transform.position, floatPos.position, speed * Time.deltaTime);

        //Vector3 pos = transform.position;
        //pos.y = Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
        //transform.position = pos;

        //transform.position = new Vector3(transform.position.x, startingPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength, transform.position.z);
        */
    }
    /*
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
    */

    private void PosUpdate()
    {
        //FloatCompanion();

        //Companion.updatePosition = true;
        Companion.isStopped = false;



        Companion.SetDestination(Target.position); 
        //Companion.speed = 3.4f;

        Companion.speed = 8f;
        Companion.acceleration = 10f; 


        if ((Target.position - transform.position).magnitude <= 2f)
        {
            Companion.speed = 3.5f;
        }
       
        else if ((Target.position - transform.position).magnitude <= 0.8f)
        {
            SlowDown();
            return;
        }
        else if ((Target.position - transform.position).magnitude >= 7f)
        {
            StartCoroutine(CatchPlayer());
            return;
        }

    }

    #endregion

    #region Follow State
    private void Follow()
    {
        // Companion.Warp(transform.position); 
        //FloatCompanion();
        //StopCoroutine(floatStart());

        PosUpdate();
       // StartCoroutine(FollowPlayer());
    }


    private IEnumerator FollowPlayer()
    {

        //yield return null;

        PosUpdate();


        // add reference to player navmesh agent

        /* CODE EXAMPLE 
         * NavMeshAgent playerAgent = Player.GetComponentInChildren<NavMeshAgent>();
            Vector3 playerDestination = playerAgent.destination;
            Vector3 positionOffset = FollowRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value),
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)
        ).normalized;

        Agent.SetDestination(playerDestination + positionOffset);

        */

        //yield return null;

        yield return new WaitUntil(() => Companion.remainingDistance <= Companion.stoppingDistance);
    }


    private IEnumerator CatchPlayer()
    {
        Setlow();
        transform.position = Target.position;
        yield return new WaitForSeconds(0.3f);
        SetHigh();
        yield return new WaitForSeconds(0.2f);
        Setlow();
        yield return new WaitForSeconds(0.3f);
        SetHigh();
    }


    private void SlowDown()
    {
        //Companion.acceleration = 8F;
        //Companion.speed = 3F;
        //Companion.velocity = Companion.velocity.normalized * 4f;

        Companion.velocity = Companion.velocity.normalized / 1.4f;
    }

    #endregion

    #endregion



    #region Enemy detection



    private void CheckEnemy()
    {

        //_EPI.SetActive(true);
        //_MiniMapCollor.SetCollorRed();

        //_EPI.SetActive(false);
        //_MiniMapCollor.SetCollorDefault();
        
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
            return;
        }
        else 
        {
            Companion.isStopped = false;
            return;
        }
    }

    private void AgentPause()
    {
        Companion.isStopped = true;
        Companion.velocity = Vector3.zero;
        return;
    }
    #endregion

    #region Destroy

    public void Replace()
    {
        Destroy(gameObject); 
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    #endregion
}
