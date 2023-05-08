using UnityEngine;

public class WarningSystemAI : MonoBehaviour
{
    [Header("Warning AI zone Gizmo")]
    [SerializeField] 
    private bool _activateGizmo = true; 

    // Set this to the layer that the player is on
    [SerializeField] private LayerMask targetLayer;

    // Set this to the layer that the AI game objects are on
    [SerializeField] private LayerMask aiLayer;

    //  radius in which the AI can alert
    [Range(0, 30)][SerializeField] private float AiRadius = 20.0f;

    private EnemyChaseBehaviour enemy;
    private EnemyBehaviour enemy2;

    private enum EnemyType {enemyChase, enemyRanged}
    private EnemyType type;

    internal bool canAlertAI;

    private void Start()
    {
        if (TryGetComponent(out EnemyChaseBehaviour e))
        {
            type = EnemyType.enemyChase;

            enemy = GetComponent<EnemyChaseBehaviour>();

            //Debug.Log(type.ToString());
        }

        if (TryGetComponent(out EnemyBehaviour r))
        {
            type = EnemyType.enemyRanged;
            enemy2 = GetComponent<EnemyBehaviour>();
            //Debug.Log(type.ToString());
        }


    }

    private void Update()
    {
        if(canAlertAI) 
        {
            AlertAI();
        }

        if (type == EnemyType.enemyChase)
        {
            if (enemy.canSee)
            {
                canAlertAI = true;

                //Debug.Log("Can Alert Enemy Chase");
            }
          
        }
        if (type == EnemyType.enemyRanged)
        {
            if (enemy2.canSee)
            {
                canAlertAI = true;
            }
        }
    }


    private void AlertAI()
    {
        // Check if the player is within the warning radius
        Collider[] aiHits = Physics.OverlapSphere(transform.position, AiRadius, aiLayer);
        if (aiHits.Length > 0)
        {
                        // Iterate through the list of AI game objects and send a warning message
            foreach (Collider aiHit in aiHits)
            {
                aiHit.gameObject.SendMessage("OnPlayerWarning", transform.position, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(_activateGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,
            AiRadius);
        }
      
    }
}
