using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(EnemyChaseBehaviour))]
public class FOVCEB : Editor
{
    private void OnSceneGUI()
    {
        EnemyChaseBehaviour fov = (EnemyChaseBehaviour)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.EEFOV.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.EEFOV.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.EEFOV.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(fov.EEFOV.position, fov.EEFOV.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.EEFOV.position, fov.EEFOV.position + viewAngle02 * fov.radius);

        if (fov.canSee)
        {
            Handles.color = Color.cyan;
            Handles.DrawLine(fov.EEFOV.position, fov.PlayerTarget.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

