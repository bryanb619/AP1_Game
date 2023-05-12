using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Akarya/abilities/Projectiles and impact effects/Projectile data")]
public class ProjectileData : ScriptableObject
{
    [FormerlySerializedAs("_magic")]
    [Header("Magic type")]
    [Tooltip("Select magic type")]
    [SerializeField]
    private WeaponType                                                          magic;
    public WeaponType                                                           Weapon => magic;

    //[Header("Particles")]
    //[Tooltip("Select Particles")]

    //public ParticleSystem                                                       _projectileParticles;

    [FormerlySerializedAs("_useRBPhysics")]
    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]
    [SerializeField] private bool                                           useRbPhysics;

    public bool                                                             UseRbPhysics => useRbPhysics;  

    [FormerlySerializedAs("_speed")]
    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 30f)]
    [SerializeField]
    private int                                                              speed;
    public int                                                               Speed => speed;

    [FormerlySerializedAs("_destroyTime")] [SerializeField] private float                                           destroyTime;
    public float                                                             DestroyTime => destroyTime;


    [FormerlySerializedAs("_enemyRangedDamage")]
    [Header("Enemy damage")]
    [Tooltip("value for ranged damage")]
    [SerializeField]
    private int                                                             enemyRangedDamage;

    public int                                                              EnemyRangedDamage => enemyRangedDamage;

    [FormerlySerializedAs("_enemyChaseDamage")]
    [Tooltip("Value for Chase damage")]
    [SerializeField]
    private int                                                             enemyChaseDamage;

    public int                                                              EnemyChaseDamage => enemyChaseDamage;

    [FormerlySerializedAs("_enemyBossDamage")]
    [Tooltip("Value for Booss damage")]
    [SerializeField]
    private int                                                             enemyBossDamage;

    public int                                                              EnemybossDamage => enemyBossDamage;


    [FormerlySerializedAs("_useImpact")]
    [Header("Impact effect")]
    [Tooltip("set to true")]
    [SerializeField]
    private bool                                                             useImpact;

    public bool                                                              UseImpact => useImpact;

    [FormerlySerializedAs("_impactEffect")]
    [Tooltip("Set impact game object and previous bool must be set to true")]
    [SerializeField]
    private GameObject impactEffect;

    public GameObject                                                       ImpactEffect => impactEffect;

    [Header("FMOD")]
    [Tooltip("Unecessary FMOD event path")]

    public string                                                           fmodSound; 
}
