using UnityEngine;

[CreateAssetMenu (menuName ="Akarya/AI/Boss Data")]
public class AIBossData : ScriptableObject
{
    // FOV // 
    [Header("FOV Masks")]
    [SerializeField] private LayerMask targetMask;
    public LayerMask TargetMask => targetMask;

    [SerializeField] private LayerMask obstructionMask;
    public LayerMask ObstructionMask => obstructionMask;


    // Hide //
    [Header("Hide")]
    private float minObstacleHeight = 0.3f;
    public float MinObstacleHeight => minObstacleHeight; 

}
