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
    public static event Action<PlayerInput> OnPlayerInputChanged;
    
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
