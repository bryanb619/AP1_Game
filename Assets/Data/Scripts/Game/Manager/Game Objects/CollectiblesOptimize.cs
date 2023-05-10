using UnityEngine;
using FMODUnity;
using UnityEngine.UIElements;

public class CollectiblesOptimize : MonoBehaviour
{

    [SerializeField] MeshRenderer mesh;

    //private SpecialItem _healthPoint;

    private RotateObject _rotate; 

    private StudioEventEmitter _event; 
    
    // Start is called before the first frame update
    void Start()
    {
        //_mesh = GetComponent<MeshRenderer>();

        //_healthPoint = GetComponent<SpecialItem>();
        
        _rotate             = GetComponentInParent<RotateObject>();
        _event              = GetComponent<StudioEventEmitter>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(mesh.isVisible)
        {
            //_healthPoint.enabled = true; 
            //_event.enabled = true; 
            _rotate.enabled = true;
            return;
        }
        else
        {
            _rotate.enabled = false; 
            //_healthPoint.enabled = false;
            //_event.enabled = false; 
            return; 
        }

        
    }
}
