using System;
using UnityEngine;

public class GameUIElements: MonoBehaviour
{
    private Transform           _camTransform;

    
    private void Awake()
    {
        //Camera mainCamera       = FindObjectOfType<Camera>();

        Camera mainCamera       = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _camTransform           = mainCamera.transform;
    }
    
    
    private void LateUpdate()
    {
        //transform.LookAt(_camTransform.position);

        Vector3 desiredTarget   = _camTransform.position - transform.position;
        Quaternion rotation     = Quaternion.LookRotation(desiredTarget);

        transform.rotation      = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
