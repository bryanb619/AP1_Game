using UnityEngine;
using UnityEngine.Serialization;

public class AiHandler : MonoBehaviour
{
    public bool AgentOperate { get; private set; }


    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    private void Update()
    {
        CheckVisibility();
    }
    private void CheckVisibility()
    {
        switch (skinnedMeshRenderer.isVisible)
        {
            case true:
                {
                    Visable();
                    break;
                }
            case false:
                {
                    Invisble();
                    break;
                }
        }
    }

    private void Visable()
    {
        AgentOperate = true;
        return;
    }

    private void Invisble()
    {
        AgentOperate = false;
        return; 
    }
}
