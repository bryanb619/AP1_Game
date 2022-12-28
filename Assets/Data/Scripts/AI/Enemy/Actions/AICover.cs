using UnityEngine;
using UnityEngine.AI;

public class AICover : MonoBehaviour
{// The Transform of the player
    public Transform playerTransform;

    // The speed at which the AI character moves
    public float moveSpeed = 5f;

    // The distance at which the AI character starts fleeing from the player
    public float fleeDistance = 10f;

    // The current cover position the AI character is using
    private Transform currentCoverPosition;

    // The transform of the AI character
    private Transform aiTransform;

    // The NavMeshAgent component of the AI character
    private NavMeshAgent agent;

    private void Start()
    {
        // Get the transform of the AI character
        aiTransform = transform;

        // Get the NavMeshAgent component of the AI character
        agent = GetComponent<NavMeshAgent>();
    }

    public void Action()
    {
        CoverAction();
    }

    private void CoverAction()
    {
        // If the player is within the flee distance, flee from the player
        if (Vector3.Distance(aiTransform.position, playerTransform.position) <= fleeDistance)
        {
            FleeFromPlayer();
        }
        // Otherwise, use cover
        else
        {
            UseCover();
        }
    }
    void FleeFromPlayer()
    {
        // Calculate the direction to flee in
        Vector3 fleeDirection = aiTransform.position - playerTransform.position;

        // Normalize the direction
        fleeDirection.Normalize();

        // Set the destination for the NavMeshAgent to the position to flee to
        agent.destination = aiTransform.position + fleeDirection * moveSpeed * Time.deltaTime;
    }

    void UseCover()
    {
        // If we don't have a current cover position, or if we've reached the current cover position, choose a new one
        if (currentCoverPosition == null || Vector3.Distance(aiTransform.position, currentCoverPosition.position) <= 1f)
        {
            // Find all cover positions in the scene
            GameObject[] coverPositions = GameObject.FindGameObjectsWithTag("Cover");

            // Choose a random cover position
            int coverPositionIndex = Random.Range(0, coverPositions.Length);
            currentCoverPosition = coverPositions[coverPositionIndex].transform;
        }

        // Set the destination for the NavMeshAgent to the current cover position
        agent.destination = currentCoverPosition.position;
    }
}
