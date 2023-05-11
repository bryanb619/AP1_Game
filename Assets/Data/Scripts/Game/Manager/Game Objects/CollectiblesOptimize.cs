using System.Collections;
using UnityEngine;
using FMODUnity;
using UnityEngine.UIElements;

public class CollectiblesOptimize : MonoBehaviour
{

    [SerializeField]    private MeshRenderer mesh;
                        private RotateObject _rotate;
                        private SpecialItem _specialItem; 
                        //private StudioEventEmitter _event; 
                        
    private void Start()
    {
        
        _rotate             = GetComponentInParent<RotateObject>();
        _specialItem        = GetComponent<SpecialItem>();
        
        //_event              = GetComponent<StudioEventEmitter>();  
    }


    private void Update()
    {
        StartCoroutine(CheckRoutine());
    }


    #region Mesh check 

    
    private IEnumerator CheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;
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
                _specialItem.enabled    = true; 
                break; 
            }
            case false:
            {
                _rotate.enabled         = false;
                _specialItem.enabled    = false;
                break;
            }
            
        }
    }
    #endregion 
}
