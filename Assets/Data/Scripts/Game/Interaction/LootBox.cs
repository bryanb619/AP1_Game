using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;


public class LootBox : MonoBehaviour
{
    [FormerlySerializedAs("_data")] [SerializeField]
    private LootBoxData         data;
    private BoxCollider         _collider;

    [FormerlySerializedAs("_spawnPos")] [SerializeField]
    private Transform           spawnPos;

    [FormerlySerializedAs("_effectPos")] [SerializeField]
    private Transform           effectPos;

    private GameObject          _loot;
    private GameObject          _openEffect;

    private int                 _maxLoot;
    private float               _dropRadius;

    private EventReference      _soundOpen;

    private bool                _giveImediate;




    [FormerlySerializedAs("_gizmos")] [SerializeField] private bool gizmos;
    [FormerlySerializedAs("_DEBUGRADIUS")] [SerializeField] private float debugradius = 2F; 



    // Start is called before the first frame update
    void Start()
    {
        _collider               = GetComponent<BoxCollider>();
        _collider.isTrigger     = true;

        _loot                   = data.Loot;
        _openEffect             = data.OpenEffect;
        _maxLoot                = data.MaxItems;
        _dropRadius             = data.DropRadius;
        _soundOpen              = data.SoundOpen;
        _giveImediate           = data.GiveLoot;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player!=null)
        {
            this._collider.enabled = false;

            OpenLoot();

        }

    }

    private void OpenLoot()
    {
        switch (_giveImediate)
        {
            case true:
                {
                    GieveLoot();
                    break;
                }
            case false:
                {
                    PickLoot(); 
                    break;
                }
        }

    }

    private void GieveLoot()
    {
        print("LOOT DELIVERED");
    }

    private void PickLoot()
    {
        if (_openEffect != null)
        {
            Instantiate(_openEffect, effectPos.position, Quaternion.identity);
        }

        RuntimeManager.PlayOneShot(_soundOpen, transform.position);


         for (int i = 0; i<_maxLoot; i++)
         {

                Vector3 spawnPosition = spawnPos.position + //transform.position +
                    new Vector3(Random.Range(-_dropRadius, _dropRadius), 0f,
                    Random.Range(-_dropRadius, _dropRadius));

                Instantiate(_loot, spawnPosition, Quaternion.identity);
         }
    }

    private void OnDrawGizmos()
    {
        if(gizmos)
        {
            Gizmos.DrawWireSphere(spawnPos.position, debugradius);
        }
         
    }

}
