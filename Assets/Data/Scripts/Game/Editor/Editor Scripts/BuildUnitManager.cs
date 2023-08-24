using UnityEditor;
using UnityEngine;

namespace Data.Scripts.Game.Editor.Editor_Scripts
{
    public class BuildUnitManager : EditorWindow
    {
        
        [MenuItem("Build Setting Build/Apply Build Target")]
        
        public static void SwitchBuildTarget()
        {
        
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.PS4)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS4, BuildTarget.PS4);
                
                // Call Game Manager
                GameManager _manager = FindObjectOfType<GameManager>();
                _manager.platform = BuildUnit.Console;
           
                // DEBUG ------------------------------------------------------------------------------------------>
                const string debugColor = "<size=14><color=white>";
                const string closeColor = "</color></size>";
                Debug.Log(debugColor + "Build Target is PlayStation 4" + closeColor);
        
            }
            
            
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.PS5)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS5, BuildTarget.PS5);
                
                // Call Game Manager
                //GameManager _manager = FindObjectOfType<GameManager>();
                //_manager.platform = BuildUnit.Console;
           
                // DEBUG ------------------------------------------------------------------------------------------>
                const string debugColor = "<size=14><color=white>";
                const string closeColor = "</color></size>";
                Debug.Log(debugColor + "Build Target is PlayStation 5" + closeColor);
        
            }

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                
                // Call Game Manager
                GameManager _manager = FindObjectOfType<GameManager>();
                _manager.platform = BuildUnit.PC;
                
                // DEBUG ------------------------------------------------------------------------------------------>
                const string debugColor = "<size=14><color=lightblue>";
                const string closeColor = "</color></size>";
                Debug.Log(debugColor + "Build Target is Microsoft Windows 64" + closeColor);

            }
        }

    }
}
