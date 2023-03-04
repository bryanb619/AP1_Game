using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/abilities/Projectiles and impact effects/Enemy Projectile data")]
public class EnemyProjectileData : ScriptableObject
{
    [Header("Particles")]
    [Tooltip("Select Particles")]

    public ParticleSystem                       _projectileParticles;

    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]


    public bool                                 _useRBPhysics;

    public float                                thrust; 

    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 30f)]

    public int                                  speed;

    [Header("Enemy damage")]
    [Tooltip("damage")]

    public int                                  damage;

    public int                                  timeAirbone; 

  
    [Header("Impact effect")]
    [Tooltip("set to true")]
    public bool _useImpact;

    [Tooltip("Set impact game object and previous bool must be set to true")]

    public GameObject impactEffect;


}
