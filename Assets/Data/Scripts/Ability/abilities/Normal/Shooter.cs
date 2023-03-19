using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform      firePoint;
    [SerializeField] private Camera         mainCamera;

    [Header("Prefabs")]
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private GameObject firePrefab, icePrefab, thunderPrefab;

    public WeaponType _magicType;
    
    [Header("Abilities options")]
    [SerializeField] private float areaAttackRadius = 5f;

    private ManaManager manaManager;

    private RaycastHit hit;
    private Vector3 enemyPosition;

    private bool _gameplay;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        manaManager = GetComponent<ManaManager>();
    }
    private void Start()
    {
        _magicType = WeaponType.Fire;
    }


    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state) 
        {
            case GameState.Paused:
                {
                    _gameplay = false;
                    break; 
                }
            case GameState.Gameplay:
                {
                    _gameplay = true;
                    break; 
                }
        }

    }

    private void Update()
    {
        if (_gameplay)
        {
            ShootInput();
        }
        
    }

    private void ShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _magicType = WeaponType.Normal;
            print("Ability is set to normal");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            _magicType = WeaponType.Fire;
            print("Ability is set to fire");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _magicType = WeaponType.Ice;
            print("Ability is set to Ice");
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _magicType = WeaponType.Thunder;
            print("Ability is set to Thunder");
            Shoot();
        }
    }

    public void Shoot()
    {
        switch(_magicType)
        {
            case WeaponType.Normal: // input nº1
                {                    
                    Instantiate(normalPrefab, firePoint.position, firePoint.rotation);
                    break; 
                }
            case WeaponType.Fire: // input nº2 (Q)
                {
                    if (manaManager.ManaCheck(_magicType) == true)
                    { 
                        Instantiate(firePrefab, firePoint.position, firePoint.rotation);
                        break;
                    }
                    else
                        break;
                }
            case WeaponType.Ice: // input nº3 (W)
                {
                    //Instantiate inside the TargetAttack function to avoid unecessary code
                    TargetAttack();
                    break;
                }
            case WeaponType.Thunder: // input nº4 (R)
                    {
                        if (manaManager.ManaCheck(_magicType) == true)
                        {
                            //Instantiate(thunderPrefab, firePoint.position, firePoint.rotation);
                            AreaAttack();
                            break;
                        }
                        else
                            break;
                }
            
            default: 
                break;
        }
        
    }

    private void TargetAttack()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (manaManager.ManaCheck(_magicType) == true)
                {
                    Instantiate(icePrefab, hit.collider.transform.position, firePoint.rotation);
                    Debug.Log("Enemy Hit with Ice");
                }
                else
                    Debug.Log("Not enough mana");
            }
        }
    }

    private void AreaAttack()
    {
/*  
        if (hit.collider.CompareTag("Enemy"))
        {
            Instantiate(icePrefab, hit.collider.transform.position, firePoint.rotation);
            Debug.Log("Enemy Hit with Ice");
        }
*/

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, areaAttackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // Deal damage to the enemy
                Instantiate(thunderPrefab, hitCollider.transform.position, firePoint.rotation);
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe sphere to show the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
    }

}
