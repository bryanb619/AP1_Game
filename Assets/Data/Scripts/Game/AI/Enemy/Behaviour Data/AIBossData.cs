using UnityEngine;

[CreateAssetMenu (menuName ="Akarya/AI/Boss Data")]
public class AiBossData : ScriptableObject
{
    // FOV // 
    [Header("FOV Masks")]
    [SerializeField] private LayerMask targetMask;
    public LayerMask TargetMask => targetMask;

    [SerializeField] private LayerMask obstructionMask;
    public LayerMask ObstructionMask => obstructionMask;


    // Hide //
    [Header("Hide")]
    private float _minObstacleHeight = 0.3f;
    public float MinObstacleHeight => _minObstacleHeight; 

}
