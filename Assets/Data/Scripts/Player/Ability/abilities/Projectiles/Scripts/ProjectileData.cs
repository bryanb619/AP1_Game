using UnityEngine;

[CreateAssetMenu(menuName ="Akarya/Projectile")]
public class ProjectileData : ScriptableObject
{

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int enemyDamage;

    [Header("Ranged enemy damage value")]
    [Tooltip("Damage values are automaticaly updated on type change and will not be visable in inspector")]
    [SerializeField] private int enemyChaseDamage;


    [Header("Physics")]
    [SerializeField] private Rigidbody _rb;


}
