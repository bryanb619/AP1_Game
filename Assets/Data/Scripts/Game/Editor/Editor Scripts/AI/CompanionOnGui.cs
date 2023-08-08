using Data.Scripts.Game.AI.Companion;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CompanionBehaviour))]
public class CompanionOnGui : Editor
{
    private void OnSceneGUI()
    {
        CompanionBehaviour companionBehaviour = (CompanionBehaviour)target;

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


        switch (companionBehaviour.stateAi)
        {
            case CompanionBehaviour.CompanionState.Idle:
                {
                    Handles.Label(companionBehaviour.transform.position + Vector3.up, "Idle  GamePlay: " + companionBehaviour.Gameplay, cyan);
                    break;
                }
            case CompanionBehaviour.CompanionState.Follow:
                {
                    Handles.Label(companionBehaviour.transform.position + Vector3.up, "Follow  GamePlay: " + companionBehaviour.Gameplay, green);
                    break;
                }
            case CompanionBehaviour.CompanionState.Combat:
                {
                    Handles.Label(companionBehaviour.transform.position + Vector3.up, "Combat  GamePlay: " + companionBehaviour.Gameplay + " Change Pos : " + companionBehaviour.ChangePos, red);
                    break;
                }
        }
    }
}
