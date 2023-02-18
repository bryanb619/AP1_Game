using UnityEngine;

[CreateAssetMenu(menuName ="Akarya/abilities/Projectiles and impact effects/Projectile data")]
public class ProjectileData : ScriptableObject
{

    [Header("Magic type")]
    [Tooltip("Select magic type")]

    public WeaponType _magic;

    [Header("Particles")]
    [Tooltip("Select Particles")]

    public ParticleSystem _projectileParticles;

    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]

    public bool _useRBPhysics;

    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 20f)]

    public int speed; 

    [Header("Enemy damage")]
    [Tooltip("value for ranged enemy")]

    public int enemyRangedDamage;

    [Tooltip("Value for Chase enemy")]
    public int enemyChaseDamage;


    public int enemybossDamage;


    [Header("Impact effect")]
    [Tooltip("set to true")]
    public bool _useImpact;

    [Tooltip("Set impact game object and previous bool must be set to true")]
    public GameObject impactEffect; 

}
