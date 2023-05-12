using UnityEngine;
using UnityEngine.Serialization;

public class HidePoint : MonoBehaviour
{
    private MeshRenderer _mRenderer;

    [SerializeField] Material normal;
    [FormerlySerializedAs("AlphaLow")] [SerializeField] Material alphaLow;

    // Start is called before the first frame update
    void Start()
    {
        _mRenderer = GetComponent<MeshRenderer>();
        _mRenderer.material = normal;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider hitInfo)
    {
        PlayerMovement player = GetComponent<PlayerMovement>(); 
        
        if (player != null) 
        {
            _mRenderer.material = alphaLow;
            print("changed");
        }
    }
    private void OnTriggerExit(Collider hitInfo)
    {
        PlayerMovement player = GetComponent<PlayerMovement>();
        if (player != null) 
        {
            _mRenderer.material = normal;
            print("changed to normal");
        }

    }
}
