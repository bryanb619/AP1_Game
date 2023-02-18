using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Projectiles & impact effects/Impact data")]
public class ImpactData : ScriptableObject
{
    [Header("Time")]
    [Tooltip("Set maximum time of impact time visable in scene")]
    [Range(0f, 10f)]
    public int timeInScene;




    
}
