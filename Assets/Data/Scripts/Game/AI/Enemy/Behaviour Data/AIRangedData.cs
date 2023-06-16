using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu (menuName ="Akarya/AI/Ranged Data")]
public class AIRangedData : ScriptableObject
{
  
    [Header("AI Type")]
    public EnemyType                    enemyType;
    
    // ATTACK //
    [Header("Attack")]
    [Tooltip("Minimum distance to player or projectiles")]
    [Range(0f, 30)]
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
    
    [SerializeField] private float     accuracy = 0.6f;
    public float                       Accuracy => accuracy;
    [SerializeField] private float    shootSpread = 0.1f;
    public float                       ShootSpread => shootSpread;

    // SPECIAL ATTACK //
    [Header(" BossAttack")]
    [SerializeField]
    private float                       currentAbilityValue = 50f;
    public float                        CurrentAbilityValue => currentAbilityValue;    

    [SerializeField]
    private float                       abilityIncreasePerFrame = 3f;
    public float                        AbilityIncreasePerFrame => abilityIncreasePerFrame; 
    
    private GameObject                  sDamageEffect; 
    public GameObject                   SDamageEffect => sDamageEffect;
    [Range(0f, 10f)]
    [SerializeField]
    private float                       specialTimeOut;
    public float                        SpecialTimeOut => specialTimeOut;
    
    [SerializeField] private float      specialAttackRate = 3f;
    public float                        SpecialAttackRate => specialAttackRate;
    
    [SerializeField] private int      areaDamageAttack = 30;
    public int                          AreaDamageAttack => areaDamageAttack;
    
    [SerializeField] private float      areaDamageRadius = 10;
    public float                        AreaDamageRadius => areaDamageRadius;
    
    [SerializeField] private float      teleportMinRange, teleportMaxRange;
    public float                        TeleportMinRange => teleportMinRange;
    public float                        TeleportMaxRange => teleportMaxRange;
    
    [SerializeField] private float      teleportTime;
    public float                        TeleportTime => teleportTime;

    [SerializeField] private GameObject teleportEffect;
    public GameObject                   TeleportEffect => teleportEffect;

    [SerializeField] private float      currentTp, cooldownTp, tpMaxValue, tpIncreasePerFrame; 
    
    public float                       CurrentTp => currentTp;
    public float                       CooldownTp => cooldownTp;
    public float                       TpMaxValue => tpMaxValue;
    public float                       TpIncreasePerFrame => tpIncreasePerFrame;
    
    
    [SerializeField] private GameObject areaAttack; 
    public GameObject                   AreaAttack => areaAttack;
    
    
    

    /*
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
*/

    // HEALTH //
    [Header("Health")]
    [Tooltip("Set AI Health")]
    [Range(0, 9999)]
    [SerializeField] private int        health;
    public int                          Health => health;

    [Tooltip("Heal rate, the higher the faster it will restore")]
    [Range(0f, 9999f)]
    [SerializeField] private float      healthRegen;
    public float                        HealthRegen => healthRegen;

    [Tooltip("Maximum health regeneration")]
    [Range(0f, 9999f)]
    [SerializeField] private float      maxHealthRegen;
    public float                        MaxHealthRegen => maxHealthRegen;

    //[Header("AI Weakness")]
    //[Tooltip("Value for every hit in player's health")]
    //[SerializeField] 
    private int        _damage;
    public int                          PlayerDamage => _damage;    

    //[Header("AI Weakness")]
    //[Tooltip("Set AI weaknesses")]
    

    [Tooltip("Select Spawn chance")]
    [Range(0f, 100f)]
    [SerializeField] private float      spawnChance;
    public float                        SpawnChance => spawnChance;

    [FormerlySerializedAs("_spawnHealth")] [SerializeField]
    private bool                        spawnHealth;
    public bool                       SpawnHealth => spawnHealth; 
    
    [FormerlySerializedAs("_healthItems")] [SerializeField]
    private int                         healthItems;
    public int                        HealthItems => healthItems;   

    [FormerlySerializedAs("_healthDrop")] [SerializeField]
    private GameObject                  healthDrop;
    public GameObject                 HealthDrop => healthDrop; 
    
    [FormerlySerializedAs("_spawnMana")] [SerializeField]
    private bool                        spawnMana;
    public bool                       SpawnMana => spawnMana;

    [FormerlySerializedAs("_manaItems")] [SerializeField]
    private int                         manaItems;
    public int                        ManaItems => manaItems;

    [FormerlySerializedAs("_manaDrop")] [SerializeField]
    private GameObject                  manaDrop;
    public GameObject                 ManaDrop => manaDrop;

    [FormerlySerializedAs("_dropRadius")] [SerializeField]
    private int                         dropRadius;
    internal int                        DropRadius => dropRadius; 
    
    
    [SerializeField] private GameObject deathEffect;
    public GameObject DeathEffect => deathEffect;
}
