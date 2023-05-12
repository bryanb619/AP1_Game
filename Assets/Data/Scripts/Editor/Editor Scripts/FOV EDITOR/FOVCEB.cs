using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(EnemyChaseBehaviour))]
public class FOVCEB : Editor
{
    private void OnSceneGUI()
    {
        EnemyChaseBehaviour fov = (EnemyChaseBehaviour)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.Eefov.position, Vector3.up, Vector3.forward, 360, fov.Radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.Eefov.transform.eulerAngles.y, -fov.Angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.Eefov.eulerAngles.y, fov.Angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(fov.Eefov.position, fov.Eefov.position + viewAngle01 * fov.Radius);
        Handles.DrawLine(fov.Eefov.position, fov.Eefov.position + viewAngle02 * fov.Radius);

        if (fov.CanSee)
        {
            Handles.color = Color.cyan;
            Handles.DrawLine(fov.Eefov.position, fov.PlayerTarget.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

