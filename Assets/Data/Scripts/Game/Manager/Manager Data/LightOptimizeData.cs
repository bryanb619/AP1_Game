using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/Manager/Light Optimize")]
public class LightOptimizeData : ScriptableObject
{
    [Range(0f, 120f)]
    [SerializeField] private float availableDistance;
    public float Distance => availableDistance;
}
