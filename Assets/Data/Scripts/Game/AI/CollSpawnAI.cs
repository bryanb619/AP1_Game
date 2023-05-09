using System.Collections;
using UnityEngine;
using FMODUnity;

public class CollSpawnAI : MonoBehaviour
{
    // Enemies --------------------------------------------------------------------------->
    [Header("AI")]

    [SerializeField]    private GameObject              _chase, _ranged;

    [SerializeField]    private GameObject              _spawnEffect;

    [SerializeField]    private int                     _chaseCount, _rangedCount;

    // Radius & Components ----------------------------------------------------------------->
    [Header("Components")]

    [SerializeField]    private float                   _dropRadius; 

    [SerializeField]    private Transform               _transform_a, _transform_b;

                        private BoxCollider             _collider;

    [SerializeField]    private bool                    _useSound; 

    [SerializeField]    private EventReference          _soundAISpawn;

    [SerializeField] private ObjectiveUI                objectiveUIScript;

    // Time -------------------------------------------------------------------------------->
    [Header("Spawn")]

    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private bool                    _startWait;
    [Tooltip("Wait time to star spawn of AI")]
    [SerializeField]    private float                   _waitTime;

    [Tooltip("spawn time out between each AI Spawn")]
    [SerializeField]    private float                   _spawnTime;  


    // Start is called before the first frame update
    void Start()
    {
        _collider                   = GetComponent<BoxCollider>();
        _collider.enabled           = true;
        _collider.isTrigger         = true;

        objectiveUIScript.RecieveEnemyCountInfo(_chaseCount, _rangedCount);
    }


    private void OnTriggerEnter(Collider other)
    {
        objectiveUIScript.PassedThroughCollider();

        PlayerMovement PLAYER       = other.GetComponent<PlayerMovement>();

        if (PLAYER != null ) 
        {
            StartCoroutine(SpawnAI());
        }
    }


    private IEnumerator SpawnAI()
    {
        bool running    = false;

        if (!running)
        {
            
            running = true;

            _collider.enabled = false;

            int i;

            if( _useSound)
            {
                RuntimeManager.PlayOneShot(_soundAISpawn);
            }

            for (i = 0; i < _chaseCount; i++)
            {
                if(_startWait) 
                {
                    yield return new WaitForSeconds(_waitTime);
                }
                Vector3 spawnPosition = _transform_a.position +
                    new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius), 0f,
                    UnityEngine.Random.Range(-_dropRadius, _dropRadius));


                Instantiate(_spawnEffect, spawnPosition, _spawnEffect.transform.rotation);
                

                yield return new WaitForSeconds(_spawnTime);
                Instantiate(_chase, spawnPosition, transform.rotation);


                //print(i);
            }

            for (i = 0; i < _rangedCount; i++)
            {
                if (_startWait)
                {
                    yield return new WaitForSeconds(_waitTime);
                }

                Vector3 spawnPosition = _transform_a.position +
                    new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius), 0f,
                    UnityEngine.Random.Range(-_dropRadius, _dropRadius));


                Instantiate(_spawnEffect, spawnPosition, _spawnEffect.transform.rotation);

                yield return new WaitForSeconds(_spawnTime);
                Instantiate(_ranged, spawnPosition, transform.rotation);


                //print(i);
            }

            Destroy(gameObject);
            running = false;

           
        }
    }


}
