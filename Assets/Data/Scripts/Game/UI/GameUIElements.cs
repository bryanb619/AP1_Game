using System;
using UnityEngine;

public class GameUIElements: MonoBehaviour
{
    [SerializeField]    private bool                useAutoRotate; 
                        private Transform           _camTransform;
    
    private void Awake()
    {
        
        Camera mainCamera       = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _camTransform           = mainCamera.transform;
    }
    
    /// <summary>
    /// Updates the UI elements to face the camera.
    /// </summary>
    private void LateUpdate()
    {
        
        if (!useAutoRotate) return;
        
        Vector3 desiredTarget   = _camTransform.position - transform.position;
        Quaternion rotation     = Quaternion.LookRotation(desiredTarget);

        transform.rotation      = Quaternion.Euler(rotation.eulerAngles.x, 0, 0);
        

    }
}