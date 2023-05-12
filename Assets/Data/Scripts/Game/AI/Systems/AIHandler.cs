using UnityEngine;
using UnityEngine.Serialization;

public class AiHandler : MonoBehaviour
{
    private bool _agentOperate; 
    public bool AgentOperate => _agentOperate;


    [FormerlySerializedAs("_skinnedMeshRenderer")] [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    private void Update()
    {
        CheckVisiblity();
    }
    private void CheckVisiblity()
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
        _agentOperate = true;
        return;
    }

    private void Invisble()
    {
        _agentOperate = false;
        return; 
    }
}
