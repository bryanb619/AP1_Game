using System.Collections;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

public class CollSpawnAi : MonoBehaviour
{
    // Enemies --------------------------------------------------------------------------->
    [FormerlySerializedAs("_chase")]
    [Header("AI")]

    [SerializeField]    private GameObject              chase;

    [FormerlySerializedAs("_ranged")]
    [Header("AI")]

    [SerializeField]    private GameObject              ranged;

    [FormerlySerializedAs("_spawnEffect")] 
    [SerializeField]    private GameObject              spawnEffect;

    [FormerlySerializedAs("_chaseCount")] [
        SerializeField]    private int                     chaseCount;
    [FormerlySerializedAs("_rangedCount")] [
        SerializeField]    private int                     rangedCount;

    // Radius & Components ----------------------------------------------------------------->
    [FormerlySerializedAs("_dropRadius")]
    [Header("Components")]

    [SerializeField]    private float                   dropRadius; 

    [FormerlySerializedAs("_transform_a")] 
    [SerializeField]    private Transform               transformA;
    [FormerlySerializedAs("_transform_b")] 
    [SerializeField]    private Transform               transformB;

    [SerializeField]    private bool                    secondTransform; 

                        private BoxCollider             _collider;

    [FormerlySerializedAs("_useSound")] 
    [SerializeField]    private bool                    useSound; 

     
    [SerializeField]    private EventReference          soundAiSpawn;
    

    [FormerlySerializedAs("objectiveUIScript")] 
    [SerializeField] private ObjectiveUi                objectiveUiScript;

    // Time -------------------------------------------------------------------------------->
    [FormerlySerializedAs("_startWait")]
    [Header("Spawn")]

    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private bool                    startWait;
    [FormerlySerializedAs("_waitTime")]
    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private float                   waitTime;

    [FormerlySerializedAs("_spawnTime")]
    [Tooltip("spawn time out between each AI Spawn")]
    [SerializeField]    private float                   spawnTime;  


    // Start is called before the first frame update

    private void Awake()
    {
        _collider                   = GetComponent<BoxCollider>();
        _collider.enabled           = true;
        _collider.isTrigger         = true;

        objectiveUiScript.RecieveEnemyCountInfo(chaseCount, rangedCount);
    }
    

    private void OnTriggerEnter(Collider other)
    {
       

        PlayerMovement player       = other.GetComponent<PlayerMovement>();

        if (player != null ) 
        {
            objectiveUiScript.PassedThroughCollider();
            
            StartCoroutine(SpawnAi());
        }
    }


    private IEnumerator SpawnAi()
    {
        bool running        = false;

        if (!running)
        {
            running             = true;
            int i;

            _collider.enabled   = false;

            

            if( !useSound)
            {
                //PlaySound();
            }
            

            for (i = 0; i < chaseCount; i++)
            {
                if(startWait) 
                {
                    yield return new WaitForSeconds(waitTime);
                }
                Vector3 spawnPosition = transformA.position +
                        new Vector3(UnityEngine.Random.Range(-dropRadius, dropRadius), 0f,
                        UnityEngine.Random.Range(-dropRadius, dropRadius));


                Instantiate(spawnEffect, spawnPosition, spawnEffect.transform.rotation);
                

                yield return new WaitForSeconds(spawnTime);
                Instantiate(chase, spawnPosition, transform.rotation);
                
            }

            if (secondTransform)
            {
                for (i = 0; i < rangedCount; i++)
                {
                    if (startWait)
                    {
                        yield return new WaitForSeconds(waitTime);
                    }

                    Vector3 spawnPosition = transformB.position +
                                            new Vector3(UnityEngine.Random.Range(-dropRadius, dropRadius), 0f,
                                                UnityEngine.Random.Range(-dropRadius, dropRadius));


                    Instantiate(spawnEffect, spawnPosition, spawnEffect.transform.rotation);

                    yield return new WaitForSeconds(spawnTime);
                    Instantiate(ranged, spawnPosition, transform.rotation);
                    
                }
            }
            
            //Destroy(gameObject);
            Destroy(_collider);
            running = false;
            
        }
    }



}
