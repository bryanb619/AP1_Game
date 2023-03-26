using UnityEngine;
using FMODUnity; 
public class HealthOptimize : MonoBehaviour
{

    private MeshRenderer _mesh;

    private HealthPoint _healthPoint;

    private StudioEventEmitter _event; 
    
    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();

        _healthPoint = GetComponent<HealthPoint>();

        _event = GetComponent<StudioEventEmitter>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(_mesh.isVisible)
        {
            _healthPoint.enabled = true; 
            _event.enabled = true; 
            return;
        }
        else
        {
            _healthPoint.enabled = false;
            _event.enabled = false; 
            return; 
        }

        
    }
}
