using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/AI/Plug AI/Patrol")]
public class AIPatrol : Behaviours
{
    //private NavMeshAgent Agent;


    [Header("Patrol Settings")]
    private int destPoint = 0;
    [SerializeField] private Transform[] _PatrolPoints;



    public override void Act(Brain controller)
    {
        //throw new System.NotImplementedException();
    }

    private void Patrol(Brain controller)
    {

        ///controller.navMeshAgent.destination = controller.


    }


/*
    private void Patrol()
    {

        Agent.autoBraking = false;
        Agent.stoppingDistance = 0f;

        if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
        {
            GotoNetPoint();
        }
        
    }

    private void GotoNetPoint()
    {
        
        Agent.autoBraking = false;

        Agent.speed = 1.4f;
        // Returns if no points have been set up
        if (_PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        Agent.destination = _PatrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % _PatrolPoints.Length;
        
    }
*/
}
