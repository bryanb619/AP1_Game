using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Akarya/AI/Chase Data")]
public class AiChaseData : ScriptableObject
{
    #region Combat
    // Combat -------------------------------------------------------------------->
    [Header("Combat")]
    [FormerlySerializedAs("_attackDist")]
    [SerializeField]
            private float               attackDist = 2.5f;
            public float                AttackDist => attackDist;

        [FormerlySerializedAs("_attackEffect")] [SerializeField] 
            private GameObject          attackEffect;
            public GameObject           AttackEffect => attackEffect;

        [FormerlySerializedAs("_damageEffectTime")] [SerializeField]
            private float               damageEffectTime; 
            public float                DamageTime => damageEffectTime;

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

        [FormerlySerializedAs("a_damage")]
        [Tooltip("Attack damage")]
        [Range(0, 30)]
        [SerializeField]
            private int                 aDamage = 10;
            public int                  Damage => aDamage;

        [Tooltip("Agent Stop distance from player position")]
        [SerializeField]
            private float               stopDistance;
            public float                StopDistance => stopDistance; 

        [Tooltip("attack value is set to previuos option, use this option to allow some more space so action can be completed succesfuly")]
        [SerializeField] 
            private float               attackDistOffset = 4F; 
            public float                AttackDistOffset => attackDistOffset;


    //[SerializeField]
    //private float attackSpeed = 4F;
    //public float AttackSpeed => attackSpeed;

        [FormerlySerializedAs("_attackTimer")]
        [Tooltip("Set this value so attacks are not duplicated")]
        [SerializeField]
            private float               attackTimer = 0.7f;
            public float                AttackTimer => attackTimer;

        // Chance attack //
        [FormerlySerializedAs("_percentage")]
        [Header("Attack Nº2 / Chance Attack")]
        [Tooltip("Higher the number, more possible it is to use attack nº2")]
        [Range(0f, 1f)]
        [SerializeField]
            private float               percentage;
            public float                Percentage => percentage;

        [FormerlySerializedAs("b_damage")] [SerializeField]
            private int                 bDamage;
            public int                  BDamage => bDamage;

        [FormerlySerializedAs("s_attackEffect")] [SerializeField]
            private GameObject          sAttackEffect; 
            public GameObject           SAttackEffect => sAttackEffect; 

            
        // Special attack //
        [Header("Special Attack")]
        [SerializeField]
            private GameObject          specialAttackEffect;
            public GameObject           SpecialAttackEffect => specialAttackEffect;


        [FormerlySerializedAs("s_damage")]
        [Tooltip("Special attack damage on hit")]
        [Range(0, 50)]
        [SerializeField]
            private int                 sDamage = 20;
            public int                  SDamage => sDamage;

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

    #endregion

    #region Movement
    // AI Walk Speed per state -------------------------------------------------------------->

    [Header("Movement Speed")]
    [SerializeField]    private float patrolSpeed; 
                        public float PatrolSpeed => patrolSpeed;

    [SerializeField]    private float attackSpeed;
                        public float AttackSpeed => attackSpeed;

    [SerializeField]    private float otherSpeed;
                        public float OtherSpeed => patrolSpeed;

    #endregion

    #region FOV
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

    #endregion

    #region Health
    // HEALTH -------------------------------------------------------------------->
    [Header("Health")]
        [Tooltip("Set AI Health")]
        [Range(0, 9999)]
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
    #endregion

    #region Damage & Death
    // DAMAGE--------------------------------------------------------------------> 

    // Stunned //
    [FormerlySerializedAs("_stunnedTime")]
    [Header("Stunned")]
        [SerializeField] 
            private float               stunnedTime;
            public float                StunnedTime => stunnedTime;     
        [FormerlySerializedAs("_stunnedChance")] [SerializeField]
            private float               stunnedChance;
            public float                SunnedChance => stunnedChance;


    //  Death & Loot --------------------------------------------------------------------> 

        [FormerlySerializedAs("_deathEffect")]
        [Header("Death")]
        [SerializeField] 
            private GameObject          deathEffect; 
            public GameObject           DeathEffect => deathEffect;  


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

        [FormerlySerializedAs("_spawnHealth")]
        [Tooltip("if checked, AI will spawn health drops upon death")]
        [SerializeField]
            private bool                spawnHealth;
            public bool                 SpawnHealth => spawnHealth;    

        [FormerlySerializedAs("_healthDrop")] [SerializeField] 
            private GameObject          healthDrop;
            public GameObject           HealthDrop => healthDrop;

        [FormerlySerializedAs("_healthUnits")]
        [Range(0, 10)]
        [SerializeField]
            private int                 healthUnits; 
            public int                  HealthUnits => healthUnits;

        [FormerlySerializedAs("_spawnMana")] [SerializeField]
            private bool                spawnMana;
            internal bool               SpawnMana => spawnMana;

        // Mana // 
        [FormerlySerializedAs("_manaDrop")] [SerializeField]
            private GameObject          manaDrop;
            public GameObject           ManaDrop => manaDrop;

        [FormerlySerializedAs("_manaItems")]
        [Range(0, 10)]
        [SerializeField]
            private int                 manaItems;
            internal int                ManaItems => manaItems;

    #endregion

    #region Sounds 
    // Sound -------------------------------------------------------------------->
    [FormerlySerializedAs("_grunt")]
    [Header("Sound")]
       
        [SerializeField]
            private EventReference      grunt;
            public EventReference       Grunt => grunt;  

        [FormerlySerializedAs("_scream")] [SerializeField]
            private EventReference      scream;
            public EventReference       Scream => scream;

        [FormerlySerializedAs("_etc")] [SerializeField]
            private EventReference      etc;
            public EventReference       Etc => etc;


    #endregion

    #region Other
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
    #endregion
}