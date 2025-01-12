using System;
using System.Collections;
using Data.Scripts.Game.AI.Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using LibGameAI.FSMs;
using TMPro;
using Random = UnityEngine.Random;
using State = LibGameAI.FSMs.State;
using StateMachine = LibGameAI.FSMs.StateMachine;

// AI Ranged Behaviour
public class EnemyRangedBehaviour : MonoBehaviour
{
    #region  Variables
    // Systems
    [Header("AI Profile")]
    [SerializeField] private AIRangedData data;
    
    // AI 
                        // AI states
                        private enum Ai                             {Patrol, Attack}
                        [Header("AI State")]
    [SerializeField]    private Ai                                  stateAi;
                        // state machine 
                        private StateMachine                        _fsm;
                        // nav mesh agent
                        public NavMeshAgent                         agent;
                        private float                               _pathUpdateDeadLine = 0.5f;
                        // Ai Shoot
                        private AiShoot                             _aiShoot;
                        // performance handler
                        private AIHandler                           _aiHandler;
    
                        private enum HandleState                    {Stoped, None}
                        private HandleState                         _currentState;
                        
                        private EnemyType                           _enemyType;
                        
      [SerializeField]  private TextMeshProUGUI                     damageText;
                        private float                               _randomPriority;
                        private bool _spawningGems; 
                        private bool _isDead; 
                        
                         
       
                        
    // other components ----------------------------------------------------------------------------------------------->
    
                        // objectives UI 
                        private ObjectiveUI                         _objectiveUiScript;
    [SerializeField]    private Outline                             outlineDeactivation;
                        private Animator                            _animator;
                        private SkinnedMeshRenderer                 _mesh;
                        private Material[]                          skinnedMaterials;
                        private float                               dissolveRate = 0.0125f;
                        private float                               refreshRate = 0.025f;
    [SerializeField]    private VisualEffect                        VFXGraph;
                        private Color                               _color; 
                        
                        // Game State
                        private GameState                           _gameState;
                    
    // Player Refs ---------------------------------------------------------------------------------------------------->
    
                        private GameObject                          _player;
                        private Transform                          _playerTarget;
                        private Shooter                             _shooterScript;
   
    
    // Combat --------------------------------------------------------------------------------------------------------->
    
    // General combat variables
    
    
                    private bool                                _canAttack;
                    private float                               _attackRange;
                    private float                               _attackTime; 
                    private float                               _firstAttackTime;

                    private float                               _fireRate = 2f;
                    private float                               _nextFire;

                    // random attack chance
                    private float                               _percentage;
                    
                    // area attack 
                    private float                               _areaAttackRange; 
                    
                    private float                               _nextAreaAttack;
                    private float                    _areaAttackRate = 3f;
    [SerializeField] private LayerMask               playerLayer;
    [SerializeField] private Transform              _areaAttack;
                    private int                     _areaAttackDamage;
                    private GameObject             _areaAttackEffect;

                    // special attack
    
    
                    private const float                         AbilityMaxValue = 100f;
                    private float                               _currentAbilityValue;
                    private float                               _abilityIncreasePerFrame;
                    private bool                                _canSpecialAttack;
                    
                    private float                               _minRange, _maxRange;
                    private GameObject                          _teleportEffect;
                    private float                              _nextTeleport;
                    private float                               teleportRate = 5f;
                    private float                               _randomFire; 
                    
    
    // projectiles
                    private GameObject                          _projectile, _randomProjectile ,_specialProjectile;
    
           
    // Health/Death --------------------------------------------------------------------------------------------------->
    
                    private AIHealth                            _healthBar;    
                    // HEALTH
                    private float                               _health; 
   [SerializeField] private VisualEffect                        hitVFX;
                    
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
    
                        private ValuesTextsScript           _valuesTexts;
    
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
        
        if (_mesh != null)
            skinnedMaterials = _mesh.materials;

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
        //if (_currentState != HandleState.None) return;
        
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
        agent.updateRotation        = false;
        _aiShoot                    = GetComponentInChildren<AiShoot>();
        _aiHandler                  = GetComponent<AIHandler>();
        _healthBar                  = GetComponentInChildren<AIHealth>();
        
        // mesh 
        _mesh                       = GetComponentInChildren<SkinnedMeshRenderer>();
        _color                      = _mesh.material.color;
        
        // animator
        _animator                   = GetComponentInChildren<Animator>();
        
        // UI 
        _valuesTexts                = GameObject.Find("ValuesText").GetComponent<ValuesTextsScript>();
        
        // Player 
        _player                     = GameObject.Find("Player");
        _playerTarget               = _player.transform;
        _shooterScript              = _player.GetComponent<Shooter>();
        
        // objective UI
        _objectiveUiScript          = FindObjectOfType<ObjectiveUI>();
    }
    
    private void ProfileSync()
    {
        if(_enemyType == EnemyType.Normal)
        {
            // random priority
            _randomPriority             = UnityEngine.Random.Range(81, 99);
            agent.avoidancePriority     = (int)_randomPriority;
        }
         
        _firstAttackTime                 = UnityEngine.Random.Range(0.5f, 2.5f);
        
        // combat 
        
        _attackRange                = UnityEngine.Random.Range(10, 17);

        _fireRate                   = data.AttackRate;
        _percentage                 = data.Percentage; 
        
        _randomFire                 = UnityEngine.Random.Range(1f, _fireRate);
        
        _areaAttackRange            = data.AreaDamageRadius;
        _areaAttackDamage           = data.AreaDamageAttack;
        _areaAttackEffect          = data.AreaDamageEffect;
        
        

        // Special attack Ability

        _currentAbilityValue        = data.CurrentAbilityValue;
        _abilityIncreasePerFrame    = data.AbilityIncreasePerFrame;
        
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
        
        

    }
    // Start is called before the first frame update
    
    private void StateSet()
    {
        // Create the states
        var patrolState   = new State("Patrol" ,Patrol);
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
    
    private void Start()
    {
        _healthBar.HealthValueSet(_health);
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
                Paused();
                break;
            }
            case GameState.Death:
            {
                _gameState = GameState.Death;
                stateAi = Ai.Patrol; 
                Paused();
                break;
            }

        }
    }
    
    private void InactiveAi()
    {
        transform.rotation = Quaternion.identity;
        
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
        
    }

    private void Gameplay()
    {
        var actions = _fsm.Update();
        actions?.Invoke();
    }
    
    private void Paused()
    {
        
    }
    #endregion
    
    #region Actions
    
    private void Patrol()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        
        transform.position = transform.position;
    }

    private void Engage()
    {
        if (_gameState == GameState.Gameplay && !_isDead)
        {
            UpdateRotation();
        
            if (!_canAttack)
            {
                _attackTime += Time.deltaTime;
                if(_firstAttackTime >= _attackTime)
                {
                    _canAttack = true;
                }
            }
        
            else if (_canAttack) 
            {
                Attack();
            
            }
     
        
            if ((_playerTarget.transform.position - transform.position).magnitude <= 2f)
            {
                agent.radius = 0.1f;

                if ((_playerTarget.transform.position - transform.position).magnitude <= 0.51f)
                {
                    agent.enabled = false;
                }
            }
        }
    }   
    
     private void Attack()
     {
         if ((_playerTarget.transform.position - transform.position).magnitude <= _attackRange)
         {
             
             PauseAi();
             
             if(_canAttack)
             {
                 if (Time.time > _nextFire)
                 {
                     NormalAttack();
                     _nextFire = Time.time + _randomFire;
                 }
             }
             
             if((_playerTarget.transform.position - transform.position).magnitude <= 2.5f)
             {
                 if(Time.time > _nextAreaAttack)
                 {
                     agent.enabled = false;
                     AreaAttack();
                     _nextAreaAttack = Time.time + _areaAttackRate;
                 }
             }
         }
         
         if ((_playerTarget.transform.position - transform.position).magnitude >= _attackRange)
         {
             UpdatePath();

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
                 _nextFire = Time.time + _randomFire;
             }
         }
         /*
         else
         {
             _animator.SetBool("isAttacking", false);
            
         }
         */
     }

     #region Agent Pathfinding & Rotation
     
     private void UpdatePath()
     {
         if (agent.enabled )
         {
             ResumeAi();
             if (Time.time >= _pathUpdateDeadLine)
             {
                 // update time
                 _pathUpdateDeadLine = Time.time + 0; 
                 // set destination
                 agent.SetDestination(_playerTarget.position);    
             }
         }
     }
     
     private void UpdateRotation()
     {
         // get player direction
         var direction = _playerTarget.position - transform.position;
         //rotation 
         var disaredRot = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

         // apply rotation to AI 
         transform.rotation = disaredRot;
     }
     
     #endregion
    
     private void NormalAttack()
    {
             _animator.SetBool("isAttacking", true);
             
             _aiShoot.Shoot(_playerTarget, _projectile); 
                    
             //StartCoroutine(AttackTimer());
             
    #if UNITY_EDITOR   
                    const string debugAttack = "<size=12><color=green>";
                    const string closeAttack = "</color></size>";
                    Debug.Log(debugAttack + "Attack: " + closeAttack);
    #endif
    
     }
        
     
     private void RandomAttack() 
     {
         _aiShoot.Shoot(_playerTarget, _randomProjectile);
                
        // StartCoroutine(AttackTimer()); 
         
#if UNITY_EDITOR                     
                const string debugAttack = "<size=12><color=yellow>";
                const string closeAttack = "</color></size>";
                Debug.Log(debugAttack + "Attack 2: " + closeAttack);
#endif
     }
     
     
     private void AreaAttack()
     {
         var hitEnemies = Physics.OverlapSphere(transform.position, _areaAttackRange, playerLayer);

         foreach (var col in hitEnemies)
         {
             PlayerHealth player = col.GetComponent<PlayerHealth>();

             if (player == null) return;
             
             Instantiate(_areaAttackEffect, transform.position, _areaAttack.transform.rotation);
             player.TakeDamage(_areaAttackDamage);
          
             
#if UNITY_EDITOR
             const string debugColor = "<size=14><color=red>";
             const string closeDebug = "</color></size>";
             Debug.Log(debugColor + "Area Attack" + closeDebug);
#endif
             //_enemyAnimationHandler.RecievePlayerCollision(player);
             // _animator.SetTrigger("Attack2");
            
         }
     }
     #endregion
    
    #region Health
    public void TakeDamage(int damage, WeaponType type, int damageBoost)
    {
         if (_health >= 0)
         {
             switch (type)
             {
                 case WeaponType.Normal:
                 {
                     _health -= damage + damageBoost;

                     StartCoroutine(HitFlash());

                     //damageEffectTime = 2f; 
                     break;
                 }

                 case WeaponType.Ice:
                 {
                     if (_shooterScript.WUpgraded == true)
                     {
                         //StartCoroutine(DamageOverTime(_damageOverTime, _durationOfDot));
                         StartCoroutine(HitFlash());
                     }
                     else
                     {
                         _health -= damage + damageBoost;
                         //Instantiate(targetEffect, transform.position, transform.rotation);
                         StartCoroutine(HitFlash());
                     }
                        
                     break;
                 }

                 case WeaponType.Fire:
                 {
                     _health -= damage + damageBoost;

                     StartCoroutine(HitFlash());

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
             
             //hitVFX.Play();
             damageText.text = damage.ToString();
             //  CALCULATE HEALTH 
             StartCoroutine(DamageTextDisappear());
             StartCoroutine(StopForSeconds(0.3f));
         }
         
         _healthBar.HandleBar(damage);
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
        damageText.text = " ";
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
#if UNITY_EDITOR
                print("STARTED STFS COROUTINE SUCCESFULLY");
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

                stfsEffect = false;

                break;
            }
            default: {break;}
        }
    }
    
    #endregion

    private void DropSpawnCheck()
    {
        if (!_spawningGems)
        {
            _spawningGems = true;
            
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
            _spawningGems = false;
        }
        
    }

    #region Death

    public void Die()
    {
        if (_health<=0)
        {
            if (!_isDead)
            {
                
                _isDead = true;
                
                _animator.enabled = false;
                
                StartCoroutine(DissolveEnemyRanged());
                stateAi = Ai.Patrol;
                
                
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
            
            }
        }
    }
    
    #endregion   

    private IEnumerator DissolveEnemyRanged()
    {
        if (VFXGraph != null)
        {
            VFXGraph.Play();
        }
        
        if (skinnedMaterials.Length > 0)
        {
            float counter = 0;
            
            while(skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
        Instantiate(_deathEffect, transform.position, Quaternion.identity);
        _valuesTexts.GetKill();
        _objectiveUiScript.IncreaseEnemyDefeatedCount();
        Destroy(this.gameObject);
    }
    

    /// <summary>
    ///  Object drop spawn
    /// </summary>
    /// <param name="drop"></param>
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
