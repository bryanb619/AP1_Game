using System;
using UnityEngine;

public class GameUIElements: MonoBehaviour
{
    private Camera              _mainCamera;
    private Transform           _camTransform;

    private Vector3             _desiredTarget;
    
    

    private void Awake()
    {
        _mainCamera                 = FindObjectOfType<Camera>();
        _camTransform               = _mainCamera.transform;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        //transform.LookAt(_camTransform.position);

        _desiredTarget = _camTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(_desiredTarget);

        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
