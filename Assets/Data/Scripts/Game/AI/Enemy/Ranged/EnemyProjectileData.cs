using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Akarya/abilities/Projectiles and impact effects/Enemy Projectile data")]
public class EnemyProjectileData : ScriptableObject
{
    [FormerlySerializedAs("_projectileParticles")]
    [Header("Particles")]
    [Tooltip("Select Particles")]

    public ParticleSystem                       projectileParticles;

    [FormerlySerializedAs("_useRBPhysics")]
    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]


    public bool                                 useRbPhysics;

    public float                                thrust; 

    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 30f)]

    public int                                  speed;

    [Header("Enemy damage")]
    [Tooltip("damage")]

    public int                                  damage;

    public int                                  timeAirbone; 


  
    [FormerlySerializedAs("_useImpact")]
    [Header("Impact effect")]
    [Tooltip("set to true")]
    public bool useImpact;

    [Tooltip("Set impact game object and previous bool must be set to true")]

    public GameObject impactEffect;


}
