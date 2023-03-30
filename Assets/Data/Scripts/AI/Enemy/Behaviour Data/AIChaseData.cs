using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Akarya/AI/Chase Data")]
public class AIChaseData : ScriptableObject
{
    // ATTACK //
    [Header("Attack")]

    [Tooltip("Minimum distance to detect player or projectiles")]
    [Range(0f, 10f)]
    [SerializeField] private float minDist = 7f;
    public float MinDist => minDist;

    [Tooltip("Attack rate")]
    [Range(1f, 5f)]
    [SerializeField] private float attackRate = 2f;
    public float AttackRate => attackRate;

    [Tooltip("Attack damage")]
    [Range(0, 30)]
    [SerializeField] private int damage = 10;
    public int Damage => damage;

    [Tooltip("Agent Stop distance from player position")]
    [SerializeField]
    private float stopDistance= 3.9F;
    public float StopDistance => stopDistance; 

    [Tooltip("attack value is set to previuos option, use this option to allow some more space so action can be completed succesfuly")]
    [SerializeField] private float attackDistOffset = 4F; 
    public float AttackDistOffset => attackDistOffset;

    [SerializeField]
    private float attackSpeed = 4F;
    public float AttackSpeed => attackSpeed;

    // ATTACK 2 //
    [Header("Attack Nº2")]
    [Tooltip("Higher the number, more possible it is to use attack nº2")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _percentage;
    public float Percentage => _percentage;


    //[SerializeField]
    //private float cooldownSpeed = 3F;
    //public float CooldownSpeed => cooldownSpeed;    

    // Special attack //
    [Header("Special Attack")]
    [Tooltip("Special attack damage on hit")]
    [Range(0, 50)]
    [SerializeField]
    private int specialDamage = 20;
    public int SpecialDamage => specialDamage;

    [Range(0, 100)]
    [SerializeField]
    [Tooltip("Starting ability value")]
    private float currentAbilityValue = 40f;
    public float CurrentAbilityValue => currentAbilityValue;

    [Tooltip("Increase value per frame")]
    [Range(0, 20)]
    [SerializeField]
    private float abilityIncreasePerFrame = 4f;
    public float AbilityIncreasePerFrame => abilityIncreasePerFrame;

    // FOV //
    [Header("FOV")]
    [Tooltip("Radius of FOV")]
    [Range(10f, 150f)]
    [SerializeField] private float radius;
    public float Radius => radius;

    [Tooltip("Angle of FOV")]
    [Range(0f, 360f)]
    [SerializeField] private float angle;
    public float Angle => angle;

    [Tooltip("Select target Layer masks so AI can identify targets in FOV")]
    [SerializeField] private LayerMask targetMask;
    public LayerMask TargetMask => targetMask;

    [Tooltip("Select target Layer masks so AI can identify obstrutions in FOV")]
    [SerializeField] private LayerMask obstructionMask;
    public LayerMask ObstructionMask => obstructionMask;

    // COVER
    [Header("Cover")]
    [Tooltip("Hidable masks layers for AI (Walls is one by default")]
    [SerializeField] private LayerMask hidableLayers;
    public LayerMask HidableLayers => hidableLayers;

    [Range(0f, 15f)]
    [Tooltip("Minimum distance to find cover")]
    [SerializeField] private float minDistInCover = 12f;
    public float MindistIncover => minDistInCover;

        // The speed at which the AI character moves
    [Tooltip("Speed of AI agent in seeking for cover")]
    [Range(1f, 15f)]
    [SerializeField] private float coverMoveSpeed = 5f;
    public float CoverMoveSpeed => coverMoveSpeed;

        // The distance at which the AI character starts fleeing from the player
    [Tooltip("Minimal distance AI seeks of player in cover state")]
    [Range(1f, 50f)]
    [SerializeField] private float fleeDistance = 15f;
    public float FleeDistance => fleeDistance;

    // HEALTH //
    [Header("Health")]
    [Tooltip("Set AI Health")]
    [Range(0, 150)]
    [SerializeField] private int health;
    public int Health => health;

    [Tooltip("Heal rate, the higher the faster it will restore")]
    [Range(0f, 30f)]
    [SerializeField] private float healthRegen;
    public float HealthRegen => healthRegen;

    [Tooltip("Maximum health regeneration")]
    [Range(0f, 150f)]
    [SerializeField] private float maxHealthRegen;
    public float MaxHealthRegen => maxHealthRegen;

    [Header("AI Weakness")]
    [Tooltip("Set AI weaknesses")]
    [SerializeField] private bool _ice;
    public bool Ice => _ice;

    [SerializeField] private bool fire;
    public bool Fire => fire;

    [SerializeField] private bool thunder;
    public bool Thunder => thunder;


    // GEM // 
    [Header("Gem")]
    [Tooltip("Set true to spawn Gem")]
    [SerializeField] private bool gemSpawnOnDeath;
    public bool GemSpawnOnDeath => gemSpawnOnDeath;

    [Tooltip("Select Gem prefab")]
    [SerializeField] private GameObject gem;
    public GameObject Gem => gem;

    [Tooltip("Select Spawn chance")]
    [Range(0f,100f)]
    [SerializeField] private float spawnChance;
    public float SpawnChance => spawnChance;    
}