using UnityEngine;

public class PredictionModel : MonoBehaviour
{
    // The speed at which the target is moving, in units per second
    public float targetSpeed;

    // The maximum distance that the target can move in one second
    public float maxMovementPerSecond;

    public Vector3[] PredictPath(GameObject target)
    {
        // Get the current position and velocity of the target
        Vector3 currentPosition = target.transform.position;
        Vector3 currentVelocity = target.GetComponent<Rigidbody>().velocity;

        // Estimate the position of the target one second in the future using the current velocity and a simple heuristic
        Vector3 predictedPosition = currentPosition + currentVelocity * targetSpeed + Vector3.ClampMagnitude(currentVelocity, maxMovementPerSecond);

        // Return the predicted path as an array with just one element (the predicted position)
        return new Vector3[] { predictedPosition };
    }
}
