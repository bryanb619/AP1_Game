using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Drawing.Printing;

[InitializeOnLoad]
public class SceneSaver : Editor
{
    static SceneSaver()
    {
        // receive change
        EditorApplication.playModeStateChanged += SaveOnPlay;
    }

    private static void SaveOnPlay(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode)
        {

            string DebugColor = "<size=14><color=green>";
            string closeColor = "</color></size>";

            // print
            Debug.Log(DebugColor + "Auto Saving" + closeColor);

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }
    
}
