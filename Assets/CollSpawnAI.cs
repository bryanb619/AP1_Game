using System.Collections;
using UnityEngine;

public class CollSpawnAI : MonoBehaviour
{
    [SerializeField]    private GameObject              _chase, _ranged;

    [SerializeField]    private GameObject              _spawnEffect;

    [SerializeField]    private int                     _chaseCount, _rangedCount;

    [SerializeField]    private float                   _dropRadius; 

                        private BoxCollider             _collider;

    [SerializeField]    private Transform               _transform_a, _transform_b; 


    // Start is called before the first frame update
    void Start()
    {
        _collider                   = GetComponent<BoxCollider>();
        _collider.enabled           = true;
        _collider.isTrigger         = true;
    }


    private void OnTriggerEnter(Collider other)
    {
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

            for (i = 0; i < _chaseCount; i++)
            {

                Vector3 spawnPosition = _transform_a.position +
                    new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius), 0f,
                    UnityEngine.Random.Range(-_dropRadius, _dropRadius));


                Instantiate(_spawnEffect, spawnPosition, _spawnEffect.transform.rotation);

                yield return new WaitForSeconds(0.4f);
                Instantiate(_chase, spawnPosition, transform.rotation);


                //print(i);
            }

            Destroy(gameObject);
            running = false;

           
        }
    }


}
