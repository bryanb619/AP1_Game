using System.Collections;
using UnityEngine;
using UnityEngine.AI; 


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement"), SerializeField]
                     internal float walkSpeed;
    [SerializeField] internal float dashSpeed, dashSpeedChangeFactor, groundDrag, moveSpeed, maxSpeed;

    [Header("Ground Check"), SerializeField]
                     private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    
    [SerializeField] private Transform orientation;
                     private MovementState state;

    [SerializeField] private LayerMask aiLayer;


    [Header("Slope Handling")]
    [HideInInspector]public bool CanMove; 
                     private float maxSlopeAngle;
                     private RaycastHit slopeHit;
                     private bool exitingSlope;

                     private bool grounded;
                     private float horizontalInput, verticalInput;
                     Vector3 moveDirection;
                     Rigidbody rb;
                     public bool dashing;


                     private bool PlayerOnMove;
                     public bool _PlayerOnMove => PlayerOnMove;
                     private CompanionBehaviour _CompanionMovement;
                     private EnemyBehaviour enemyHealth;
                     
                     private RestartMenu restartMenu;
                     
                     private float speed;
                     
                     // player health
                     

    

    [Header("Dash Explosion"), SerializeField]
                     private float explosionForce = 10f;
    //[SerializeField] private float explosionDamage = 20f;  
                     

    [Header("References"), SerializeField]
                     private MeshRenderer playerMaterial;
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private GameObject effect;
    [SerializeField] private LayerMask SeeThroughLayer; 
    [SerializeField] private GameObject playerMesh;

                     private SkinnedMeshRenderer[] skinnedMeshRenderers;
                     private Color[] originalColors;
                     private bool _gamePlay;

                     private NavMeshPath path;
                     //private float elapsed = 0.0f;
                     
                     internal NavMeshAgent agent;
                     
                     private LayerMask _layerMask;
                     
                     public float acceleration = 2f;
                     public float deceleration = 60f;
                     
                     public float closeEnoughMeters = 3f;
                     
                     private Vector3 currentDest;
                     
                     private bool _isMoving; 
                     public bool IsMoving => _isMoving;

    private enum MovementState
    {
        walking,
        dashing,
        air
    }

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
        {
            case GameState.Gameplay:
                {
                    _gamePlay = true;
                    break;
                }
            case GameState.Paused:
                {
                    _gamePlay = false;
                    break;
                }
        }

        //throw new NotImplementedException();
    }



    private void Start()
    {
        this.enabled = true;

        // Make a list of all SkinnedMeshRender in the character
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Create an array to store the original colors
        originalColors = new Color[skinnedMeshRenderers.Length];
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            originalColors[i] = skinnedMeshRenderers[i].material.color;
        }

        CanMove = true;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        agent = GetComponent<NavMeshAgent>();

        _CompanionMovement = FindObjectOfType<CompanionBehaviour>();
        restartMenu = FindObjectOfType<RestartMenu>();
        

        path = new NavMeshPath();
        //elapsed = 0.0f;

        currentDest = transform.position;

    }

    private void Update()
    {    

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        EnemiesAround();

        if (state == MovementState.walking /*|| state == MovementState.crouching*/)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }


    private void FixedUpdate()
    {
        
        MovePlayer();
        PlayerSpeed();     
    }

    private float desiredMoveSpeed, lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < maxSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = maxSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)   keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

        

    }

    private float speedChangeFactor;
    private Vector3 lastPosition;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }


    private void MyInput()
    {
        bool canMoveL = false;

        if(_gamePlay)
        {
            //agent.isStopped = false;
           
            if (canMoveL)
            {
                if(Input.GetMouseButtonDown(1)) 
                {
                   // agent.isStopped = false;
                    RaycastHit hit;

                    // Something other than the world was hit!
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, ~SeeThroughLayer))
                    //if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100, NavMesh.GetAreaFromName("Walkable")))
                    {
                        agent.angularSpeed = 0f;

                        Instantiate(effect, hit.point, Quaternion.identity);

                        if (hit.transform.CompareTag("Walk"))
                        {
                            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(hit.point), 2f);
                            //agent.destination = hit.point;
                            if (agent.enabled) 
                            {

                                //transform.LookAt(new Vector3(hit.point.x, 0, hit.point.z));
                                
                                agent.SetDestination(hit.point);


                                if (!agent.pathPending &&  agent.remainingDistance <= agent.stoppingDistance) 
                                {
                                    
                                    agent.velocity = Vector3.zero;
                                    agent.isStopped = true;
                                    return;

                                }
                                else
                                {
                                    agent.isStopped = false;
                                    return; 
                                }


                                //agent.isStopped = true;

                                
                               /* if (agent.remainingDistance <= 0.1f)
                                {
                                    agent.isStopped = true;
                                }

                                */
                            }
                            
                        }
                        else
                        {
                            agent.isStopped = true; 
                            return;
                        }  
                        
                    }
                    return; 
                }
                return; 
            }
        }
        else if(!_gamePlay) 
        {
            agent.velocity = Vector3.zero;
            //agent.isStopped = true;
        }
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }


        if(state == MovementState.walking)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        { 
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }



    private void PlayerSpeed()
    {
        //speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        //lastPosition = transform.position;
        

        //if (speed >= 1.5f)
        if(agent.velocity.magnitude > 2f)
        {
           _isMoving = true;
           //print("walk");
            //_CompanionMovement._playerIsMoving = true;

        }
        else
        {
            _isMoving = false;
            //_CompanionMovement._playerIsMoving = false;
        }
        
    }

    void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == "Enemy" && state == MovementState.dashing)
        {
            Rigidbody enemyRB = other.gameObject.GetComponent<Rigidbody>();
            Debug.Log("Dash damage");

            enemyHealth = other.gameObject.GetComponent<EnemyBehaviour>();
            
            //direction and distance of the push
            Vector3 pushDirection = other.transform.position - transform.position;
            float pushDistance = pushDirection.magnitude;
            pushDirection.y = 0;
            pushDirection.Normalize();

            Debug.Log("Push Direction: " + pushDirection);
            rb.AddForce(-pushDirection * explosionForce, ForceMode.Impulse);
            enemyRB.AddForce(pushDirection * explosionForce * 0.05f, ForceMode.Impulse);

            //explosion damage
            //enemyHealth.TakeDamage((int)explosionDamage, WeaponType.Dash);
        }
    }

    private void EnemiesAround()
    {
        //Collider[] aiHits = Physics.OverlapSphere(transform.position, 30f, aiLayer);

    }


    #region Enumerators

    internal IEnumerator VisualFeedbackDamage()
    {

        //Change the material colors to red
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        // Change the material colors back to their original colors
        for (int j = 0; j < skinnedMeshRenderers.Length; j++)
        {
            skinnedMeshRenderers[j].material.color = originalColors[j];
        }
    }

    internal IEnumerator VisualFeedbackHeal()
    {
        for (int i = 0; i <= 2; i++)
        {
            // Set the material colors to green
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                skinnedMeshRenderer.material.color = Color.green;
            }

            yield return new WaitForSeconds(0.2f);

            // Change the material colors back to their original colors
            for (int j = 0; j < skinnedMeshRenderers.Length; j++)
            {
                skinnedMeshRenderers[j].material.color = originalColors[j];
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}