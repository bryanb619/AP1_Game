using UnityEngine;
using UnityEngine.Serialization;

public class AiSpawn : MonoBehaviour
{

    [FormerlySerializedAs("m_SpawnObject")] [SerializeField] private GameObject mSpawnObject;


    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyUp( KeyCode.H))
        {
            Instantiate(m_SpawnObject, transform.position, Quaternion.identity);
        }*/
    }
}
