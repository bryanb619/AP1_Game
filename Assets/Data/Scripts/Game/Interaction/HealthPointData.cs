using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/Interaction/Health Point")]
public class HealthPointData : ScriptableObject
{
   [Header("Health")]
   [Range(0f, 100f)]
   [SerializeField] private int _health;
   public int Health => _health;
}
