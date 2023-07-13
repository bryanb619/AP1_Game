using UnityEngine;

public class MaterialDisable : MonoBehaviour
{
    private void Awake()
    {
        MaterialDisabler(); 
    }  
    
    /// <summary>
    /// Disables the mesh renderers of its children
    /// </summary>
    private void MaterialDisabler()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer currentRender in renderers)
            currentRender.enabled = false;;
    }
}
