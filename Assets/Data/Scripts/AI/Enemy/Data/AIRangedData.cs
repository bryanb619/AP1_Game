using UnityEngine;

[CreateAssetMenu (menuName ="Akarya/AI/Ranged Data")]
public class AIRangedData : ScriptableObject
{
    [Header("Attack")]

    [Tooltip("Minimum distance to player or projectiles")]
    [Range(0f, 10f)]
    [SerializeField] private float minDist = 7f;
    public float MinDist => minDist;

    [Tooltip("Attack rate")]
    [Range(1f, 5f)]
    [SerializeField] private float attackRate = 2f;
    public float AttackRate => attackRate;

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
    [Tooltip("Value for every hit in player's health")]
    [SerializeField] private int damage;
    public int PlayerDamage => damage;    

    [Header("AI Weakness")]
    [Tooltip("Set AI weaknesses")]
    [SerializeField] private bool _ice;
    public bool Ice => _ice;

    [SerializeField] private bool fire;
    public bool Fire => fire;

    [SerializeField] private bool thunder;
    public bool Thunder => thunder;

    public GameObject projectile;
    public GameObject specialProjectile;

    [Header("Gem")]
    [Tooltip("Set true to spawn Gem")]
    [SerializeField] private bool gemSpawnOnDeath;
    public bool GemSpawnOnDeath => gemSpawnOnDeath;

    [Tooltip("Select Gem prefab")]
    [SerializeField] private GameObject gem;
    public GameObject Gem => gem;

    [Tooltip("Select Spawn chance")]
    [Range(0f, 100f)]
    [SerializeField] private float spawnChance;
    public float SpawnChance => spawnChance;
}
