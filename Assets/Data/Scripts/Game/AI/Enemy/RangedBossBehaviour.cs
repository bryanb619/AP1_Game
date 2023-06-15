using System;
using System.Collections;
using Data.Scripts.Game.AI.Enemy;
using UnityEngine;
using UnityEngine.AI;
using LibGameAI.FSMs;
using TMPro;
using Random = UnityEngine.Random;
using State = LibGameAI.FSMs.State;
using StateMachine = LibGameAI.FSMs.StateMachine;


public class RangedBossBehaviour : MonoBehaviour
{
        #region  Variables
    // Systems
    [Header("AI Profile")]
    [SerializeField] private AiRangedData data;
    
    // AI 
                        // AI states
                        private enum Ai                             {Patrol, Attack}
                        [Header("AI State")]
    [SerializeField]    private Ai                                  stateAi;
                        // state machine 
                        private StateMachine                        _fsm;
                        // nav mesh agent
                        public NavMeshAgent                         agent;
                        private float                               _pathUpdateDeadLine = 0.2f;
                        // Ai Shoot
                        private AiShoot                             _aiShoot;
                        // performance handler
                        private AIHandler                           _aiHandler;
    
                        private enum HandleState                    {Stoped, None}
                        private HandleState                         _currentState;
                        
                        private EnemyType                           _enemyType;
                         
       
                        
    // other components ----------------------------------------------------------------------------------------------->
    
                        // objectives UI 
                        private ObjectiveUi                         _objectiveUiScript;
    [SerializeField]    private Outline                             outlineDeactivation;
                        private Animator                            _animator;
                        private SkinnedMeshRenderer                 _mesh;
                        private Color                               _color; 
                        
                        // Game State
                        private GameState                           _gameState;
                    
    // Player Refs ---------------------------------------------------------------------------------------------------->
    
                        private GameObject                          _player;
                        private Transform                          _playerTarget;
                        private Shooter                             _shooterScript;
    [SerializeField]    private LayerMask                          playerLayer;
   
    
    // Combat --------------------------------------------------------------------------------------------------------->
    
    // General combat variables
    
                    private bool                                _canAttack;
                    private float                               _safeDistance;  
    
                    private float                               _fireRate = 2f;
                    private float                               _nextFire;

                    // random attack chance
                    private float                               _percentage;
    
    // special attack
    
    
                    private const float                         AbilityMaxValue = 100f;
                    private float                               _currentAbilityValue;
                    private float                               _abilityIncreasePerFrame;
                    private bool                                _canSpecialAttack;
                    
                    private float                               _minRange, _maxRange;
                    private GameObject                          _teleportEffect;
                    private float                              _nextTeleport;
                    private float                               _teleportRate;

                    private int                                 _areaAttackDamage; 
                    private float                               _areaAttackRange; 
                    private float                              _nextSpecialAttack;
                    private float                               _specialAttackRate = 2f;
                    
                    
    // projectiles
                    private GameObject                          _projectile, _randomProjectile,
                        _specialProjectile, _areaAttack;
    
           
    // Health/Death --------------------------------------------------------------------------------------------------->
    
                    private AiHealth                            _healthBar;    
                    // HEALTH
                    private float                               _health;

                    // DEATH
                    private GameObject                         _deathEffect;
                    
                    
    // Drops/Loot ----------------------------------------------------------------------------------------------------->
    
                        // Health
                        private GameObject                          _healthDrop;
                        private int                                 _healthItems;
                        private bool                                _spawnHealth;
                    
                        // Mana
                        private GameObject                          _manaDrop;
                        private int                                 _manaItems;
                        private bool                                _spawnMana;

                        // Drop Radius
                        private float                               _dropRadius; 
    [SerializeField]    private Transform                           dropPos;
                    
                    
    // UI ------------------------------------------------------------------------------------------------------------->
    
                        private TextMeshProUGUI             _damageText;




private float _stunnedTime;

    #endregion
    
     //----------------------------------------------------------------------------------------------------------------->
    
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        
        _enemyType                  = data.enemyType;
        
        _currentState = HandleState.None;
        _canAttack = true;
        
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
        {
            // resume agent
            HandleStateAi(false);
        }
    }
    
    private void PauseAi()
    {
        if (_currentState != HandleState.None) return;
        
        if (agent.enabled)
        {
            //stop agent
            HandleStateAi(true);
        }
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
        agent                       = GetComponent<NavMeshAgent>(); 
        _aiShoot                    = GetComponentInChildren<AiShoot>();
        _aiHandler                  = GetComponent<AIHandler>();
        _healthBar                   = GetComponentInChildren<AiHealth>();
        
        // mesh 
        _mesh                       = GetComponentInChildren<SkinnedMeshRenderer>();
        _color                      = _mesh.material.color;
        
        // animator
        _animator                   = GetComponentInChildren<Animator>();
        
        // Player 
        _player                     = GameObject.Find("Player");
        _playerTarget               = _player.transform;
        _shooterScript              = _player.GetComponent<Shooter>();
        
        _damageText                = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    private void ProfileSync()
    {
        
        
        // combat 
        _safeDistance               = data.MinDist;
        
        _fireRate                   = data.AttackRate;
        _percentage                 = data.Percentage; 
        

        // Special attack Ability

        _currentAbilityValue        = data.CurrentAbilityValue;
        _abilityIncreasePerFrame    = data.AbilityIncreasePerFrame;
        
        _teleportRate              = data.TeleportTime;
        _areaAttackDamage           = data.AreaDamageAttack;
        _areaAttackRange            = data.AreaDamageRadius;
        
        _areaAttack                = data.AreaAttack;
        
        
        // Drops ------------------------------------------------------------------------------------------------------>
        
        // Health Standard
        _spawnHealth                = data.SpawnHealth;    
        _healthDrop                 = data.HealthDrop;
        _healthItems                = data.HealthItems;    

        // Mana Standard
        _spawnMana                  = data.SpawnMana;
        _manaDrop                   = data.ManaDrop;
        _manaItems                  = data.ManaItems;  
        
        // special Health
        
        
        // Special Mana
        
        // Drop Radius
        _dropRadius                 = data.DropRadius;
        
        
        // Health/Death ----------------------------------------------------------------------------------------------->
         _health                    = data.Health;
        _deathEffect                = data.DeathEffect;
        
        // projectiles ------------------------------------------------------------------------------------------------>

        _projectile                 = data.NProjectile;
        _randomProjectile           = data.RProjectile;

        
        // Boss 
        if (_enemyType == EnemyType.Boss)
        {
            _specialProjectile      = data.SProjectile;
            _minRange               = data.TeleportMinRange;
            _maxRange               = data.TeleportMaxRange;
            
            _teleportEffect         = data.TeleportEffect;

#if UNITY_EDITOR
            Debug.Log("Boss");
#endif
            
        }
        
    }
    // Start is called before the first frame update
    private void Start()
    {
        _healthBar.HealthValueSet(_health);
    }

    private void StateSet()
    {
        // Create the states
        var patrolState   = new State("Patrol" ,null);
        var chaseState    = new State("Chase", Engage);
        
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

    // Update is called once per frame
    private void Update()
    {
        UpdateAi();
    }

    #region AI Handling
    private void UpdateAi()
    {
        switch (_aiHandler.activeAi)
        {
            case AIHandler.ActiveAi.Active:
            {
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
                InactiveAi(); 
                break;
            }

        }
    }
    
    private void InactiveAi()
    {
        if (agent.enabled == true)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            
        }
    }



    private void Gameplay()
    {
        var actions = _fsm.Update();
        actions?.Invoke();

        if ((_playerTarget.transform.position - transform.position).magnitude <= 2)
        {
            agent.enabled = false;
            
        }
    }
    
    private void Paused()
    {
       
    }
    #endregion
    
    #region Actions
    
    private void Patrol()
    {
        
    }

    private void Engage()
    {
        if (_canAttack)
        {
            BossAttack();
            UpdateRotation(); 
            
        }            
        
    }   
    
     private void BossAttack()
     { 
         switch(_canSpecialAttack)
        {
            case true:
                {
                    if ((_playerTarget.transform.position - transform.position).magnitude <= _areaAttackRange) 
                    {
                        PauseAi();
                        
                        if (Time.time > _nextSpecialAttack)
                        {
                            AreaAttack();
                            _nextSpecialAttack = Time.time + _specialAttackRate;
                        }
                    }
                    
                    if ((_playerTarget.transform.position - transform.position).magnitude >= _safeDistance)  //
                    {
                        PauseAi();
                        
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
                        if(_canAttack) 
                        { 
                            ResumeAi();
                            UpdatePath();
                        }
                    }
                    else if ((_playerTarget.transform.position 
                              - transform.position).magnitude <= 9) //_attackRange && !_canSpecialAttack) 
                    {
                        GenerateRandomPos();
                    }
                    
                    else if ((_playerTarget.transform.position - transform.position).magnitude >= 13) // valor mais alto 
                    {
                        //var position = _playerTarget.position;
                        
                        PauseAi();

                        if(_canAttack && !_canSpecialAttack) { Attack(); }
                    }
                    
                    CoolDoownPower();
                    break; 
                }
        }
     }
     
     private void SpecialAttack()
     {
         _aiShoot.Shoot(_playerTarget, _specialProjectile);
        
         _currentAbilityValue = 0;
         //abilitySlider.value = _currentAbilityValue;
         //_canSpecialAttack = false;
        
         _animator.SetBool("isAttacking", false);
         // StartCoroutine(SpecialAttackTimer());
         _canSpecialAttack = false;
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
             PlayerHealth player = col.GetComponent<PlayerHealth>();

             if (player == null) return;
             
             Instantiate(_areaAttack, transform.position, _areaAttack.transform.rotation);
             player.TakeDamage(_areaAttackDamage);
             
             _currentAbilityValue = 0;
            _canSpecialAttack = false;
#if UNITY_EDITOR
             const string debugColor = "<size=14><color=red>";
             const string closeDebug = "</color></size>";
             Debug.Log(debugColor + "Area Attack" + closeDebug);
#endif
             //_enemyAnimationHandler.RecievePlayerCollision(player);
            // _animator.SetTrigger("Attack2");
            
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

                 if(Random.value < _percentage && _canAttack)
                 {
                     _animator.SetBool("isAttacking", true);
                     RandomAttack();
                 }
                 
                 else if(_canAttack)
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
            _currentAbilityValue = Mathf.Clamp(_currentAbilityValue + (_abilityIncreasePerFrame * Time.deltaTime), 0.0f, AbilityMaxValue);   
        }

        //abilitySlider.value = _currentAbilityValue;
        //print(currentAbilityValue);
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
        if(Time.time > _nextTeleport)
        {
            TeleportProcess(); 
            
            _nextTeleport = Time.time + _teleportRate;
        }
    }

    private void TeleportProcess()
    {

#if UNITY_EDITOR
        
        print("running");
#endif
        
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(_minRange, _maxRange);

        randomDirection += _playerTarget.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDirection, out hit, _maxRange, 1);

        Vector3 teleportPos = hit.position;

        StartCoroutine(Teleport(teleportPos));
    }

    private IEnumerator Teleport(Vector3 teleportPos)
    {
        
        Instantiate(_teleportEffect, teleportPos, _teleportEffect.transform.rotation);
        
        
        _canAttack = false;
        yield return new WaitForSeconds(0.5f);
        
        agent.enabled = false;
        transform.position = teleportPos;
        yield return new WaitForSeconds(0.2f);
        
        agent.enabled = true;
        _canAttack = true;
        
    }
    
    #endregion
    
    #region Health
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

            if (_spawnHealth || _spawnMana)
            {
                DropSpawnCheck();
            }
            
            _damageText.text = damage.ToString();
            StartCoroutine(DamageTextDisappear());

            //healthSlider.value = _health;
            _healthBar.HandleBar(damage);
        }
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
    
    private IEnumerator DamageTextDisappear()
    {
        yield return new WaitForSeconds(2f);
        _damageText.text = " ";
    }
    
    private IEnumerator DamageOverTime(float damagePerSecond, float durationOfdamage)
    {
#if UNITY_EDITOR
        
        Debug.Log("Started DOT coroutine");
#endif
        float elapsedTime = 0f;
        while (elapsedTime < durationOfdamage)
        {
            _health -= damagePerSecond;
            StartCoroutine(HitFlash());
            yield return new WaitForSeconds(damagePerSecond);
            elapsedTime += 2.5f;
        }

    }
    
    private IEnumerator StopForSeconds(float value)
    {
        // Start bool at false to make sure coroutine is not being run more than once
        bool stfsEffect = false;

        switch(stfsEffect)
        {
            case false:
            {
                stfsEffect = true;

                print("STARTED STFS COROUTINE SUCCESFULLY");

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

                stfsEffect = false;

                break;
            }
            default: {break;}
        }
    }
    
    #endregion

    private void DropSpawnCheck()
    {
        float randomFloat = UnityEngine.Random.value;

        if (!(randomFloat <= 0.08f)) return;
        
        float randomPercent = UnityEngine.Random.value;

        if (randomPercent <= 0.6f && _spawnHealth)
        {
            SpawnDrop(_healthDrop);
        }
        else if (randomPercent >= 0.61 && _spawnMana)
        {
            SpawnDrop(_manaDrop);
        }
    }

    #region Death
    public void Die()
    {
        if(_spawnHealth)
        {
            for (var i = 0; i < _healthItems; i++)
            {
                SpawnDrop(_healthDrop);
            }   
        }

        if(_spawnMana)
        {
            for (var i = 0; i < _manaItems; i++)
            {
                SpawnDrop(_manaDrop);
            }
        }
        
        
        Instantiate(_deathEffect, transform.position, Quaternion.identity);

        //_valuesTexts.GetKill();

        _objectiveUiScript.IncreaseEnemyDefeatedCount();
        

        Destroy(gameObject);
    }
    
    #endregion   

    // Drop area
    private void SpawnDrop(GameObject drop)
    {
        var spawnPosition = dropPos.position +
                            new Vector3(UnityEngine.Random.Range(-_dropRadius, _dropRadius),
                                0f,
                                UnityEngine.Random.Range(-_dropRadius, _dropRadius));

        Instantiate(drop, spawnPosition, Quaternion.identity);
    }
    
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
    
    
}
