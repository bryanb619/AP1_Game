using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{

    private void Update()
    {
        LookAtUpdate();
    }
    private void LookAtUpdate()
    {
        RaycastHit HitInfo;
        Ray RayCast;

        RayCast = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            transform.LookAt(HitInfo.point);
        }
        else
        {
            transform.LookAt(transform.forward);
        }
    }
}
