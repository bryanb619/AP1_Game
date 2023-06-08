using UnityEngine;
using UnityEngine.Serialization;

public class AIHandler : MonoBehaviour
{
    
    public enum ActiveAi { Active, Inactive }
    public ActiveAi activeAi;
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
                    Visible();
                    break;
                }
            case false:
                {
                    Invisible();
                    break;
                }
        }
    }

    private void Visible()
    {
        AgentOperate = true;
        activeAi = ActiveAi.Active;
        
        
    }

    private void Invisible()
    {
        AgentOperate = false;
        activeAi = ActiveAi.Inactive;
    }
}
