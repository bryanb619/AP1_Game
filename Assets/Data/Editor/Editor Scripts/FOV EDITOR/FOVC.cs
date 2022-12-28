using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FOV))]
public class EditorFOV : Editor
{

    private void OnSceneGUI()
    {
        FOV fov = (FOV)target;
        Handles.color = Color.grey;
        Handles.DrawWireArc(fov.pfovPoS.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.pfovPoS.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.pfovPoS.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(fov.pfovPoS.position, fov.pfovPoS.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.pfovPoS.position, fov.pfovPoS.position + viewAngle02 * fov.radius);

        
        if (fov.canSee)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.pfovPoS.position, fov.Target.transform.position);
        }
        
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
