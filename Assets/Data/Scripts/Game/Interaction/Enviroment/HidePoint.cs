using UnityEngine;

public class HidePoint : MonoBehaviour
{
    private MeshRenderer m_Renderer;

    [SerializeField] Material normal, AlphaLow;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Renderer.material = normal;
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
            m_Renderer.material = AlphaLow;
            print("changed");
        }
    }
    private void OnTriggerExit(Collider hitInfo)
    {
        PlayerMovement player = GetComponent<PlayerMovement>();
        if (player != null) 
        {
            m_Renderer.material = normal;
            print("changed to normal");
        }

    }
}
