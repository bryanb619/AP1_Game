using UnityEngine;
using UnityEngine.Serialization;

public class WarningSystemAi : MonoBehaviour
{
    [FormerlySerializedAs("_activateGizmo")]
    [Header("Warning AI zone Gizmo")]
    [SerializeField] 
    private bool activateGizmo = true; 

    // Set this to the layer that the player is on
    [SerializeField] private LayerMask targetLayer;

    // Set this to the layer that the AI game objects are on
    [SerializeField] private LayerMask aiLayer;

    //  radius in which the AI can alert
    [FormerlySerializedAs("AiRadius")] [Range(0, 30)][SerializeField] private float aiRadius = 20.0f;

    private EnemyChaseBehaviour _enemy;
    private EnemyBehaviour _enemy2;

    private enum EnemyType {EnemyChase, EnemyRanged}
    private EnemyType _type;

    internal bool CanAlertAi;

    private void Start()
    {
        if (TryGetComponent(out EnemyChaseBehaviour e))
        {
            _type = EnemyType.EnemyChase;

            _enemy = GetComponent<EnemyChaseBehaviour>();

            //Debug.Log(type.ToString());
        }

        if (TryGetComponent(out EnemyBehaviour r))
        {
            _type = EnemyType.EnemyRanged;
            _enemy2 = GetComponent<EnemyBehaviour>();
            //Debug.Log(type.ToString());
        }


    }

    private void Update()
    {
        if(CanAlertAi) 
        {
            AlertAi();
        }

        if (_type == EnemyType.EnemyChase)
        {
            if (_enemy.CanSee)
            {
                CanAlertAi = true;

                //Debug.Log("Can Alert Enemy Chase");
            }
          
        }
        
        /*
         if (_type == EnemyType.EnemyRanged)
        {
            if (_enemy2.CanSee)
            {
                CanAlertAi = true;
            }
        }
        */
    }


    private void AlertAi()
    {

        bool isRunning = false;

        

        if(!isRunning) 
        {
            
            //print("WARNING");

            // Check if the player is within the warning radius
            Collider[] aiHits = Physics.OverlapSphere(transform.position, aiRadius, aiLayer);
            if (aiHits.Length > 0)
            {
                // Iterate through the list of AI game objects and send a warning message
                foreach (Collider aiHit in aiHits)
                {
                    aiHit.gameObject.SendMessage("OnPlayerWarning", transform.position, SendMessageOptions.DontRequireReceiver);
                }
            }
            isRunning = true;
        }
        
        
    }

    private void OnDrawGizmos()
    {
        if(activateGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,
            aiRadius);
        }
      
    }
}
