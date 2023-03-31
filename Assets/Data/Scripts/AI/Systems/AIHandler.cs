using UnityEngine;

public class AIHandler : MonoBehaviour
{
    private bool _agentOperate; 
    public bool AgentOperate => _agentOperate;


    [SerializeField] SkinnedMeshRenderer _skinnedMeshRenderer;

    private void Update()
    {
        CheckVisiblity();
    }

    private void CheckVisiblity()
    {
        switch (_skinnedMeshRenderer.isVisible)
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
        _skinnedMeshRenderer.enabled = true;
        return;
    }

    private void Invisble()
    {
        _agentOperate = false;
        _skinnedMeshRenderer.enabled = true;
        return; 
    }
}
