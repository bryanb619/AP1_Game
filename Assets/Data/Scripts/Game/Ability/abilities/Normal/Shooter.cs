using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.AI;
using FMODUnity;
using UnityEngine.VFX;


public class Shooter : MonoBehaviour
{
    [Header("References")] private Transform _firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material cleansedCrystal;
    [SerializeField] private Color lightColor;
    [SerializeField] private EventReference cleanCrystalSound;
    [SerializeField] private VisualEffect wVisualEffect;

    private GameObject _firePrefab, _icePrefab, _thunderPrefab;
    internal WeaponType MagicType;

    [SerializeField] private GameObject areaEffect;

    [Header("Default Ability Prefabs")] [SerializeField]
    private GameObject defaultFirePrefab;

    [SerializeField] private GameObject defaultIcePrefab, defaultThunderPrefab, normalPrefab;

    [Header("Upgraded Ability Prefabs")] [SerializeField]
    private GameObject upgradedFirePrefab;

    [SerializeField] private GameObject upgradedIcePrefab, upgradedThunderPrefab;
    internal bool WUpgraded = false, RUpgraded = false;

    [Header("Abilities options")] [SerializeField]
    private float areaAttackRadius = 5f;

    [SerializeField] public Ability normalTimer, fireTimer, iceTimer, thunderTimer;
    [SerializeField] private GameObject rAbilityTelegraph;
    private KeyCode qKey = KeyCode.Q, wKey = KeyCode.W, rKey = KeyCode.R;
    internal bool NormalCooldown, FireCooldown, IceCooldown, ThunderCooldown = false;

    //Script References
    private AbilityHolder targetedAttackAbilityHolder;
    private AbilityHolder areaAttackAbilityHolder;
    private ManaManager _manaManager;
    private ObjectiveUI objectiveUi;
    private ValuesTextsScript _valuesTexts;
    private PlayerAnimationHandler _playerAnim;
    private PlayerMovement _coroutineCaller;
    private NavMeshAgent _playerNavMesh;

    [Header("Others")] private RaycastHit _hit;
    private Vector3 _enemyPosition;
    [SerializeField] private float maxDistanceToCrystal = 10f;
    private GameObject targetedEnemy;

    private bool _gameplay;
    private bool rTelegraphIsOn = false;
    private Vector3 currentFirePointPosition;
    private Quaternion currentFirePointRotation;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        _manaManager = GetComponent<ManaManager>();
    }

    private void Start()
    {
        _playerNavMesh = GetComponent<NavMeshAgent>();
        targetedAttackAbilityHolder =
            GameObject.Find("Abilities UI").transform.GetChild(2).GetComponent<AbilityHolder>();
        areaAttackAbilityHolder = GameObject.Find("Abilities UI").transform.GetChild(4).GetComponent<AbilityHolder>();
        objectiveUi = FindObjectOfType<ObjectiveUI>();
        _playerAnim = GetComponentInChildren<PlayerAnimationHandler>();
        _coroutineCaller = FindObjectOfType<PlayerMovement>();

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
/*
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _playerAnim.CastBasicAttack();
            }
*/
            if(Input.GetKeyDown(KeyCode.Keypad5))
                _playerAnim.ToggleSwordMode();
            
            ShootInput();
            HoverHighlight();
        }

    }

    private void ShootInput()
    {
        if (_firePoint != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !NormalCooldown && !_playerAnim.cantUseOtherAbilities)
            {
                MagicType = WeaponType.Normal;
                Shoot();
            }
            else if (Input.GetKeyDown(qKey) && !FireCooldown && !_playerAnim.cantUseOtherAbilities)
            {
                MagicType = WeaponType.Fire;
                if (_manaManager.ManaCheck(MagicType))
                {
                    currentFirePointPosition = _firePoint.position;
                    currentFirePointRotation = _firePoint.rotation;

                    RaycastHit hit;
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        transform.LookAt(new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z));
                        StartCoroutine(FireAttackCooldown());
                        _playerAnim.QAttack();
                    }
                }
            }
            else if (Input.GetKeyDown(wKey) && !IceCooldown && !_playerAnim.cantUseOtherAbilities)
            {
                MagicType = WeaponType.Ice;
                if (_manaManager.ManaCheck(MagicType))
                {
                    _playerAnim.WAttack();
                }
            }
            else if (Input.GetKeyDown(rKey) && !ThunderCooldown && !_playerAnim.cantUseOtherAbilities)
            {
                MagicType = WeaponType.Thunder;
                if (_manaManager.ManaCheck(MagicType))
                {
                    _playerAnim.RAttack();
                    areaAttackAbilityHolder.AreaAttackCooldownUi();
                    StartCoroutine(ThunderAttackCooldown());
                }
            }
        }
    }

    public void RAbilityTelegraphActivation()
    {
        rTelegraphIsOn = !rTelegraphIsOn;

        if (rTelegraphIsOn)
            rAbilityTelegraph.SetActive(true);
        else
            rAbilityTelegraph.SetActive(false);
    }

    public void Shoot()
    {
        switch (MagicType)
        {
            case WeaponType.Normal: // input nº1
            {
                RaycastHit _hit;
                //Cleanse Crystal
                AIHandler[] ai = FindObjectsOfType<AIHandler>();
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 100))
                {
                    if (_hit.collider.GetComponent<CrystalSelect>())
                    {
                        if (ai.Length == 0 && _hit.collider.GetComponent<CrystalSelect>().enabled == true)
                        {
                            float distance = Vector3.Distance(_hit.collider.gameObject.transform.position,
                                gameObject.transform.position);

                            if (distance < maxDistanceToCrystal)
                            {

                                _hit.collider.GetComponent<MeshRenderer>().material.Lerp(
                                    _hit.collider.GetComponent<MeshRenderer>().material, cleansedCrystal, 1f);
                                // light color switch 
                                RuntimeManager.PlayOneShot(cleanCrystalSound, transform.position);
                                _hit.collider.GetComponentInChildren<Light>().color = lightColor;
                                _hit.collider.GetComponent<CrystalSelect>().enabled = false;
                                objectiveUi.CleansedTheCrystals();
                                _valuesTexts.GetCrystal();
#if UNITY_EDITOR
                                print("CLEANSED CRYSTAL");
                                
#endif
                                
                                break;
                            }

#if UNITY_EDITOR
                            Debug.Log("Too far away from the crystal");
#endif
                        }
                        else
                        {
#if UNITY_EDITOR
                            Debug.Log("There are still enemies alive");
#endif
                        }
                    }
                    else
                    {
                        StartCoroutine(NormalAttackCooldown());
                        Instantiate(normalPrefab, _firePoint.position, _firePoint.rotation);
                    }
                }

                break;
            }
            case WeaponType.Fire: // input nº2 (Q)
            {
                _manaManager.FireAttack();
                Instantiate(_firePrefab, currentFirePointPosition, currentFirePointRotation);
                break;
            }
            case WeaponType.Ice: // input nº3 (W)
            {
                TargetAttack();
                break;
            }
            case WeaponType.Thunder: // input nº4 (R)
            {
                _manaManager.ThunderAttack();
                AreaAttack();
                break;
            }

            default:
                break;
        }

    }

    private void HoverHighlight()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 50f))
            if (_hit.collider.GetComponent<AIHandler>())
            {
                _hit.collider.gameObject.GetComponent<Outline>().enabled = true;
            }

            else
            {
                AIHandler[] enemies = GameObject.FindObjectsOfType<AIHandler>();

                foreach (AIHandler enemy in enemies)
                {
                    enemy.gameObject.GetComponent<Outline>().enabled = false;

                }
            }

    }

    private void TargetAttack()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 500))
        {
            if (_hit.collider.GetComponent<AIHandler>())
            {
                targetedEnemy = _hit.collider.gameObject;
                Debug.Log("Enemy Hit with Ice");
                targetedAttackAbilityHolder.TargetedAttackCooldownUi();
                StartCoroutine(IceAttackCooldown());
                _manaManager.IceAttack();
            }
        }

    }

    public void TargetAttackVFX()
    {
        Instantiate(wVisualEffect, targetedEnemy.transform.position, _firePoint.rotation);
    }

    public void TargetAttackShoot()
    {
        Instantiate(_icePrefab, targetedEnemy.transform.position, _firePoint.rotation);
    }

    private void AreaAttack()
    {
        int maxColliders = 20;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, areaAttackRadius, hitColliders);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i]  != null)
                if (hitColliders[i].GetComponent<AIHandler>())
                {
                    // Deal damage to the enemy
                    Instantiate(_thunderPrefab, hitColliders[i].transform.position, _firePoint.rotation);
                    Instantiate(areaEffect, transform.position, Quaternion.identity);
                }
        }
        
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

    public void KeyChanger(int option)
    {
        if (option == 1)
        {
            qKey = KeyCode.Q;
            wKey = KeyCode.W;
            rKey = KeyCode.R;
        }
        else if (option == 2)
        {
            qKey = KeyCode.Alpha1;
            wKey = KeyCode.Alpha2;
            rKey = KeyCode.Alpha4;
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
        yield return new WaitForSeconds(thunderTimer.cooldownTime);
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