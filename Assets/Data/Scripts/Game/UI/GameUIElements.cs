using UnityEngine;

public class GameUIElements : MonoBehaviour
{
    private Camera              mainCamera;
    private Transform           _camTransform;

    private Vector3             desiredTarget; 

    // Start is called before the first frame update
    void Start()
    {
        mainCamera              = FindObjectOfType<Camera>();
        _camTransform           = mainCamera.transform;
    }
  
    // Update is called once per frame
    void LateUpdate()
    {
        //transform.LookAt(_camTransform.position);

        desiredTarget = _camTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(desiredTarget);

        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
