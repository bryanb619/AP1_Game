using UnityEditor;

[CustomEditor(typeof(EnemyChaseBehaviour))]
public class AiBehaviour : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var enemyChaseBehaviour = (EnemyChaseBehaviour)target;
        
       
    }
}
  

