using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CompanionBehaviour))]
public class CompanionOnGui : Editor
{
    private void OnSceneGUI()
    {
        CompanionBehaviour _companionBehaviour = (CompanionBehaviour)target;

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


        switch (_companionBehaviour._stateAI)
        {
            case CompanionBehaviour.CompanionState._idle:
                {
                    Handles.Label(_companionBehaviour.transform.position + Vector3.up, "Idle  GamePlay: " + _companionBehaviour.Gameplay + " Ubstructed : " + _companionBehaviour.Obstructed, cyan);
                    break;
                }
            case CompanionBehaviour.CompanionState._follow:
                {
                    Handles.Label(_companionBehaviour.transform.position + Vector3.up, "Follow  GamePlay: " + _companionBehaviour.Gameplay + " Ubstructed : " + _companionBehaviour.Obstructed, green);
                    break;
                }
            case CompanionBehaviour.CompanionState._combat:
                {
                    Handles.Label(_companionBehaviour.transform.position + Vector3.up, "Combat  GamePlay: " + _companionBehaviour.Gameplay + " Ubstructed : " + _companionBehaviour.Obstructed, red);
                    break;
                }
        }
    }
}
