using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBehaviour))]
public class FOVEEB : Editor
{
    private void OnSceneGUI()
    {
        EnemyBehaviour fov = (EnemyBehaviour)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.EEFOV.position, Vector3.up, Vector3.forward, 360, fov.Radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.EEFOV.transform.eulerAngles.y, -fov.Angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.EEFOV.eulerAngles.y, fov.Angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(fov.EEFOV.position, fov.EEFOV.position + viewAngle01 * fov.Radius);
        Handles.DrawLine(fov.EEFOV.position, fov.EEFOV.position + viewAngle02 * fov.Radius);
            
        if (fov.canSee)
        {
            Handles.color = Color.cyan;
            Handles.DrawLine(fov.EEFOV.position, fov.playerTarget.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
