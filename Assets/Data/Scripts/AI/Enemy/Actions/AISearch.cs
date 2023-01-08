using UnityEngine;
using UnityEngine.AI;

public class AISearch : AI
{
    private NavMeshAgent agent;

    private PredictionModel pathPrediction;

    [SerializeField]private GameObject Player; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        Player = GameObject.Find("Player");
    }

    public void Action()
    {
        Search();
    }

    private void Search()
    {


        if (agent.remainingDistance <= agent.stoppingDistance)
        {

            


            // Get the player's last position (their destination)
            Vector3 lastPosition = agent.destination;


            // * play animation ( Move FOV HEAD in Y rotation) and initiate again patrol state

            //Debug.Log("Player's last position: " + lastPosition);

            //GetPath();
            PathPredict();
        }

    }

    private void PathPredict()
    {
        //print("predicting");
        // Predict the target's path using the prediction model
        Vector3[] predictedPath = pathPrediction.PredictPath(Player);

        // Set the AI agent's destination to be the predicted position of the target at a certain point in the future
        agent.destination = predictedPath[predictedPath.Length - 1];
    }
}
