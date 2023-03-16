using UnityEngine;

public class GameUIElements : MonoBehaviour
{
    private Camera camera;

    public Transform _camTransform; 

    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<Camera>();

        _camTransform = camera.transform;
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_camTransform.position);   
    }
}
