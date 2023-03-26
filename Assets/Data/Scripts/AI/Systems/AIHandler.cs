using UnityEngine;

public class AIHandler : MonoBehaviour
{

    [SerializeField] private EnemyBehaviour _enemyRanged;

    [SerializeField] private MeshRenderer _meshRenderer;


    //private SkinnedMeshRenderer _skinnedMeshRenderer;

    private void Update()
    {
        CheckVisiblity();
    }

    private void CheckVisiblity()
    {
        switch (_meshRenderer.isVisible)
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
       _enemyRanged.enabled = true;
       _meshRenderer.enabled = true;
        return; 
    }

    private void Invisble()
    {
        _enemyRanged.enabled = false;
        _meshRenderer.enabled = false;
        return; 
    }
}
