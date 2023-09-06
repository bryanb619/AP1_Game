using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;


public class PlayerAnimationHandler : MonoBehaviour
{
    private Vector3 _previousPosition;
    
    [HideInInspector]
    public float speed;

    //private float crossFadeTime = 0.2f; 

    private Animator _playerAnimator;

    private AnimatorStateInfo _stateInfo;
    
    [SerializeField]
    private VisualEffect _swordVFX;  

    [FormerlySerializedAs("CANMOVE")] [HideInInspector] 
    public bool canmove;

    private bool _gamePlay;

    [SerializeField]
    private float velocityChanger;

    private float _velocity;

    private Shooter shooter;
    private PlayerMovement playerMovement;

    public bool cantUseOtherAbilities = false;
    
    private void Awake()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        _gamePlay = true;

        canmove = false;
 
        _previousPosition = transform.position;

        _playerAnimator = GetComponent<Animator>();

        _playerAnimator.SetBool("_canRun", false);

         _stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(0);

        _velocity = 0f;

        shooter = FindObjectOfType<Shooter>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (_gamePlay)
        {
            Vector3 curMove = transform.position - _previousPosition;
            speed = curMove.magnitude / Time.deltaTime;
            _previousPosition = transform.position;

            if (speed >= 1)
            {
                // The object is moving
                canmove = true;
                
            }
            else if (speed <= 1)
            {
                // The object is not moving
                canmove = false;
                
            }
            HandleAnimation();
        }
        
    }

    private void HandleAnimation()
    {
        if (canmove)
        {
            BlendTreeStartup();
            return;
        }
        else
        {
            BlendTreeSlowdown();
            return;
        }
    }
    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                {
                    _gamePlay = true;
                    break;
                }
            case GameState.Paused:
                {
                    _gamePlay = false;
                    break;
                }
        }
    }



    private void BlendTreeStartup()
    {
        if(_velocity < 1f)
        {
            _velocity += velocityChanger * Time.deltaTime;
            _playerAnimator.SetFloat("Velocity", _velocity);
        }
    }
    private void BlendTreeSlowdown()
    {
        if (_velocity > 0)
        {
            _velocity -= velocityChanger * Time.deltaTime;
            _playerAnimator.SetFloat("Velocity", _velocity);
        }
        else
            _velocity = 0f;
    }

    public void DeathAnim()
    {
        _playerAnimator.SetBool("isDead", true);
    }

    public void HitAnim()
    {
        _playerAnimator.SetTrigger("isHit");
    }

    public void DealDamage()
    {
        shooter.Shoot();
        Debug.Log("Dealt Damage");
    }

    public void QAttack()
    {
        if (_playerAnimator.GetBool("qAbility") == false && !cantUseOtherAbilities)
        {
            _playerAnimator.SetBool("qAbility", true);
        }

    }
    public void QAttackStop()
    {
        _playerAnimator.SetBool("qAbility", false);
    }
    
    public void WAttack()
    {
        if (_playerAnimator.GetBool("wAbility") == false && !cantUseOtherAbilities)
        {
            _playerAnimator.SetBool("wAbility", true);
        }
        
    }
    
    public void WAttackVFX()
    {
        shooter.TargetAttackVFX();
    }

    public void WAttackDamage()
    {
        shooter.TargetAttackShoot();
    }

    public void WAttackStop()
    {
        _playerAnimator.SetBool("wAbility", false);
    }

    public void RAttack()
    {
        if(_playerAnimator.GetBool("rAbility") == false && !cantUseOtherAbilities)
        {
            _playerAnimator.SetBool("rAbility", true);
        }
    }
    public void RAttackStop()
    {
        _playerAnimator.SetBool("rAbility", false);
    }

    public void DisableUsageOfOtherAbilities()
    {
        cantUseOtherAbilities = true;
    }
    
    public void EnableUsageOfOtherAbilities()
    {
        cantUseOtherAbilities = false;
    }

    public void NormalAttack()
    {
        if(_playerAnimator.GetBool("normalAttack") == false)
            _playerAnimator.SetBool("normalAttack", true);
    }

    public void NormalAttackStop()
    {
        _playerAnimator.SetBool("normalAttack", false);
    }
    
    public void StopMovement()
    {
        playerMovement.StopMovement();
    }

    public void RestartMovement()
    {
        playerMovement.RestartMovement();
    }

    public void RCircle()
    {
        shooter.RAbilityTelegraphActivation();
    }

    public void CastBasicAttack()
    {
        _playerAnimator.SetBool("BasicAttack", true);
    }

    public void ResetBasicAttackVariable()
    {
        _playerAnimator.SetBool("BasicAttack", false);
    }

    public void SwordAnimationVFX()
    {
        _swordVFX.Play();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}
