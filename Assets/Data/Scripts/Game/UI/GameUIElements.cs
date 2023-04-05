using UnityEngine;

public class GameUIElements : MonoBehaviour
{
    private Camera mainCamera;

    public Transform _camTransform; 

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        _camTransform = mainCamera.transform;
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(_camTransform.position);   
    }
}
