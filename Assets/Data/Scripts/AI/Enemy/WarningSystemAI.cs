using UnityEngine;

public class WarningSystemAI : MonoBehaviour
{
    // Set this to the layer that the player is on
    [SerializeField] private LayerMask targetLayer;

    // Set this to the layer that the AI game objects are on
    [SerializeField] private LayerMask aiLayer;

    // Set this to the radius in which the AI game objects should be warned
    [Range(0,30)][SerializeField] private float radius = 20.0f;

    void Update()
    {
        // Check if the player is within the warning radius
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayer);
        if (hits.Length > 0)
        {
            // If the player is within the warning radius, get a list of all AI game objects within the radius
            Collider[] aiHits = Physics.OverlapSphere(transform.position, radius, aiLayer);

            // Iterate through the list of AI game objects and send a warning message
            foreach (Collider aiHit in aiHits)
            {
                aiHit.gameObject.SendMessage("OnPlayerWarning", transform.position, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,
        radius);
    }
}
