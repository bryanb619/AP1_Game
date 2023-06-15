using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class LootBox : MonoBehaviour
{ [SerializeField]
    private LootBoxData         data;
    private BoxCollider         _collider;

  [SerializeField]
    private Transform           spawnPos;

 [SerializeField]
    private Transform           effectPos;

    private DropType            _dropType; 

    private GameObject          _loot;
    private GameObject          _openEffect;

    private int                 _maxLoot;
    private float               _dropRadius;

    private EventReference      _soundOpen;

    private bool                _giveImediate;
    private int                 _empower; 
    
    // player
    private PlayerHealth _playerHealth;
    private ManaManager _playerMana;


#if UNITY_EDITOR
    [SerializeField] private bool gizmos;
[SerializeField] private float debugradius = 2F;
    
#endif



private void Awake()
{
    _collider               = GetComponent<BoxCollider>();
    _collider.isTrigger     = true;
    
    _dropType              = data.DropType;

    _loot                   = data.Loot;
    _openEffect             = data.OpenEffect;
    _maxLoot                = data.MaxItems;
    _dropRadius             = data.DropRadius;
    _soundOpen              = data.SoundOpen;
    
    _giveImediate           = data.GiveLoot;
    _empower               = data.Empower;
    
}


// Start is called before the first frame update
    private void Start()
    {
        if (_dropType == DropType.SpecialHealth)
        {
            _playerHealth = FindObjectOfType<PlayerHealth>(); 
        }
        
        if(_dropType == DropType.SpecialMana)
        {
            _playerMana = FindObjectOfType<ManaManager>();
            
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        
        
        if (player!=null)
        {
            _collider.enabled = false;

            OpenLoot();

        }

    }

    private void OpenLoot()
    {
        switch (_giveImediate)
        {
            case true:
                {
                    GiveLoot(); 
                    break;
                }
            case false:
                {
                    PickLoot(); 
                    break;
                }
        }

    }

    private void GiveLoot()
    {
        print("LOOT DELIVERED");

        switch (_dropType)
        {
            case DropType.SpecialHealth:
            {
                _playerHealth.EmpowerHealth(_empower);
                
                break;
            }
            case DropType.SpecialMana:
            {
                _playerMana.ManaIncrease(_empower);
                
                break;
            }
            
        }
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

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(gizmos)
        {
            Gizmos.DrawWireSphere(spawnPos.position, debugradius);
        }
         
    }
#endif


}
