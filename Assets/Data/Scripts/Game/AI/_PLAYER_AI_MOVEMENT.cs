using UnityEngine;
using UnityEngine.AI;

public class _PLAYER_AI_MOVEMENT : MonoBehaviour
{

    private Camera                              mainCamera; 
    private NavMeshAgent                        agent; 
    //private bool _isMoving;
    //private float _turnSpeed = 10f; 
    [SerializeField]private LayerMask           seeThroughLayer;

    private float                               maxAngle = 30f;


    private float                               playerSpeed = 4f;

    private float                               playerAcceleration = 2000f; 

    private float                               _turnSpeed = 10f;

    private bool                                _isMoving; 

    [SerializeField] 
    private GameObject                          _clickEffect;

    private Vector3                             targetPosition;
    private Vector3                             direction; 

    float height = 0;

    private void Start()
    {
        agent                   = GetComponent<NavMeshAgent>();
        mainCamera              = FindObjectOfType<Camera>();

        agent.speed             = playerSpeed;
        agent.acceleration      = playerAcceleration;


        agent.updateRotation    = false;
        agent.angularSpeed      = 0;
        //agent.angularSpeed = 0f; 

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 150, ~seeThroughLayer))
            {
                if (hit.transform.CompareTag("Walk"))
                {
                    if (agent.enabled)
                    {
                        //agent.enabled = true;
                        //Instantiate(_clickEffect, hit.point, Quaternion.identity);
                        height = hit.point.y + 0.3f;

                        // check if the angle between the current direction and the new direction is greater than maxAngle

                        // set the new target position
                        Vector3 newDirection = (hit.point - transform.position).normalized;
                        float angle = Vector3.Angle(direction, newDirection);
                        

                        if (angle > maxAngle && agent.remainingDistance >= 0.3f)
                        {
                            agent.velocity = Vector3.zero;
                            //agent.isStopped = true;

                            agent.isStopped = false;

                            targetPosition = hit.point;
                            agent.SetDestination(hit.point);
                            
                            //_isMoving = false;

                        }

                        else
                        {
                            _isMoving = true;

                            agent.isStopped = false;
                            targetPosition = hit.point;
                            agent.SetDestination(hit.point);
                            
                        }

                     

                        direction = newDirection;

                        GameObject spawnedObject = Instantiate(_clickEffect, hit.point, Quaternion.identity);

                        spawnedObject.transform.position =
                            new Vector3(spawnedObject.transform.position.x, height, spawnedObject.transform.position.z);

                        /*
                        if (_isMoving)
                        {
                            targetPosition = hit.point;

                        }
                        else
                        {
                            targetPosition = hit.point;
                            _isMoving = true;

                        }
                     */

                    }
                }
            }
        }
        
        
        
        if (agent.enabled)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }

            if (_isMoving)
            {
                direction = targetPosition - transform.position;

                direction.y = 0;

                // Rotate player towards the target position
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            }
        }




    }
      
}
