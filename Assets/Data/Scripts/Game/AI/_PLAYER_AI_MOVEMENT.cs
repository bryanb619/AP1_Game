using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _PLAYER_AI_MOVEMENT : MonoBehaviour
{

    private Camera mainCamera; 
    private NavMeshAgent agent; 
    private bool _isMoving;
    private float _turnSpeed = 10f; 
    [SerializeField]private LayerMask seeThroughLayer;

    private float maxAngle = 90f;


    private float playerSpeed = 3.5f; 

    [SerializeField] private GameObject _clickEffect;

    private Vector3 direction; 

    float height = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera= FindObjectOfType<Camera>();

        agent.speed = playerSpeed;
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

                        if (angle > maxAngle)
                        {
                            agent.velocity = Vector3.zero;
                            agent.isStopped = true;
                            agent.isStopped = false;
                            agent.SetDestination(hit.point);
                        }
                    
                        else
                        {
                            agent.isStopped = false;
                            agent.SetDestination(hit.point);
                        }

                        direction = newDirection;

                        GameObject spawnedObject = Instantiate(_clickEffect, hit.point, Quaternion.identity);

                        spawnedObject.transform.position =
                            new Vector3(spawnedObject.transform.position.x, height, spawnedObject.transform.position.z);


                        //agent.SetDestination(hit.point);


                        



                        //targetPosition = hit.point; 
                        /*
                        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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
                        */
                    }
                }
            }
        }
        if(agent.enabled) 
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
        }

        

        /*
        if (_isMoving)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance < agent.stoppingDistance)
            {
                agent.isStopped = true;
                _isMoving = false;
                return;
            }

            //Vector3 direction = targetPosition - transform.position;
            //direction.y = 0;

            // Smoothly rotate the player towards the target
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            // Move the player towards the target position
            agent.SetDestination(targetPosition);

            if (agent.pathPending)
            {
                return;
            }

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                agent.isStopped = false;
            }
            else
            {
                agent.isStopped = true;
            }
        }
        /*
        // Check if the NavMesh agent has a valid path
        if (agent.enabled && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            // Calculate the direction and distance to the target
            Vector3 direction = agent.steeringTarget - transform.position;
            float distance = direction.magnitude;

            // If the new path is more than 150 degrees in the other direction, stop the agent
            float angle = Vector3.Angle(transform.forward, direction);
            if (angle > 150f)
            {
                agent.isStopped = true;
                return;
            }

            // Move the player using NavMesh
            agent.isStopped = false;
            agent.SetDestination(agent.steeringTarget);

            // Smoothly rotate the player towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            // Stop moving if the player is close enough to the target position
            if (distance < agent.stoppingDistance)
            {
                agent.isStopped = true;
                _isMoving = false;
                return;
            }
        }
        */
    }
}
