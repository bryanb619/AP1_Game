using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInput
{
    Keyboard,
    Mouse
}

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private PlayerInput playerInput;

    public static PlayerControl Instance;
    private Shooter shooterScript;
    private Dashing dashing;
    [SerializeField]private AbilityHolder qAbility, wAbility, eAbility, rAbility;
    public static event Action<PlayerInput> OnPlayerInputChanged;


    private void Start()
    {
        shooterScript = FindObjectOfType<Shooter>();
        dashing = FindObjectOfType<Dashing>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);

        Instance = this;

        PlayerControl[] otherManagers = FindObjectsOfType<PlayerControl>();

        for (int i = 1; i < otherManagers.Length; i++)
        {
            Destroy(otherManagers[i].gameObject);
        }

        playerInput = PlayerInput.Mouse; 
        
        UpdatePlayerControl(playerInput);

    }

    public void UpdatePlayerInput()
    {
        if (playerInput == PlayerInput.Keyboard)
        {
            shooterScript.KeyChanger(2);
            dashing.KeyChanger(2);
            //abilityHolder.gameObject.transform.Find("Ice/TargetShot (W)").GetComponent<AbilityHolder>().KeyChanger(KeyCode.Alpha2);
            qAbility.KeyChanger(KeyCode.Alpha1);
            wAbility.KeyChanger(KeyCode.Alpha2);
            eAbility.KeyChanger(KeyCode.Alpha3);
            rAbility.KeyChanger(KeyCode.Alpha4);
        }
        else if (playerInput == PlayerInput.Mouse)
        {
            shooterScript.KeyChanger(1);
            dashing.KeyChanger(1);
            qAbility.KeyChanger(KeyCode.Q);
            wAbility.KeyChanger(KeyCode.W);
            eAbility.KeyChanger(KeyCode.E);
            rAbility.KeyChanger(KeyCode.R);
        }
    }

    public void UpdatePlayerControl(PlayerInput newControl)
    {
        playerInput = newControl;
        
        switch(newControl)
        {
            case PlayerInput.Keyboard:
            {
                // call Player control to update for Keyboard
                newControl = PlayerInput.Keyboard;
                break;
            }
            case PlayerInput.Mouse:
            {
                // Call Player control to update for Mouse
                newControl = PlayerInput.Mouse;
               
                break;
            }       
        }
        OnPlayerInputChanged?.Invoke(newControl);
        
    }
}
