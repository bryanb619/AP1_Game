using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material cleansedCrystal;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private GameObject firePrefab, icePrefab, thunderPrefab;
    public WeaponType _magicType;

    [Header("Abilities options")]
    [SerializeField] private float areaAttackRadius = 5f;
    [SerializeField] private float normalTimer, fireTimer, iceTimer, thunderTimer;
    internal bool normalCooldown, fireCooldown, iceCooldown, thunderCooldown = false;

    [Header("Script References")]
    private ManaManager manaManager;
    [SerializeField] private ObjectiveUI objectiveUI;

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
            HoverHighlight();
            firePoint = GameObject.Find("CompanionShootPos").transform;
        }

    }

    private void ShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
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
        switch (_magicType)
        {
            case WeaponType.Normal: // input nº1
                {
                    //Cleanse Crystal
                    if (hit.collider.name == "Crystal" && hit.collider.GetComponent<Outline>().enabled == true)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            hit.collider.GetComponent<MeshRenderer>().material.Lerp(hit.collider.GetComponent<MeshRenderer>().material, cleansedCrystal, 1f);
                            hit.collider.GetComponent<Outline>().enabled = false;
                            objectiveUI.passedSecondObjective = true;
                            break;
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (!normalCooldown)
                        {
                            StartCoroutine(NormalAttackCooldown());
                            Instantiate(normalPrefab, firePoint.position, firePoint.rotation);
                        }
                        break;
                    }
                }
            case WeaponType.Fire: // input nº2 (Q)
                {
                    if(!fireCooldown)
                    { 
                        if (manaManager.ManaCheck(_magicType) == true)
                        {
                            StartCoroutine(FireAttackCooldown());
                            Instantiate(firePrefab, firePoint.position, firePoint.rotation);
                        }
                        break;
                    }
                    else
                        break;
                }
            case WeaponType.Ice: // input nº3 (W)
                {
                    if(!iceCooldown)
                        //Instantiate inside the TargetAttack function to avoid unecessary code
                        TargetAttack();
                    
                    break;
                }
            case WeaponType.Thunder: // input nº4 (R)
                {
                    if(!thunderCooldown)
                    {
                        if (manaManager.ManaCheck(_magicType) == true)
                        {
                            //Instantiate(thunderPrefab, firePoint.position, firePoint.rotation);
                            AreaAttack();
                        }
                            break;
                    }
                    else
                        break;
                }

            default:
                break;
        }

    }

    private void HoverHighlight()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
            if (hit.collider.CompareTag("Enemy"))
                hit.collider.gameObject.GetComponent<Outline>().enabled = true;
            else
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<Outline>().enabled = false;
                }
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
                        StartCoroutine(IceAttackCooldown());
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, areaAttackRadius);
        
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // Deal damage to the enemy
                Instantiate(thunderPrefab, hitCollider.transform.position, firePoint.rotation);
            }
        }
        StartCoroutine(ThunderAttackCooldown());
    }
    
    IEnumerator NormalAttackCooldown()
    {
        normalCooldown = true;
        yield return new WaitForSecondsRealtime(normalTimer);
        normalCooldown = false;
    }   
    IEnumerator FireAttackCooldown()
    {
        fireCooldown = true;
        yield return new WaitForSecondsRealtime(fireTimer);
        fireCooldown = false;
    }

    IEnumerator IceAttackCooldown()
    {
        iceCooldown = true;
        yield return new WaitForSecondsRealtime(iceTimer);
        iceCooldown = false;
    }

    IEnumerator ThunderAttackCooldown()
    {
        thunderCooldown = true;
        yield return new WaitForSecondsRealtime(thunderTimer);
        thunderCooldown = false;
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
