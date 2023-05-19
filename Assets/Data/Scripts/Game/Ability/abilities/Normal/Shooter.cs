using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    private Transform _firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material cleansedCrystal;

    private GameObject _firePrefab, _icePrefab, _thunderPrefab;
    internal WeaponType MagicType;

    [FormerlySerializedAs("_areaEffect")] [SerializeField] private GameObject areaEffect;

    [Header("Default Ability Prefabs")]
    [SerializeField] private GameObject defaultFirePrefab;
    [SerializeField] private GameObject defaultIcePrefab, defaultThunderPrefab, normalPrefab;

    [Header("Upgraded Ability Prefabs")]
    [SerializeField] private GameObject upgradedFirePrefab;
    [SerializeField] private GameObject upgradedIcePrefab, upgradedThunderPrefab;
                     internal bool WUpgraded = false, RUpgraded = false;

    [Header("Abilities options")]
    [SerializeField] private float areaAttackRadius = 5f;

    [SerializeField] private Ability normalTimer, fireTimer, iceTimer, thunderTimer;
    [SerializeField] private GameObject rAbilityTelegraph;

    internal bool NormalCooldown, FireCooldown, IceCooldown, ThunderCooldown = false;

    [Header("Script References")]
    private ManaManager _manaManager;
    [FormerlySerializedAs("objectiveUI")] [SerializeField] private ObjectiveUi objectiveUi;
    private ValuesTextsScript _valuesTexts;
    [SerializeField] private AbilityHolder targetedAttackAbilityHolder;

    private RaycastHit _hit;
    private Vector3 _enemyPosition;

    private bool _gameplay;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        _manaManager = GetComponent<ManaManager>();
    }
    private void Start()
    {
        MagicType = WeaponType.Fire;

        _firePoint = GameObject.Find("CompanionShootPos").transform;

        _valuesTexts = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();

        _firePrefab = defaultFirePrefab;
        _icePrefab = defaultIcePrefab;
        _thunderPrefab = defaultThunderPrefab;
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
        }

    }

    private void ShootInput()
    {
        if (_firePoint != null)
        {
            //When you press the key (telegraph of the ability)
            if (Input.GetKey(KeyCode.Mouse0))
            {
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                
            }
            else if (Input.GetKey(KeyCode.W))
            {
                
            }
            else if (Input.GetKey(KeyCode.R))
            {
                rAbilityTelegraph.SetActive(true);
            }

            //--------------------------------------
            //When you release the key

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Debug.Log("Shot auto");
                MagicType = WeaponType.Normal;
                Shoot();
            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                MagicType = WeaponType.Fire;
                Shoot();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                MagicType = WeaponType.Ice;
                Shoot();
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                MagicType = WeaponType.Thunder;
                rAbilityTelegraph.SetActive(false);
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        switch (MagicType)
        {
            case WeaponType.Normal: // input nº1
                {
                    //Cleanse Crystal
                    if (_hit.collider.name == "Crystal" && _hit.collider.GetComponent<Outline>().enabled == true)
                    {
                        if (Input.GetKeyUp(KeyCode.Mouse0))
                        {
                            _hit.collider.GetComponent<MeshRenderer>().material.Lerp(_hit.collider.GetComponent<MeshRenderer>().material, cleansedCrystal, 1f);
                            _hit.collider.GetComponent<Outline>().enabled = false;
                            objectiveUi.Passed();
                            _valuesTexts.GetCrystal();
                            
                            break;
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (!NormalCooldown && Input.GetKeyUp(KeyCode.Mouse0))
                        {
                            StartCoroutine(NormalAttackCooldown());
                            Instantiate(normalPrefab, _firePoint.position, _firePoint.rotation);
                        }
                        break;
                    }
                }
            case WeaponType.Fire: // input nº2 (Q)
                {
                    if(!FireCooldown)
                    { 
                        if (_manaManager.ManaCheck(MagicType) == true)
                        {
                            StartCoroutine(FireAttackCooldown());
                            Instantiate(_firePrefab, _firePoint.position, _firePoint.rotation);
                        }
                        break;
                    }
                    else
                        break;
                }
            case WeaponType.Ice: // input nº3 (W)
                {
                    if(!IceCooldown)
                        //Instantiate inside the TargetAttack function to avoid unecessary code
                        TargetAttack();
                    
                    break;
                }
            case WeaponType.Thunder: // input nº4 (R)
                {
                    if(!ThunderCooldown)
                    {
                        if (_manaManager.ManaCheck(MagicType) == true)
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
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 50f))
            if (_hit.collider.CompareTag("Enemy"))
            {
                //Debug.Log(_hit.collider.gameObject + " hightlight on");
                _hit.collider.gameObject.GetComponent<Outline>().enabled = true;
            }
                
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
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 100))
        {
            if (_hit.collider.CompareTag("Enemy"))
            {
                    if (_manaManager.ManaCheck(MagicType) == true)
                    {
                        StartCoroutine(IceAttackCooldown());
                        Instantiate(_icePrefab, _hit.collider.transform.position, _firePoint.rotation);
                        targetedAttackAbilityHolder.TargetedAttackCooldownUi();
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
                Instantiate(_thunderPrefab, hitCollider.transform.position, _firePoint.rotation);

                Instantiate(areaEffect, transform.position, Quaternion.identity);
            }
        }
        StartCoroutine(ThunderAttackCooldown());
    }
  
    internal void UpgradeChecker(int abilityNumber)
    {
        switch (abilityNumber)
        {
            
            case 1:
                _firePrefab = upgradedFirePrefab;
                break;
            
            case 2:
                _icePrefab = upgradedIcePrefab;
                WUpgraded = true;
                break;
            
            case 3:
                _thunderPrefab = upgradedThunderPrefab;
                RUpgraded = true;
                break;
            
            default:
                break;
        }
    }

    #region Enumerators

    IEnumerator NormalAttackCooldown()
    {
        NormalCooldown = true;
        yield return new WaitForSecondsRealtime(normalTimer.cooldownTime);
        NormalCooldown = false;
    }   
    IEnumerator FireAttackCooldown()
    {
        FireCooldown = true;
        yield return new WaitForSecondsRealtime(fireTimer.cooldownTime);
        FireCooldown = false;
    }

    IEnumerator IceAttackCooldown()
    {
        IceCooldown = true;
        yield return new WaitForSecondsRealtime(iceTimer.cooldownTime);
        IceCooldown = false;
    }

    IEnumerator ThunderAttackCooldown()
    {
        ThunderCooldown = true;
        yield return new WaitForSecondsRealtime(thunderTimer.cooldownTime);
        ThunderCooldown = false;
    }

    #endregion

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
