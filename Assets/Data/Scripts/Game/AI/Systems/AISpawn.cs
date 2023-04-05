using UnityEngine;

public class AISpawn : MonoBehaviour
{

    [SerializeField] private GameObject m_SpawnObject;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.H))
        {
            Instantiate(m_SpawnObject, transform.position, Quaternion.identity);
        }
    }
}
