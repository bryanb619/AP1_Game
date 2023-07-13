#region Script used to deactivate collider meshes used for cover and other purposes  
#endregion
using UnityEngine;

public class MeshDisable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer currentRender in renderers)
            currentRender.enabled = false;
    }  
}
