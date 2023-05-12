using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
                        public enum MusicState          {Menu, Normal, Combat}

                        private MusicState              _state;

                        public static MusicManager      Instance;

    [SerializeField]    private StudioEventEmitter      menuMusic, normalMusic, combatMusic;
                        private StudioEventEmitter[]    _eventEmitters;  

    private void Awake()
    {
        Instance = this;

        MusicManager[] otherManagers = FindObjectsOfType<MusicManager>();

        for (int i = 1; i < otherManagers.Length; i++)
        {
            Destroy(otherManagers[i].gameObject);
        }
    }

    private void Start()
    {
        _eventEmitters = GetComponentsInChildren<StudioEventEmitter>();
    }

    #region  
    
    public void UpdateMusic(MusicState newMusicState)
    {
        _state = newMusicState;

        switch (newMusicState)
        {
            case MusicState.Menu:
            {
                HandleNormal();
                break; 
            }
            case MusicState.Normal:
            {
                HandleNormal();
                break; 
            }
            case MusicState.Combat:
            {
                HandleCombat();
                break; 

            }
            default: { throw new ArgumentOutOfRangeException(nameof(newMusicState), newMusicState, null); }
            
        }

    }
    private void HandleMenu()
    {
        
       
    }

    private void HandleNormal()
    {
        
       
    }

    private void HandleCombat()
    {
        
    }

    public void MusicVolume(float volume)
    {
        print("triggered in manager");
        for (int i = 0; i < _eventEmitters.Length; i++)
        {
            _eventEmitters[i].EventInstance.setVolume(volume);
        }
        
    }
    #endregion
}