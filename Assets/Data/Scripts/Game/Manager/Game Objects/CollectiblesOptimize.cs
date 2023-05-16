using System.Collections;
using UnityEngine;

public class CollectiblesOptimize : MonoBehaviour
{

    [SerializeField]    private MeshRenderer mesh;
                        private RotateObject _rotate;
                        private SpecialItem _specialItem; 
                        //private StudioEventEmitter _event; 

                       
    private void Awake()
    {
        _rotate             = GetComponentInParent<RotateObject>();
        _specialItem        = GetComponent<SpecialItem>();
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
