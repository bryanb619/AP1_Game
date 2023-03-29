using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Akarya/abilities/Projectiles and impact effects/Projectile data")]
public class ProjectileData : ScriptableObject
{
    [Header("Magic type")]
    [Tooltip("Select magic type")]
    [SerializeField]
    private WeaponType                                                          _magic;
    public WeaponType                                                           weapon => _magic;

    //[Header("Particles")]
    //[Tooltip("Select Particles")]

    //public ParticleSystem                                                       _projectileParticles;

    [Header("RigidBody Physics")]
    [Tooltip("Rigidbody componet must be edited in Prefab for desired changes such as gravity")]
    [SerializeField] private bool                                           _useRBPhysics;

    public bool                                                             UseRBPhysics => _useRBPhysics;  

    [Header("Projectile speed")]
    [Tooltip("Apply speed of projectile")]
    [Range(0f, 30f)]
    [SerializeField]
    private int                                                              _speed;
    public int                                                               Speed => _speed;

    [SerializeField] private float                                           _destroyTime;
    public float                                                             DestroyTime => _destroyTime;


    [Header("Enemy damage")]
    [Tooltip("value for ranged damage")]
    [SerializeField]
    private int                                                             _enemyRangedDamage;

    public int                                                              EnemyRangedDamage => _enemyRangedDamage;



    [Tooltip("Value for Chase damage")]
    [SerializeField]
    private int                                                             _enemyChaseDamage;

    public int                                                              EnemyChaseDamage => _enemyChaseDamage;

    [Tooltip("Value for Booss damage")]
    [SerializeField]
    private int _enemyBossDamage;

    public int                                                              EnemybossDamage => _enemyBossDamage;


    [Header("Impact effect")]
    [Tooltip("set to true")]
    [SerializeField]
    private bool                                                             _useImpact;

    public bool                                                              UseImpact => _useImpact;

    [Tooltip("Set impact game object and previous bool must be set to true")]
    [SerializeField]
    private GameObject _impactEffect;

    public GameObject                                                       ImpactEffect => _impactEffect;

    [Header("FMOD")]
    [Tooltip("Unecessary FMOD event path")]

    public string                                                           fmodSound; 
}
