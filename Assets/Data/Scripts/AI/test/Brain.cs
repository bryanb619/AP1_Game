using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
public class Brain : MonoBehaviour
{   [HideInInspector] public NavMeshAgent _agent; 

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
