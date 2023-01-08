using UnityEngine;

public class AIAlert : MonoBehaviour
{
    public void GetPlayer(Transform Target)
    {
          
        transform.LookAt(new Vector3(0, Target.position.y, 0));
    }
  
}
