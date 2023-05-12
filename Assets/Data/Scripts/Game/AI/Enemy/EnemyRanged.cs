#region Libs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibGameAI.FSMs;
using TMPro;

#if UNITY_EDITOR
using UnityEditor; 
#endif

#endregion

public class EnemyRanged : MonoBehaviour
{
    #region Variabless
    
    #region AI States

    private enum AI                             { 
        Guard, 
        Patrol,
        Attack,
        Cover, 
        Search, 
        GloryKill, 
        None }
    
    [SerializeField]    private AI               stateAI;

                        private enum HandleState                    { Stoped, None }
                        private HandleState                         _currentState;
                        private GameState                           _gamePlay;


    #endregion
                   
    #region Components
                        // Components ----------------------------------------------------------------------------------------------------->
        
                        // Reference to AI data
    [SerializeField]    private AIRangedData                        data;

                        // Reference to the state machine
                        private StateMachine                        _stateMachine;

                        // Reference to the NavMeshAgent
                        private NavMeshAgent                       _agent;
                        
                        private WarningSystemAI                     _warn;

    [SerializeField]    private Agents                              agentAI;

                        private AIHandler                           _handlerAI;
                        private bool                                _deactivateAI;
                        
                        private ObjectiveUI                         _objectiveUIScript;
                        
                        // animator
                        private Animator                            animator;

    // Reference to the Outline component & meshes
    [SerializeField]    private Outline                             outlineDeactivation;
    [SerializeField]    private SkinnedMeshRenderer                 enemyMesh;
                        private Color                               _originalColor;
                        
                        #endregion
                        
    #region Patrol
                        // Patrol Points
                        private int                                     _destPoint = 0;
    [SerializeField]    private Transform[]                             patrolPoints;
                        private float _patrolSpeed; 
    #endregion
    
    #region Cover

    // hide code
                            
    [Header("Hide config")]
    private Collider[]                             _colliders = new Collider[10];

    [Range(-1, 1)]
    [SerializeField]        private float                                   hideSensitivity = 0;
    
    [Range(0.01f, 1f)]
    [SerializeField]        private float                                   UpdateFrequency = 0.65f;

    [SerializeField]        private LayerMask                               HidableLayers;

    [Range(0, 5f)]
    [SerializeField]        private float                                   MinObstacleHeight = 0f;

    public SceneChecker                             LineOfSightChecker;

    private Coroutine                               _movementCoroutine;
    #endregion
    
    #region Combat
                        // Combat  ---------------------------------------------------------------------------------------------------->

        // Attack 
                            private float _attackSpeed; 

        [SerializeField]    private Transform                           shootPos;

                            private GameObject                          _bullet, _randomBullet, _specialPower;

                            private float                               _fireRate;
                            private float                               _nextFire;
                            private float                               _percentage;

                            
        // special ability 

                            private const float                         AbilityMaxValue = 100F;
                            private float                               _currentAbilityValue;
                            private float                               _abilityIncreasePerFrame;

                            private GameObject                          _targetEffect;
                            
                            
                            // COMBAT //
                            private float                                   damageEffectTime;
                            
                            // damage over time variables
                            private float                                    _damageOverTime = 2f;
                            private float                                    _durationOfDOT = 10f;

                            // stunned variables
                            private float                                     _stunnedTime = 1.5f;
        
    
                            
        // FOV -------------------------------------------------------------------------------------------------------->
               
        [Range(10, 150)]
        [SerializeField]    private float                                   radius;
                            public float                                    Radius => radius;
        [Range(50, 360)]
        [SerializeField]    private float                                   angle;
                            public float                                    Angle => angle;

        [SerializeField]    private LayerMask                               targetMask;
        [SerializeField]    private LayerMask                               obstructionMask;
        [SerializeField]    private Transform                               fov;
                            public Transform                                EefovTransform => fov; // Enemy Editor FOV

                            private bool                                    _canSeePlayer;
                            public bool                                     CanSee => _canSeePlayer;
                            
    // Drops & Death -------------------------------------------------------------------------------------------------->
                            private GameObject                              _death;

                            private bool                                    _gemSpawnOnDeath;
                            private GameObject                              _gemPrefab;

                            private bool                                    _spawnHealth;
                            private int                                     _healthItems;
                            private GameObject                              _healthDrop;

                            private bool                                    _spawnMana; 
                            private int                                     _manaItems;
                            private GameObject                              _manaDrop;
                            private int                                     _dropRadius; 

                            #endregion

    #region  Health & UI
                            // Health --------------------------------------------------------------------------------------------------------->

        // UI
        [Header("UI ")]
        [SerializeField]    private Slider                                  healthSlider;
                            private float                                   _health;
                            
                            public int                                      damageBoost = 0;
                            
    [SerializeField]        private Slider          abilitySlider;

                            private ValuesTextsScript                        _valuesTexts;

                            
        // References to enemies
        [SerializeField]    private GameObject                              playerObject;

                            private Transform                               _playerTarget;
                            public Transform                                PlayerTarget => _playerTarget;

                            private PlayerHealth                            _player; 
                            
                            #endregion
                            
    // Debug // 
    [SerializeField]        private bool                                       showExtraGizmos; 
    
    #endregion

    private void Awake()
    {
        GameManager.OnGameStateChanged
    }
    
    private 

    #region Start
    void Start()
    {
        GetComponents();
        GetProfile();
        StateCheck();
    }

    
#region Components Sync
    private void GetComponents()
    {
        _agent              = GetComponent<NavMeshAgent>();
        _warn               = GetComponent<WarningSystemAI>();
        _handlerAI          = GetComponent<AIHandler>();

        agentAI            = GetComponentInChildren<Agents>();
        animator           = GetComponentInChildren<Animator>();
        healthSlider       = GetComponentInChildren<Slider>();
        //enemyMesh           = GetComponentInChildren<SkinnedMeshRenderer>();

        LineOfSightChecker  = GetComponentInChildren<SceneChecker>();

        
        _player             = FindObjectOfType<PlayerHealth>();
        playerObject        = GameObject.Find("Player");
        _playerTarget       = PlayerObject.transform;
        //_shooterScript      = PlayerObject.GetComponent<Shooter>();

        _valuesTexts         = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
    }
    #endregion

    #region Profile Sync
    private void GetProfile()
    {
       
        // HEALTH //
        _health = data.Health;
       
        // ATTACK //
        _fireRate = data.AttackRate;
        
        //_minDist = data.MinDist;

        _percentage = data.Percentage; 

        // Special attack Ability

        _currentAbilityValue = data.CurrentAbilityValue;
        _abilityIncreasePerFrame = data.AbilityIncreasePerFrame;
        //specialDamage = data.SpecialDamage;

        // projectiles //

        _bullet          = data.ProjectileA;
        _randomBullet    = data.R_projectile;
        _specialPower    = data.S_Projectile;

        // cover //
        //fleeDistance = data.FleeDistance; 
        
        // Death & Loot //

        _gemPrefab = data.Gem;

        _spawnHealth = data.SpawnHealth;    
        _healthDrop = data.HealthDrop;
        _healthItems = data.HealthItems;    

        _spawnMana = data.SpawnMana;
        _manaDrop = data.ManaDrop;
        _manaItems = data.ManaItems;
        _dropRadius = data.DropRadius;  
        
        // UI //
        healthSlider.value = _health;
        abilitySlider.value = _currentAbilityValue; 

    }
    #endregion

    #region States
    private void GetStates()
    {
        // Create the states
        State onGuardState = new State("Guard" ,null);

        State PatrolState = new State("On Patrol", PatrolAction);

        State ChaseState = new State("Fight",AttackAction);
        
        // Add the transitions

        // GUARD -> CHASE
        onGuardState.AddTransition(
            new Transition(
                () => stateAI == AI.Attack,
                //() => Debug.Log(" GUARD -> CHASE"),
                ChaseState));

        // CHASE->PATROL
        ChaseState.AddTransition(
            new Transition(
                () => stateAI == AI.Patrol,
                PatrolState));
        
        //  PATROL -> CHASE 
        PatrolState.AddTransition(
            new Transition(
                () => stateAI == AI.Attack,
                ChaseState));
        
        //state machine
        _stateMachine = new StateMachine(PatrolState);
    }
    #endregion
    
    #region State Set 
    
    private void StateCheck()
    {
        switch (stateAI)
        {
            case AI.Patrol:
            {
                SetPatrol();
                break;
            }
            case AI.Attack:
            {
                SetAttack();
                break; 
            }
        }
    }
    private void SetPatrol()
    {
        _agent.speed = _patrolSpeed;
    }
    
    private void SetAttack()
    {
        _agent.speed = _attackSpeed; 
    }
    
    #endregion
    #endregion
    #region Update
    // Request actions to the FSM and perform them
    private void Update()
    {
        //UpdateAI();
    }

    #region Patrol
    private void PatrolAction()
    {
        
        
    }
    #endregion


    #region Attack
    private void AttackAction()
    {
        
    }


    private void Attack()
    {
        
    }
    #endregion
    #endregion
    
    
    
}
