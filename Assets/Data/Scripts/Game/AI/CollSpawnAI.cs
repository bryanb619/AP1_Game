using System.Collections;
using UnityEngine;
using FMODUnity;


public class CollSpawnAI : MonoBehaviour
{
    // Enemies --------------------------------------------------------------------------->

    [Header("AI")]

    [SerializeField]    private GameObject              chase;


    [Header("AI")]

    [SerializeField]    private GameObject              ranged;


    [SerializeField]    private GameObject              spawnEffect;


        [SerializeField]    private int                     chaseCount;
        
        [SerializeField]    private int                     rangedCount;

    // Radius & Components ----------------------------------------------------------------->
    [Header("Components")]

    [SerializeField]    private float                   dropRadius; 
    
    [SerializeField]    private Transform               transformA;
    [SerializeField]    private Transform               transformB;

    [SerializeField]    private bool                    secondTransform; 

                        private BoxCollider             _collider;
                        
    [SerializeField]    private bool                    useSound; 

     
    [SerializeField]    private EventReference          soundAiSpawn;
    
    [SerializeField] private ObjectiveUi                objectiveUiScript;

    // Time -------------------------------------------------------------------------------->
    [Header("Spawn")]

    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private bool                    startWait;
    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private float                   waitTime;
    
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
                        new Vector3(Random.Range(-dropRadius, dropRadius), 0f,
                        Random.Range(-dropRadius, dropRadius));


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
                                            new Vector3(Random.Range(-dropRadius, dropRadius), 0f,
                                                Random.Range(-dropRadius, dropRadius));


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
