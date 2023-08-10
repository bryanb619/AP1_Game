using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileOnGUI : Editor
{
    private void OnSceneGUI()
    {
        Projectile projectile = (Projectile)target;

        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;

        GUIStyle yellow = new GUIStyle();
        yellow.normal.textColor = Color.yellow;

        GUIStyle blue = new GUIStyle();
        blue.normal.textColor = Color.blue;

        GUIStyle green = new GUIStyle();
        green.normal.textColor = Color.green;

        GUIStyle cyan = new GUIStyle();
        cyan.normal.textColor = Color.cyan;

        switch (projectile.Gameplay)
        {
            case true:
                {
                    Handles.Label(projectile.transform.position + Vector3.up, "GamePlay: ", green);
                    break;
                }
            case false:
                {
                    Handles.Label(projectile.transform.position + Vector3.up, "Paused: ", red);
                    break;
                }

        }

    }
    
}
