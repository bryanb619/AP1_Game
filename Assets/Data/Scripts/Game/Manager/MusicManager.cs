using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
                        public enum MusicState          {Normal, Combat}

                        private MusicState              _state;

                        public static MusicManager      Instance;

    [SerializeField]    private EventReference          menuMusic, normalMusic, combatMusic; 

    private void Awake()
    {
        Instance = this;

        MusicManager[] OTHER_MANAGERS = FindObjectsOfType<MusicManager>();

        for (int i = 1; i < OTHER_MANAGERS.Length; i++)
        {
            Destroy(OTHER_MANAGERS[i].gameObject);
        }
    }

    

    public void UpdateMusic(MusicState newMusicState)
    {
        _state = newMusicState;

        switch (newMusicState)
        {
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

    private void HandleNormal()
    {
        
       
    }

    private void HandleCombat()
    {
        
    }
    
}