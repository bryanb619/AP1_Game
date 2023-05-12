using UnityEngine;

public class AiAlert : MonoBehaviour
{
    public void GetPlayer(Transform target)
    {   
        transform.LookAt(new Vector3(0, target.position.y, 0));
    }
  
}
