using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Akarya/abilities/Projectiles and impact effects/Projectile data")]
public class ProjectileData : ScriptableObject
{
    [Header("Magic type")]
    [Tooltip("Select magic type")]
    [SerializeField]
    private WeaponType                                                          magic;
    public WeaponType                                                           Weapon => magic;

    //[Header("Particles")]
    //[Tooltip("Select Particles")]

    //public ParticleSystem                                                       _projectileParticles;

    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]
    [SerializeField] private bool                                           useRbPhysics;

    public bool                                                             UseRbPhysics => useRbPhysics;  

    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 30f)]
    [SerializeField]
    private int                                                              speed;
    public int                                                               Speed => speed;

    [SerializeField] private float                                           destroyTime;
    public float                                                             DestroyTime => destroyTime;


    [Header("Enemy damage")]
    [Tooltip("value for ranged damage")]
    [SerializeField]
    public int                                                             enemyRangedDamage;

    public int                                                              EnemyRangedDamage => enemyRangedDamage;

    [Tooltip("Value for Chase damage")]
    [SerializeField]
    public int                                                             enemyChaseDamage;

    public int                                                              EnemyChaseDamage => enemyChaseDamage;

    [Tooltip("Value for Booss damage")]
    [SerializeField]
    public int                                                             enemyBossDamage;

    public int                                                              EnemybossDamage => enemyBossDamage;


    [Header("Impact effect")]
    [Tooltip("set to true")]
    [SerializeField]
    private bool                                                             useImpact;

    public bool                                                              UseImpact => useImpact;

    [Tooltip("Set impact game object and previous bool must be set to true")]
    [SerializeField]
    private GameObject impactEffect;

    public GameObject                                                       ImpactEffect => impactEffect;

    [Header("FMOD")]
    [Tooltip("Unecessary FMOD event path")]

    public string                                                           fmodSound; 
}
