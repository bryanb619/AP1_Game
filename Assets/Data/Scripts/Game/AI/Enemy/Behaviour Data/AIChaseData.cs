using UnityEngine;
using FMODUnity; 

[CreateAssetMenu(menuName = "Akarya/AI/Chase Data")]
public class AIChaseData : ScriptableObject
{
    // Combat -------------------------------------------------------------------->
    [Header("Combat")]

        [SerializeField]
            private float               _attackSpeed = 1.8f;
            public float                AttackSpeed => _attackSpeed; 

        [SerializeField]
            private float               _attackDist = 2.5f;
            public float                AttackDist => _attackDist;

        [SerializeField] 
            private GameObject          _attackEffect;
            public GameObject           AttackEffect => _attackEffect;

        [SerializeField]
            private float               _damageEffectTime; 
            public float                DamageTime => _damageEffectTime;

        [Tooltip("Minimum distance to detect player or projectiles")]
        [Range(0f, 10f)]
        [SerializeField]
            private float               minDist = 7f;
            public float                MinDist => minDist;

        [Tooltip("Attack rate")]
        [Range(1f, 5f)]
        [SerializeField] 
            private float               attackRate = 2f;
            public float                AttackRate => attackRate;

        [Tooltip("Attack damage")]
        [Range(0, 30)]
        [SerializeField]
            private int                 a_damage = 10;
            public int                  Damage => a_damage;

        [Tooltip("Agent Stop distance from player position")]
        [SerializeField]
            private float               stopDistance = 5F;
            public float                StopDistance => stopDistance; 

        [Tooltip("attack value is set to previuos option, use this option to allow some more space so action can be completed succesfuly")]
        [SerializeField] 
            private float               attackDistOffset = 4F; 
            public float                AttackDistOffset => attackDistOffset;


    //[SerializeField]
    //private float attackSpeed = 4F;
    //public float AttackSpeed => attackSpeed;

        [Tooltip("Set this value so attacks are not duplicated")]
        [SerializeField]
            private float               _attackTimer = 0.7f;
            public float                AttackTimer => _attackTimer;

        // Chance attack //
        [Header("Attack Nº2 / Chance Attack")]
        [Tooltip("Higher the number, more possible it is to use attack nº2")]
        [Range(0f, 1f)]
        [SerializeField]
            private float               _percentage;
            public float                Percentage => _percentage;

        [SerializeField]
            private int                 b_damage;
            public int                  B_damage => b_damage;

        [SerializeField]
            private GameObject          s_attackEffect; 
            public GameObject           S_attackEffect => s_attackEffect; 


    //[SerializeField]
    //private float cooldownSpeed = 3F;
    //public float CooldownSpeed => cooldownSpeed;    

    // Special attack //
        [Header("Special Attack")]
        [SerializeField]
            private GameObject          specialAttackEffect;
            public GameObject           SpecialAttackEffect => specialAttackEffect;


        [Tooltip("Special attack damage on hit")]
        [Range(0, 50)]
        [SerializeField]
            private int                 s_damage = 20;
            public int                  S_damage => s_damage;

        [Range(0, 100)]
        [SerializeField]
        [Tooltip("Starting ability value")]
            private float               currentAbilityValue = 40f;
            public float                CurrentAbilityValue => currentAbilityValue;

        [Tooltip("Increase value per frame")]
        [Range(0, 20)]
        [SerializeField]
            private float               abilityIncreasePerFrame = 5f;
            public float                AbilityIncreasePerFrame => abilityIncreasePerFrame;

    // FOV -------------------------------------------------------------------->
        [Header("FOV")]
        [Tooltip("Radius of FOV")]
        [Range(10f, 150f)]
        [SerializeField] 
            private float               radius;
            public float                Radius => radius;

        [Tooltip("Angle of FOV")]
        [Range(0f, 360f)]
        [SerializeField]
            private float               angle;
            public float                Angle => angle;

        [Tooltip("Select target Layer masks so AI can identify targets in FOV")]
        [SerializeField] 
            private LayerMask           targetMask;
            public LayerMask            TargetMask => targetMask;

        [Tooltip("Select target Layer masks so AI can identify obstrutions in FOV")]
        [SerializeField] 
            private LayerMask           obstructionMask;
            public LayerMask            ObstructionMask => obstructionMask;

       
    // HEALTH -------------------------------------------------------------------->
        [Header("Health")]
        [Tooltip("Set AI Health")]
        [Range(0, 1000)]
        [SerializeField] 
            private int                 health;
            public int                  Health => health;

        [Tooltip("Heal rate, the higher the faster it will restore")]
        [Range(0f, 50f)]
        [SerializeField] 
            private float               healthRegen;
            public float                HealthRegen => healthRegen;

        [Tooltip("Maximum health regeneration")]
        [Range(0f, 350f)]
        [SerializeField] 
            private float               maxHealthRegen;
            public float                MaxHealthRegen => maxHealthRegen;

    /* [Header("AI Weakness")]
     [Tooltip("Set AI weaknesses")]
     [SerializeField] private bool _ice;
     public bool Ice => _ice;

     [SerializeField] private bool fire;
     public bool Fire => fire;

     [SerializeField] private bool thunder;
     public bool Thunder => thunder;

     */

    // DAMAGE--------------------------------------------------------------------> 

    // Stunned //
        [Header("Stunned")]
        [SerializeField] 
            private float               _stunnedTime = 3F;
            public float                StunnedTime => _stunnedTime;     
        [SerializeField]
            private float               _stunnedChance = 0.35F;
            public float                SunnedChance => _stunnedChance;


    //  Death & Loot --------------------------------------------------------------------> 

        [Header("Death")]
        [SerializeField] 
            private GameObject          _deathEffect; 
            public GameObject           DeathEffect => _deathEffect;  


        [Header("Loot")]
        [Tooltip("Set true to spawn Gem")]
        [SerializeField] 
            private bool                gemSpawnOnDeath;
            public bool                 GemSpawnOnDeath => gemSpawnOnDeath;
        
        [Tooltip("Select Gem prefab")]
        [SerializeField] 
            private GameObject          gem;
            public GameObject           Gem => gem;

        [Tooltip("Select Spawn chance")]
        [Range(0f,1f)]
        [SerializeField] 
            private float               spawnChance;
            public float                SpawnChance => spawnChance;

        [Tooltip("if checked, AI will spawn health drops upon death")]
        [SerializeField]
            private bool                _spawnHealth;
            public bool                 SpawnHealth => _spawnHealth;    

        [SerializeField] 
            private GameObject          _healthDrop;
            public GameObject           HealthDrop => _healthDrop;

        [Range(0, 10)]
        [SerializeField]
            private int                 _healthUnits = 4; 
            public int                  HealthUnits => _healthUnits;


    // Sound -------------------------------------------------------------------->
        [Header("Sound")]
       
        [SerializeField]
            private EventReference      _grunt;
            public EventReference       grunt => _grunt;  

        [SerializeField]
            private EventReference      _scream;
            public EventReference       Scream => _scream;

        [SerializeField]
            private EventReference      _etc;
            public EventReference       Etc => _etc;




    // COVER
    [Header("Cover")]
    [Tooltip("Hidable masks layers for AI (Walls is one by default")]
    [SerializeField]
    private LayerMask hidableLayers;
    public LayerMask HidableLayers => hidableLayers;

    [Range(0f, 15f)]
    [Tooltip("Minimum distance to find cover")]
    [SerializeField]
    private float minDistInCover = 12f;
    public float MindistIncover => minDistInCover;

    // The speed at which the AI character moves
    [Tooltip("Speed of AI agent in seeking for cover")]
    [Range(1f, 15f)]
    [SerializeField]
    private float coverMoveSpeed = 5f;
    public float CoverMoveSpeed => coverMoveSpeed;

    // The distance at which the AI character starts fleeing from the player
    [Tooltip("Minimal distance AI seeks of player in cover state")]
    [Range(1f, 50f)]
    [SerializeField]
    private float fleeDistance = 15f;
    public float FleeDistance => fleeDistance;

}