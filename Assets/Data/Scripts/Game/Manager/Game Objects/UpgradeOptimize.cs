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
    private WaitForSeconds _wait;


    private void Start()
    {
        
        _rotate             = GetComponentInParent<RotateObject>();
        _ability            = GetComponentInParent<AbilityUpgradesManager>(); 
        _wait                = new WaitForSeconds(1f);
        //_specialItem        = GetComponent<SpecialItem>();


        //_event              = GetComponent<StudioEventEmitter>();  
    }


    private void Update()
    {
        StartCoroutine(CheckRoutine());
    }


    #region Mesh check 

    
    private IEnumerator CheckRoutine()
    {

        while (true)
        {
            yield return _wait;
            CheckMesh();
        }
    }

    
    private void CheckMesh()
    {
        
        if(mesh.isVisible)
        {
            ComponentState(true);
        }
        else if (!mesh.isVisible)
        {
            ComponentState(false);
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
