using UnityEngine;
using UnityEngine.AI;

public class AIPatrol : MonoBehaviour
{
    private NavMeshAgent _Agent;
    private EnemyBrain brain;

    [Header("Patrol Settings")]
    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;

    private void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
        brain= GetComponent<EnemyBrain>();
    }

    public void Action()
    {
        Patrol();
    }

    private void Patrol()
    {
        brain._returnPatrol = false;
        _Agent.autoBraking = false;
        _Agent.stoppingDistance = 0f;

        if (!_Agent.pathPending && _Agent.remainingDistance < 0.5f)
        {
            GotoNetPoint();
        }

    }

    private void GotoNetPoint()
    {
        _Agent.autoBraking = false;

        _Agent.speed = 1.4f;
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        _Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
    }

}
