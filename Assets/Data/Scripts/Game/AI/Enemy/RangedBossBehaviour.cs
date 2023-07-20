#region Libs

using System;
using System.Collections;
using Data.Scripts.Game.AI.Enemy;
using UnityEngine;
using UnityEngine.AI;
using LibGameAI.FSMs;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using State = LibGameAI.FSMs.State;
using StateMachine = LibGameAI.FSMs.StateMachine;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

public class RangedBossBehaviour : MonoBehaviour
{
    #region Variables

    // Systems
    [Header("AI Profile")] [SerializeField]
    private AIRangedData data;

    // AI 
    // AI states
    private enum Ai
    {
        Patrol,
        Attack,
        Cover
    }

    [Header("AI State")] [SerializeField] private Ai stateAi;

    // state machine 
    private StateMachine _fsm;

    // nav mesh agent
    public NavMeshAgent agent;

    private float _pathUpdateDeadLine = 0.2f;

    // Ai Shoot
    private AiShoot _aiShoot;

    // performance handler
    private AIHandler _aiHandler;

    private enum HandleState
    {
        Stoped,
        None
    }

    private HandleState _currentState;

    private EnemyType _enemyType;

    // other components ---------------------------------------------------------------------------------------->

    // objectives UI 
    private ObjectiveUI _objectiveUiScript;
    [SerializeField] private Outline outlineDeactivation;
    private Animator _animator;
    private SkinnedMeshRenderer _mesh;
    private Color _color;

    // Game State
    private GameState _gameState;

    // Player Refs ------------------------------------------------------------------------------------------------>

    private GameObject _player;
    private Transform _playerTarget;
    private Shooter _shooterScript;
    [SerializeField] private LayerMask playerLayer;

    // Combat ----------------------------------------------------------------------------------------------------->

    // General combat variables

    private bool _canAttack;
    private float _safeDistance;

    private float _fireRate = 2f;
    private float _nextFire;

    // random attack chance
    private float _percentage;
    // Boss attack

    [Header("Boss Events")] [SerializeField]
    private int[] healthEvents;

    [Header("Chase AI spawned")] 
    [SerializeField] private int[] chaseCount; 
    [Header("ranged AI spawned")] 
    [SerializeField] private int[]   rangedCount;
    
    [SerializeField] private float effectTime, spawnTime, minRange, maxRange;

    private bool _runningAiSpawn;
    [SerializeField] private GameObject aiSpawnEffect, chaseAi, rangedAi;

    private const float AbilityMaxValue = 100f;
    private float _currentAbilityValue = 100;
    private float _abilityIncreasePerFrame;
    private bool _canSpecialAttack;

    private float _minRange, _maxRange;
    private GameObject _teleportEffect;
    private float _nextTeleport;
    private float _teleportRate;

    private int _areaAttackDamage;
    private float _areaAttackRange;
    private float _nextSpecialAttack;
    private float _specialAttackRate = 2f;

    private bool _canIncreaseAbility;

    private float _cooldownTp;
    private float _currentTpValue;
    private float _tpMaxValue;
    private float _tpIncreasePerFrame;
    private float _neededTpValue;
    private bool _canCdIncrease;

    [SerializeField] private float firstAttack;

    // projectiles
    private GameObject _projectile,
        _randomProjectile,
        _specialProjectile,
        _areaAttack;

    // Health/Death ----------------------------------------------------------------------------------------------->

    // HEALTH
    [SerializeField] private AIHealth healthBar;

    private float _health;
    private float _maxHealth;
    private float _healthIncreasePerFrame;

    // DEATH 
    private GameObject _deathEffect;
    private bool _isDead;

    // others

    private bool _stfsEffect = false;

    // Drops/Loot ------------------------------------------------------------------------------------------------->

    private bool _spawningGems;

    // Health
    private GameObject _healthDrop;
    private int _healthItems;
    private bool _spawnHealth;

    // Mana
    private GameObject _manaDrop;
    private int _manaItems;
    private bool _spawnMana;

    // Drop Radius
    private float _dropRadius;
    [SerializeField] private Transform dropPos;


    // UI --------------------------------------------------------------------------------------------------------->

    private TextMeshProUGUI _damageText;
    [SerializeField] private Slider abilitySlider;
    private ValuesTextsScript _valuesTexts;

    private float _stunnedTime = 0.5f;


    // Sound ------------------------------------------------------------------------------------------------------>
    [SerializeField] private EventReference attackSound,
        areaAttackSound,
        specialAttackSound,
        teleportSound,
        deathSound;

#if UNITY_EDITOR
    [SerializeField] private bool showGizmos;
    private bool _showIsActive;
#endif

    #endregion

    //---------------------------------------------------------------------------------------------------------------->

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        _enemyType = data.enemyType;
        _currentState = HandleState.None;
        _canAttack = true;
        _canIncreaseAbility = true;

        GetAiComponents();
        ProfileSync();
        StateSet();
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
            {
                _gameState = state;
                ResumeAi();
                break;
            }

            case GameState.Paused:
            {
                _gameState = state;
                PauseAi();
                break;
            }

            case GameState.Death:
            {
                _gameState = state;
                PauseAi();
                break;
            }
        }
    }

    private void ResumeAi()
    {
        if (_currentState != HandleState.None) return;

        if (agent.enabled)
            // resume agent
            HandleStateAi(false);
    }

    private void PauseAi()
    {
        if (_currentState != HandleState.None) return;

        //transform.rotation = Quaternion.identity;

        if (agent.enabled)
            //stop agent
            HandleStateAi(true);
    }

    private void HandleStateAi(bool stop)
    {
        switch (stop)
        {
            case true:
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                break;
            }
            case false:
            {
                agent.isStopped = false;
                break;
            }
        }
    }
    
    private void GetAiComponents()
    {
        // AI components
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.angularSpeed = 0f;
        _aiShoot = GetComponentInChildren<AiShoot>();
        _aiHandler = GetComponent<AIHandler>();
        //_healthBar                   = GetComponentInChildren<AiHealth>();

        // mesh 
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _color = _mesh.material.color;

        // animator
        _animator = GetComponentInChildren<Animator>();

        // UI 
        _valuesTexts = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();

        // Player 
        _player = GameObject.Find("Player");
        _playerTarget = _player.transform;
        _shooterScript = _player.GetComponent<Shooter>();

        _objectiveUiScript = FindObjectOfType<ObjectiveUI>();
        _damageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ProfileSync()
    {
        agent.avoidancePriority = 50;

        // combat 
        _safeDistance = data.MinDist;

        _fireRate = data.AttackRate;
        _percentage = data.Percentage;


        // Special attack Ability

        _currentAbilityValue = data.CurrentAbilityValue;
        abilitySlider.value = _currentAbilityValue;
        abilitySlider.maxValue = _currentAbilityValue;

        _abilityIncreasePerFrame = data.AbilityIncreasePerFrame;

        _areaAttackDamage = data.AreaDamageAttack;
        _areaAttackRange = data.AreaDamageRadius;

        _areaAttack = data.AreaAttack;


        // Drops ------------------------------------------------------------------------------------------------------>

        // Health Standard
        _spawnHealth = data.SpawnHealth;
        _healthDrop = data.HealthDrop;
        _healthItems = data.HealthItems;

        // Mana Standard
        _spawnMana = data.SpawnMana;
        _manaDrop = data.ManaDrop;
        _manaItems = data.ManaItems;

        // special Health
        // Special Mana

        // Drop Radius
        _dropRadius = data.DropRadius;

        // Health/Death ----------------------------------------------------------------------------------------------->
        _health = data.Health;
        _healthIncreasePerFrame = data.HealthRegen;
        _maxHealth = data.MaxHealthRegen;
        _deathEffect = data.DeathEffect;

        // projectiles ------------------------------------------------------------------------------------------------>

        _projectile = data.NProjectile;
        _randomProjectile = data.RProjectile;


        // Boss 
        if (_enemyType == EnemyType.Boss)
        {
            _specialProjectile = data.SProjectile;

            _minRange = data.TeleportMinRange;
            _maxRange = data.TeleportMaxRange;
            _teleportRate = data.TeleportTime;
            _teleportEffect = data.TeleportEffect;
            _currentTpValue = data.CurrentTp;
            _tpMaxValue = data.TpMaxValue;
            _tpIncreasePerFrame = data.TpIncreasePerFrame;

            _cooldownTp = data.CooldownTp;
            _canCdIncrease = true;

#if UNITY_EDITOR
            Debug.Log("Boss");
#endif
        }
    }

    private void StateSet()
    {
        // Create the states
        var patrolState = new State("Patrol", null);
        var chaseState = new State("Chase", Engage);

        //  PATROL -> CHASE 
        patrolState.AddTransition(
            new Transition(
                () => stateAi == Ai.Attack,
                chaseState));

        // CHASE -> PATROL
        chaseState.AddTransition(
            new Transition(
                () => stateAi == Ai.Patrol,
                patrolState));

        // Create the state machine
        _fsm = new StateMachine(chaseState);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // set health at start
        healthBar.HealthValueSet(_health);

        // set slider max value
        abilitySlider.maxValue = 100;
        // set slider starting value value
        abilitySlider.value = _currentAbilityValue;
    }


    // Update is called once per frame
    private void Update()
    {
        UpdateAi();

#if UNITY_EDITOR
        EventTest();
#endif
    }

    #region AI Handling

    private void UpdateAi()
    {
        switch (_aiHandler.activeAi)
        {
            case AIHandler.ActiveAi.Active:
            {
#if UNITY_EDITOR
                if (!_showIsActive)
                {
                    _showIsActive = true;
                    print("active");
                }
#endif

                ActiveAi();
                break;
            }
            case AIHandler.ActiveAi.Inactive:
            {
                InactiveAi();
                break;
            }
        }
    }

    private void ActiveAi()
    {
        switch (_gameState)
        {
            case GameState.Gameplay:
            {
                Gameplay();
                break;
            }
            case GameState.Paused:
            {
                InactiveAi();
                break;
            }
            case GameState.Death:
            {
                _gameState = GameState.Death;
                stateAi = Ai.Patrol;
                InactiveAi();
                break;
            }
        }
    }

    private void InactiveAi()
    {
        if (!agent.enabled) return;

        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    private void Gameplay()
    {
        
        var actions = _fsm.Update();
        actions?.Invoke();
    }

    #endregion

    private void EventTest()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X)) CombatEvent(2, 4);
#endif
    }

    #region Actions

    private void Patrol()
    {
    }

    private void Engage()
    {
        if ((_playerTarget.transform.position - transform.position).magnitude <= 1.5f)
        {
            agent.radius = 0.1f;
            if ((_playerTarget.transform.position - transform.position).magnitude <= 1f) agent.enabled = false;
        }

        if (_canAttack)
        {
            BossAttack();
            UpdateRotation();
        }

        CoolDownTp();
    }

    private void BossAttack()
    {
        switch (_canSpecialAttack)
        {
            case true:
            {
                if ((_playerTarget.transform.position - transform.position).magnitude <= _safeDistance)
                {
                    PauseAi();

                    if (Time.time > _nextSpecialAttack)
                    {
                        AreaAttack();
                        _nextSpecialAttack = Time.time + _specialAttackRate;
                    }
                }

                if ((_playerTarget.transform.position - transform.position).magnitude >= _safeDistance) //
                {
                    //PauseAi();
                    ResumeAi();

                    if (Time.time > _nextSpecialAttack)
                    {
                        SpecialAttack();
                        _nextSpecialAttack = Time.time + _specialAttackRate;
                    }
                }

                break;
            }

            case false:
            {
                if ((_playerTarget.transform.position
                     - transform.position).magnitude >= 15f)
                {
                    if (_canAttack)
                    {
                        ResumeAi();
                        UpdatePath();
                    }
                }
                else if ((_playerTarget.transform.position
                          - transform.position).magnitude <= 9) //_attackRange && !_canSpecialAttack) 
                {
                    if (Time.time > _nextTeleport && _currentTpValue >= 3)
                    {
                        GenerateRandomPos();
                        _nextTeleport = Time.time + _teleportRate;
                    }

                    else
                    {
                        if ((_playerTarget.transform.position
                             - transform.position).magnitude <= 3)
                            if (_canAttack)
                                if (Time.time > _nextFire)
                                {
                                    AreaAttack();
                                    _nextFire = Time.time + _fireRate;
                                }
                    }
                }

                else if ((_playerTarget.transform.position - transform.position).magnitude >= 13) // valor mais alto 
                {
                    //var position = _playerTarget.position;

                    PauseAi();

                    if (_canAttack && !_canSpecialAttack) Attack();
                }

                CoolDoownPower();

                break;
            }
        }
    }

    private void SpecialAttack()
    {
        _aiShoot.Shoot(_playerTarget, _specialProjectile);

        // _currentAbilityValue = 0;
        _currentAbilityValue -= 10; 
        //abilitySlider.value = _currentAbilityValue;
        //abilitySlider.value = _currentAbilityValue;
        _canSpecialAttack = false;

        _animator.SetBool("isAttacking", false);
        // StartCoroutine(SpecialAttackTimer());

#if UNITY_EDITOR

        const string debugAttack = "<size=14><color=purple>";
        const string closeAttack = "</color></size>";
        Debug.Log(debugAttack + "Target Attack: " + closeAttack);

#endif
    }

    private void AreaAttack()
    {
        var hitEnemies = Physics.OverlapSphere(transform.position, _areaAttackRange, playerLayer);

        foreach (var col in hitEnemies)
        {
            var player = col.GetComponent<PlayerHealth>();

            if (player == null) return;

            Instantiate(_areaAttack, transform.position, _areaAttack.transform.rotation);
            player.TakeDamage(_areaAttackDamage);

            _currentAbilityValue = 0;
            abilitySlider.value = _currentAbilityValue;
            _canSpecialAttack = false;
#if UNITY_EDITOR
            const string debugColor = "<size=14><color=red>";
            const string closeDebug = "</color></size>";
            Debug.Log(debugColor + "Area Attack" + closeDebug);
#endif
            //_enemyAnimationHandler.RecievePlayerCollision(player);
            _animator.SetTrigger("Attack2");
        }
    }

    private void Attack()
    {
        if ((_playerTarget.transform.position - transform.position).magnitude >= _safeDistance)
        {
            if (Time.time > _nextFire)
            {
                //float randomFloat = UnityEngine.Random.value;

                //print("percentage is: "+randomPercentage);

                if (Random.value < _percentage && _canAttack)
                {
                    _animator.SetBool("isAttacking", true);
                    RandomAttack();
                }

                else if (_canAttack)
                {
                    NormalAttack();
                }

                _nextFire = Time.time + _fireRate;
            }
        }

        else
        {
            _animator.SetBool("isAttacking", false);
        }
    }

    private void UpdatePath()
    {
        if (agent.enabled)
        {
            if (!(Time.time >= _pathUpdateDeadLine)) return;

            // update time
            _pathUpdateDeadLine = Time.time + 0;
            // set destination
            agent.SetDestination(_playerTarget.position);
        }
    }

    private void UpdateRotation()
    {
        // get player direction
        var direction = _playerTarget.position - transform.position;
        //rotation 
        var rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // apply rotation to AI 
        transform.rotation = rotation;
    }


    private void NormalAttack()
    {
        _animator.SetBool("isAttacking", true);

        _aiShoot.Shoot(_playerTarget, _projectile);

        //StartCoroutine(AttackTimer());

#if UNITY_EDITOR
        const string debugAttack = "<size=12><color=green>";
        const string closeAttack = "</color></size>";
        Debug.Log(debugAttack + "Normal Attack: " + closeAttack);
#endif
    }


    private void RandomAttack()
    {
        _aiShoot.Shoot(_playerTarget, _randomProjectile);

        // StartCoroutine(AttackTimer()); 

#if UNITY_EDITOR
        const string debugAttack = "<size=12><color=yellow>";
        const string closeAttack = "</color></size>";
        Debug.Log(debugAttack + "chance attack: " + closeAttack);
#endif
    }

    private void CoolDoownPower()
    {
        if (_currentAbilityValue >= AbilityMaxValue)
        {
            _canSpecialAttack = true;
        }
        else
        {
            if (_canIncreaseAbility)
            {
                //print(_currentAbilityValue);
                _currentAbilityValue = Mathf.Clamp(_currentAbilityValue + _abilityIncreasePerFrame * Time.deltaTime,
                    0.0f, AbilityMaxValue);

                abilitySlider.value = _currentAbilityValue;
            }
        }

        //abilitySlider.value = _currentAbilityValue;
        //print(currentAbilityValue);
    }

    private void CoolDownTp()
    {
        if (_canCdIncrease)
            _currentTpValue = Mathf.Clamp(_currentAbilityValue + _tpIncreasePerFrame * Time.deltaTime, 0.0f,
                _tpMaxValue);
        //print(_currentAbilityValue);
    }
/*
    private IEnumerator SpecialAttackTimer()
    {
        bool isruning = false; 

        if(!isruning) 
        {
            isruning = true;
            _canPeformAttack = false;
            _canAttack = false;
            _activeDizzy = true; 
            yield return new WaitForSeconds(4f);
            _activeDizzy = false;
            _canAttack = true;
            _canPeformAttack = true;
            ResumeAgent();  
            isruning = false;
        }
        
    }
    */

/*

    private IEnumerator AttackTimer()
    {
        bool isruning = false;

        if (!isruning)
        {
            isruning = true;
            _canPeformAttack = false;
            yield return new WaitForSeconds(1f);
            _canPeformAttack = true;
            isruning = false;
        }
    }
    */

    private void GenerateRandomPos()
    {
        _currentTpValue -= _cooldownTp;
        TeleportProcess();
    }

    private void TeleportProcess()
    {
#if UNITY_EDITOR

        Debug.Log("teleport running");
#endif
        
        // Generate random position to TP 
        var randomDirection = Random.insideUnitSphere * Random.Range(_minRange, _maxRange);
        randomDirection += _playerTarget.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _maxRange, 1);
        var teleportPos = hit.position;

        // TP to random position
        StartCoroutine(Teleport(teleportPos));
    }

    private IEnumerator Teleport(Vector3 teleportPos)
    {
        // Instantiate TP effect
        Instantiate(_teleportEffect, teleportPos, _teleportEffect.transform.rotation);

        _canCdIncrease = false;
        _canIncreaseAbility = false;
        
        //  Disable animation attack
        
        _canAttack = false;
        
        // wait for spawn time done
        agent.enabled = false;
        yield return new WaitForSeconds(2f);

        // Instantiate spawn effect
        Instantiate(_teleportEffect, teleportPos, _teleportEffect.transform.rotation);
        yield return new WaitForSeconds(0.1f);
        // transform position =  spawn effect
        transform.position = teleportPos;
        
        
        
        // enable agent
        agent.enabled = true;
        _canAttack = true;
        _canIncreaseAbility = true;
        _canCdIncrease = true;
    }
    #endregion

    #region Health

    /// <summary>
    /// Health damage system for AI 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="type"></param>
    /// <param name="damageBoost"></param>
    public void TakeDamage(int damage, WeaponType type, int damageBoost)
    {
        if (_health > 0)
        {
            switch (type)
            {
                case WeaponType.Normal:
                {
                    _health -= damage + damageBoost;

                    //StartCoroutine(HitFlash());

                    //damageEffectTime = 2f; 
                    break;
                }

                case WeaponType.Ice:
                {
                    if (_shooterScript.WUpgraded == true)
                    {
                        //StartCoroutine(DamageOverTime(_damageOverTime, _durationOfDot));
                        //StartCoroutine(HitFlash());
                    }
                    else
                    {
                        _health -= damage + damageBoost;
                        //Instantiate(targetEffect, transform.position, transform.rotation);
                        //StartCoroutine(HitFlash());
                    }

                    break;
                }

                case WeaponType.Fire:
                {
                    _health -= damage + damageBoost;

                    //StartCoroutine(HitFlash());

                    //damageEffectTime = 2.3f;

                    break;
                }

                case WeaponType.Thunder:
                {
                    _health -= damage + damageBoost;

                    if (_shooterScript.RUpgraded == true)
                        StartCoroutine(StopForSeconds(_stunnedTime));
                    else
                        StartCoroutine(HitFlash());
                    break;
                }

                case WeaponType.Dash:
                {
                    _health -= damage + damageBoost;

                    StartCoroutine(HitFlash());
                    break;
                }
            }

            _damageText.text = damage.ToString();
            StartCoroutine(DamageTextDisappear());

            //healthSlider.value = _health;

            if (_spawnHealth || _spawnMana) DropSpawnCheck();

            if (_health <= healthEvents[0])
                // load event
                CombatEvent(chaseCount[0], rangedCount[0]);

            else if (_health <= healthEvents[1])
                // load event
                CombatEvent(chaseCount[1], rangedCount[1]);

            else if (_health <= healthEvents[2])
                // load event
                CombatEvent(chaseCount[2], rangedCount[2]);
        }

        healthBar.HandleBar(damage);
    }

    
    private IEnumerator HitFlash()
    {
        var isRunning = false;

        if (!isRunning)
        {
            isRunning = true;

            var material = _mesh.material;

            material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            material.color = _color;

            isRunning = false;
        }
    }
    /// <summary>
    /// Damage text disappear, This will make the damage text disappear after a certain amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageTextDisappear()
    {
        yield return new WaitForSeconds(2f);
        _damageText.text = " ";
    }

    /// <summary>
    ///  Damage over time effect, This will damage the AI for a certain amount of time
    /// </summary>
    /// <param name="damagePerSecond"></param>
    /// <param name="durationOfdamage"></param>
    /// <returns></returns>
    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
#if UNITY_EDITOR

        Debug.Log("Started DOT coroutine");
#endif
        var elapsedTime = 0f;
        while (elapsedTime < durationOfdamage)
        {
            _health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(damagePerSecond);
            elapsedTime += 2.5f;
        }
    }

    /// <summary>
    ///  Stop for seconds effect, This will stop the AI from moving and attacking for a certain amount of time
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerator StopForSeconds(float value)
    {
        // Start bool at false to make sure coroutine is not being run more than once


        switch (_stfsEffect)
        {
            case false:
            {
                _stfsEffect = true;

#if UNITY_EDITOR
                Debug.Log("STARTED Stop for" + value + " COROUTINE");
#endif

                _canAttack = false;
                _currentState = HandleState.Stoped;

                HandleStateAi(true);

                //originalColor = GetComponent<Renderer>().material.color;
                //GetComponent<Renderer>().material.color = new Color(0.6933962f, 0.9245283f, 0.871814f);

                yield return new WaitForSeconds(value);

                //GetComponent<Renderer>().material.color = originalColor;

                _currentState = HandleState.None;

                HandleStateAi(false);
                _canAttack = true;

                _stfsEffect = false;

                break;
            }
            default:
            {
                break;
            }
        }
    }

    #endregion

    /// <summary>
    /// Drop spawn check to see if a drop should be spawned
    /// </summary>
    private void DropSpawnCheck()
    {
        if (!_spawningGems)
        {
            _spawningGems = true;
            var randomFloat = Random.value;

            if (!(randomFloat <= 0.08f)) return;

            var randomPercent = Random.value;

            if (randomPercent <= 0.6f && _spawnHealth)
                SpawnDrop(_healthDrop);
            else if (randomPercent >= 0.61 && _spawnMana) SpawnDrop(_manaDrop);
            _spawningGems = false;
        }
    }

    /// <summary>
    /// Spawn enemies based on the health of the player and quantity of enemies is passed in inspector
    /// </summary>
    /// <param name="enemyChase"></param>
    /// <param name="enemyRanged"></param>
    private void CombatEvent(int enemyChase, int enemyRanged)
    {
        // spawn AI
        StartCoroutine(SpawnAi(enemyChase, enemyRanged));
    }

    /// <summary>
    /// Coroutine that spawns the enemies
    ///  Spawn enemies based on the health of the player and quantity of enemies is passed in inspector
    /// </summary>
    /// <param name="chaseCount"></param>
    /// <param name="rangedCount"></param>
    /// <param name="chase"></param>
    /// <param name="ranged"></param>
    /// <returns></returns>
    private IEnumerator SpawnAi(int chase, int ranged)
    {
        if (_runningAiSpawn) yield break;

        _runningAiSpawn = true;

        for (var i = 0; i < chase; i++)
        {
            var randomDirection = Random.insideUnitSphere * Random.Range(20, 40);

            randomDirection += _playerTarget.position;

            NavMeshHit hit;

            NavMesh.SamplePosition(randomDirection, out hit, _maxRange, 6);

            var spawnPos = hit.position;

            yield return new WaitForSeconds(effectTime);
            Instantiate(aiSpawnEffect, spawnPos, transform.rotation);

            yield return new WaitForSeconds(spawnTime);
            Instantiate(chaseAi, spawnPos, Quaternion.identity);

#if UNITY_EDITOR
            Debug.Log("Boss spawning chase");
#endif
        }

        for (var i = 0; i < ranged; i++)
        {
            var randomDirection = Random.insideUnitSphere * Random.Range(minRange, maxRange);

            randomDirection += _playerTarget.position;

            NavMeshHit hit;

            NavMesh.SamplePosition(randomDirection, out hit, maxRange, 1);

            var spawnPos = hit.position;

            yield return new WaitForSeconds(effectTime);
            Instantiate(aiSpawnEffect, spawnPos, transform.rotation);

            yield return new WaitForSeconds(spawnTime);
            Instantiate(rangedAi, spawnPos, Quaternion.identity);

#if UNITY_EDITOR
            Debug.Log("Boss spawning ranged");
#endif
        }

        _runningAiSpawn = false;
    }

    #region Death

    public void Die()
    {
        if (!_isDead)
        {
            _isDead = true;
            RuntimeManager.PlayOneShot(deathSound, transform.position);


            if (_spawnHealth)
                for (var i = 0; i < _healthItems; i++)
                    SpawnDrop(_healthDrop);

            if (_spawnMana)
                for (var i = 0; i < _manaItems; i++)
                    SpawnDrop(_manaDrop);


            Instantiate(_deathEffect, transform.position, Quaternion.identity);

            _valuesTexts.GetKill();

            _objectiveUiScript.IncreaseEnemyDefeatedCount();


            Destroy(gameObject);
        }
    }

    #endregion

    // Drop area
    /// <summary>
    ///  Drops a health or mana drop
    /// </summary>
    /// <param name="drop"></param>
    private void SpawnDrop(GameObject drop)
    {
        var spawnPosition = dropPos.position +
                            new Vector3(Random.Range(-_dropRadius, _dropRadius),
                                0f,
                                Random.Range(-_dropRadius, _dropRadius));

        Instantiate(drop, spawnPosition, Quaternion.identity);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

#if UNITY_EDITOR

    #region Editor Gizmos

    private void OnDrawGizmos()
    {
        var red = new GUIStyle
        {
            normal =
            {
                textColor = Color.red
            }
        };

        var yellow = new GUIStyle
        {
            normal =
            {
                textColor = Color.yellow
            }
        };

        var blue = new GUIStyle
        {
            normal =
            {
                textColor = Color.blue
            }
        };

        var green = new GUIStyle
        {
            normal =
            {
                textColor = Color.green
            }
        };

        var cyan = new GUIStyle
        {
            normal =
            {
                textColor = Color.cyan
            }
        };

        #region Gizmos code

        #region AI State Label

        if (showGizmos)
            switch (stateAi)
            {
                case Ai.Patrol:
                {
                    Handles.Label(transform.position + Vector3.up * 2f, "Patrol" + "  Gameplay: " + _gameState, blue);
                    break;
                }
                case Ai.Attack:
                {
                    Handles.Label(transform.position + Vector3.up * 2f, "Attack" + "  Gameplay: " + _gameState, red);
                    break;
                }
                default:
                {
                    Handles.Label(transform.position + Vector3.up * 2f, "NO STATE FOUND" + "  Gameplay: " + _gameState);
                    break;
                }
            }

        #endregion

        #endregion
    }

    #endregion

#endif
}