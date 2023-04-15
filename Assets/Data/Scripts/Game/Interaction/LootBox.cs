using FMODUnity;
using UnityEngine;


public class LootBox : MonoBehaviour
{
    [SerializeField]
    private LootBoxData         _data;
    private BoxCollider         _collider;

    [SerializeField]
    private Transform           _spawnPos, _effectPos;

    private GameObject          _loot;
    private GameObject          _openEffect;

    private int                 _maxLoot;
    private float               _dropRadius;

    private EventReference      _soundOpen;

    private bool                _giveImediate;




    [SerializeField] private bool _gizmos;
    [SerializeField] private float _DEBUGRADIUS = 2F; 



    // Start is called before the first frame update
    void Start()
    {
        _collider               = GetComponent<BoxCollider>();
        _collider.isTrigger     = true;

        _loot                   = _data.Loot;
        _openEffect             = _data.OpenEffect;
        _maxLoot                = _data.MaxItems;
        _dropRadius             = _data.DropRadius;
        _soundOpen              = _data.SoundOpen;
        _giveImediate           = _data.GiveLoot;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player!=null)
        {
            this._collider.enabled = false;

            openLoot();

        }

    }

    private void openLoot()
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
            Instantiate(_openEffect, _effectPos.position, Quaternion.identity);
        }

        RuntimeManager.PlayOneShot(_soundOpen, transform.position);


         for (int i = 0; i<_maxLoot; i++)
         {

                Vector3 spawnPosition = _spawnPos.position + //transform.position +
                    new Vector3(Random.Range(-_dropRadius, _dropRadius), 0f,
                    Random.Range(-_dropRadius, _dropRadius));

                Instantiate(_loot, spawnPosition, Quaternion.identity);
         }
    }

    private void OnDrawGizmos()
    {
        if(_gizmos)
        {
            Gizmos.DrawWireSphere(_spawnPos.position, _DEBUGRADIUS);
        }
         
    }

}
