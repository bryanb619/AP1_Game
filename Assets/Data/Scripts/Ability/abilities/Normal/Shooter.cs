using UnityEngine;

public class Shooter : MonoBehaviour
{
  
    [SerializeField] private Transform      firePoint;
    [SerializeField] private GameObject     normalPrefab, firePrefab, icePrefab, thunderPrefab;

    public WeaponType            _magicType;
    private bool                  _gameplay;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    private void Start()
    {
        _magicType = WeaponType.Normal;
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
            ShootPos();
            ShootInput();
        }
        
    }

    private void ShootPos()
    {

       
    }

    private void ShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _magicType = WeaponType.Normal;
            print("Ability is set to normal");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _magicType = WeaponType.Fire;
            print("Ability is set to fire");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _magicType = WeaponType.Ice;
            print("Ability is set to Ice");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("Ability is set to Thunder");
            _magicType = WeaponType.Thunder;
        }
    }

    public void Shoot()
    {
        switch(_magicType)
        {
            case WeaponType.Normal: // input nº1
                {
                    // call Script for time out
                    
                    Instantiate(normalPrefab, firePoint.position, firePoint.rotation);
                    break; 
                }
            case WeaponType.Fire: // input nº2
                {
                    Instantiate(firePrefab, firePoint.position, firePoint.rotation);
                    break;
                }
            case WeaponType.Ice: // input nº3
                {
                    Instantiate(icePrefab, firePoint.position, firePoint.rotation);
                    break; 
                }
            case WeaponType.Thunder: // input nº4
                {
                    Instantiate(thunderPrefab, firePoint.position, firePoint.rotation);
                    break;
                }
            default: {break;}
        }
        
    }


    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}
