using UnityEngine;
using UnityEngine.AI;

public class _PLAYER_AI_MOVEMENT : MonoBehaviour
{
    // Components ----------------------------------------------------->
                        private Camera              mainCamera; 
                        private NavMeshAgent        agent; 

    // Player Movement ------------------------------------------------>
    [SerializeField]    private LayerMask           _ignoreLayer;
      
                        private float               maxAngle = 30f;
                        private float               playerSpeed = 4f;
    [SerializeField]    private float               playerAcceleration = 2000f;
    [SerializeField]    private float               _turnSpeed;
                        private bool                _isMoving;

    // Click ------------------------------------------------------------>
                        private enum EffectState    { _CLICKED, _UNCLICKED }
                        private EffectState         _cursorState;

    [SerializeField]    private GameObject          _clickEffect;

                        float                       height = 0;
    [SerializeField]    private float               heightOffset;

    // target & direction ------------------------------------------------->
                        private Vector3             targetPosition;
                        private Vector3             direction; 

    // Enemy detection ----------------------------------------------------->
    [SerializeField]    private float               _maxRange = 20f; 

    private void Start()
    {
        Components();
        PlayerProfile();
    }

    private void Components()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = FindObjectOfType<Camera>();
    }

    private void PlayerProfile()
    {
        agent.speed = playerSpeed;
        agent.acceleration = playerAcceleration;


        agent.updateRotation = false;
        agent.angularSpeed = 0;

        _cursorState = EffectState._UNCLICKED;

    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
        newDirection();
    }

    private void MoveInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destination(true);
        }

        else if (Input.GetMouseButton(1))
        {
            _cursorState = EffectState._CLICKED;
        }
        else
        {
            _cursorState = EffectState._UNCLICKED;
        }
    }

    private void newDirection()
    {
        if (agent.enabled)
        {
            if (_isMoving)
            {
                // set direction
                direction = targetPosition - transform.position;

                // Ignore Y position 
                direction.y = 0;

                // Rotate player towards the target position
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // apply rotation with desired speed
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            }


            if (_cursorState == EffectState._CLICKED)
            {
                Destination(false);
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }

        }
    }

    private void Destination(bool input)
    {
        RaycastHit hit;

        Vector3 destination;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 50, ~_ignoreLayer))
        {
            if (agent.enabled)
            {
                Vector3 newDirection = (hit.point - transform.position).normalized;
                float angle = Vector3.Angle(direction, newDirection);

                destination = hit.point;

                direction = newDirection;

                NavMeshHit navHit;

                if (NavMesh.SamplePosition(destination, out navHit, _maxRange, NavMesh.AllAreas))
                {

                    if (angle > maxAngle && agent.remainingDistance >= 0.2f)
                    {
                        agent.velocity = Vector3.zero;

                        targetPosition = navHit.position;

                        //_isMoving = false;
                    }


                    else
                    {
                        _isMoving = true;
                        agent.isStopped = false;
                        targetPosition = (navHit.position);

                        agent.SetDestination(navHit.position);
                    }


                }

                if (input)
                {
                    EffectSpawn(hit);
                }
            }
            /*
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 50, ~seeThroughLayer))
            {
                if (hit.transform.CompareTag("Walk"))
                {
                    if (agent.enabled)
                    {
                        //  target position
                        Vector3 newDirection                    = (hit.point - transform.position).normalized;
                        float angle                             = Vector3.Angle(direction, newDirection);

                        direction                               = newDirection;


                        if (angle > maxAngle && agent.remainingDistance >= 0.2f)
                        {
                            agent.velocity                      = Vector3.zero;

                            targetPosition                      = hit.point;

                            agent.SetDestination(hit.point);

                            //_isMoving = false;
                        }

                        else
                        {
                            _isMoving                           = true;
                            agent.isStopped                     = false;
                            targetPosition                      = hit.point;

                            agent.SetDestination(hit.point);
                        }

                        if(input) 
                        {
                            height                              = hit.point.y + 0.3f;

                            GameObject spawnedObject            = Instantiate(_clickEffect, hit.point, Quaternion.identity);
                            spawnedObject.transform.position    = new Vector3
                                (spawnedObject.transform.position.x, 
                                height, 
                                spawnedObject.transform.position.z);
                        }

                    }
                }

            } */
        }
    }

    private void EffectSpawn(RaycastHit hit)
    {
        height = hit.point.y + heightOffset;

        GameObject spawnedObject = Instantiate(_clickEffect, hit.point, _clickEffect.transform.rotation);
        spawnedObject.transform.position = new Vector3
            (spawnedObject.transform.position.x,
            height,
            spawnedObject.transform.position.z);
    }
}

     
