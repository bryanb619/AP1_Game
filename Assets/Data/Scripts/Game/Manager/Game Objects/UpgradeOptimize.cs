using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOptimize : MonoBehaviour
{
    [SerializeField]    private MeshRenderer mesh;
    private RotateObject _rotate;
    //private SpecialItem _specialItem; 
    private AbilityUpgradesManager _ability;
    //private StudioEventEmitter _event; 

    private void Awake()
    {
        _rotate             = GetComponentInParent<RotateObject>();
        _ability            = GetComponentInParent<AbilityUpgradesManager>();
    }
    
    private void Update()
    {
        CheckMesh();
        //StartCoroutine(CheckRoutine());
    }
    

    #region Mesh check
    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            CheckMesh();
        }
    }

    
    private void CheckMesh()
    {
        switch (mesh.isVisible)
        {
            case true:
            {
                ComponentState(true);
                
                break; 
            }
            case false:
            {
                ComponentState(false);
                break;
            }
        }
    }
    

    private void ComponentState(bool active)
    {
        switch (active)
        {
            case true:
            {
                _rotate.enabled         = true;
                _ability .enabled       = true; 
                break; 
            }
            case false:
            {
                _rotate.enabled         = false;
                _ability .enabled       = false;
                break;
            }
            
        }
    }
    #endregion 
}
