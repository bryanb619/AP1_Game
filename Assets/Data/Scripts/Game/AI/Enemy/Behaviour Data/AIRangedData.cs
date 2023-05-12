using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu (menuName ="Akarya/AI/Ranged Data")]
public class AiRangedData : ScriptableObject
{
    // ATTACK //
    [Header("Attack")]
    [Tooltip("Minimum distance to player or projectiles")]
    [Range(0f, 10f)]
    [SerializeField] private float      minDist = 7f;
    public float                        MinDist => minDist;

    [FormerlySerializedAs("n_projectile")] [SerializeField]
    private GameObject                  nProjectile;
    public GameObject                   NProjectile => nProjectile;
    
    [Tooltip("Attack rate")]
    [Range(1f, 10f)]
    [SerializeField] private float      attackRate = 5f;
    public float                        AttackRate => attackRate;

    // ATTACK  2//
    [FormerlySerializedAs("_percentage")]
    [Header("Attack Nº2")]
    [Tooltip("Higher the number, more possible it is to use attack nº2")]
    [Range(0f, 1f)]
    [SerializeField]
    private float                       percentage;
    public float                        Percentage => percentage;

    [FormerlySerializedAs("r_projectile")] [SerializeField]
    private GameObject                  rProjectile;

    [FormerlySerializedAs("s_projectile")] [SerializeField]
    private GameObject                  sProjectile;

    public GameObject                   RProjectile => rProjectile;
    internal GameObject                 SProjectile => sProjectile;
    

    [Range(0f, 10f)]
    [SerializeField]
    private float                       attackTimeOut;
    public float                        AttackTimeOut => attackTimeOut;

    // SPECIAL ATTACK //
    [Header("Special Attack")]
    [SerializeField]
    private float                       currentAbilityValue = 50f;
    public float                        CurrentAbilityValue => currentAbilityValue;    

    [SerializeField]
    private float                       abilityIncreasePerFrame = 3f;
    public float                        AbilityIncreasePerFrame => abilityIncreasePerFrame; 

    [SerializeField] 
    private int                         specialDamage = 25;
    public int                          SpecialDamage => specialDamage;

    [FormerlySerializedAs("s_damageEffect")] [SerializeField]
    private GameObject                  sDamageEffect; 
    public GameObject                   SDamageEffect => sDamageEffect;

    [Range(0f, 10f)]
    [SerializeField]
    private float                       specialTimeOut;
    public float                        SpecialTimeOut => specialTimeOut;


    // FOV //
    [Header("FOV")]
    [Tooltip("Radius of FOV")]
    [Range(10f, 150f)]
    [SerializeField] private float      radius;
    public float                        Radius => radius;

    [Tooltip("Angle of FOV")]
    [Range(0f, 360f)]
    [SerializeField] private float      angle;
    public float                        Angle => angle;

    [Tooltip("Select target Layer masks so AI can identify targets in FOV")]
    [SerializeField] private LayerMask targetMask;
    public LayerMask                   TargetMask => targetMask;

    [Tooltip("Select target Layer masks so AI can identify obstrutions in FOV")]
    [SerializeField] private LayerMask obstructionMask;
    public LayerMask                   ObstructionMask => obstructionMask;

    // COVER //
    [Header("Cover")]
    [Tooltip("Hidable masks layers for AI (Walls is one by default")]
    [SerializeField] private LayerMask  hidableLayers;
    public LayerMask                    HidableLayers => hidableLayers;

    [Range(0f, 15f)]
    [Tooltip("Minimum distance to find cover")]
    [SerializeField] private float      minDistInCover = 12f;
    public float                        MindistIncover => minDistInCover;

    // The speed at which the AI character moves
    [Tooltip("Speed of AI agent in seeking for cover")]
    [Range(1f, 15f)]
    [SerializeField] private float      coverMoveSpeed = 5f;
    public float                        CoverMoveSpeed => coverMoveSpeed;

    // The distance at which the AI character starts fleeing from the player
    [Tooltip("Minimal distance AI seeks of player in cover state")]
    [Range(1f, 50f)]
    [SerializeField] private float      fleeDistance = 15f;
    public float                        FleeDistance => fleeDistance;


    // HEALTH //
    [Header("Health")]
    [Tooltip("Set AI Health")]
    [Range(0, 150)]
    [SerializeField] private int        health;
    public int                          Health => health;

    [Tooltip("Heal rate, the higher the faster it will restore")]
    [Range(0f, 30f)]
    [SerializeField] private float      healthRegen;
    public float                        HealthRegen => healthRegen;

    [Tooltip("Maximum health regeneration")]
    [Range(0f, 150f)]
    [SerializeField] private float      maxHealthRegen;
    public float                        MaxHealthRegen => maxHealthRegen;

    //[Header("AI Weakness")]
    //[Tooltip("Value for every hit in player's health")]
    //[SerializeField] 
    private int        _damage;
    public int                          PlayerDamage => _damage;    

    //[Header("AI Weakness")]
    //[Tooltip("Set AI weaknesses")]
    
    //[SerializeField] 
    private bool       _ice;
    public bool                         Ice => _ice;

    //[SerializeField] 
    private bool       _fire;
    public bool                         Fire => _fire;

    //[SerializeField]
    private bool       _thunder;
    public bool                         Thunder => _thunder;

    [Header("Gem")]
    [Tooltip("Set true to spawn Gem")]
    [SerializeField] private bool       gemSpawnOnDeath;
    public bool                         GemSpawnOnDeath => gemSpawnOnDeath;

    [Tooltip("Select Gem prefab")]
    [SerializeField] 
    private GameObject                  gem;
    public GameObject                   Gem => gem;

    [Tooltip("Select Spawn chance")]
    [Range(0f, 100f)]
    [SerializeField] private float      spawnChance;
    public float                        SpawnChance => spawnChance;

    [FormerlySerializedAs("_spawnHealth")] [SerializeField]
    private bool                        spawnHealth;
    internal bool                       SpawnHealth => spawnHealth; 
    
    [FormerlySerializedAs("_healthItems")] [SerializeField]
    private int                         healthItems;
    internal int                        HealthItems => healthItems;   

    [FormerlySerializedAs("_healthDrop")] [SerializeField]
    private GameObject                  healthDrop;
    internal GameObject                 HealthDrop => healthDrop; 
    
    [FormerlySerializedAs("_spawnMana")] [SerializeField]
    private bool                        spawnMana;
    internal bool                       SpawnMana => spawnMana;

    [FormerlySerializedAs("_manaItems")] [SerializeField]
    private int                         manaItems;
    internal int                        ManaItems => manaItems;

    [FormerlySerializedAs("_manaDrop")] [SerializeField]
    private GameObject                  manaDrop;
    internal GameObject                 ManaDrop => manaDrop;

    [FormerlySerializedAs("_dropRadius")] [SerializeField]
    private int                         dropRadius;
    internal int                        DropRadius => dropRadius; 
}
