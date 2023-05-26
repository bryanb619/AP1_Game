using UnityEngine;
using UnityEngine.Serialization;


public class PlayerAnimationHandler : MonoBehaviour
{
    private Vector3 _previousPosition;
    
    [HideInInspector]
    public float speed;

    //private float crossFadeTime = 0.2f; 

    private Animator _playerAnimator;

    private AnimatorStateInfo _stateInfo;

    [FormerlySerializedAs("CANMOVE")] [HideInInspector] 
    public bool canmove;

    private bool _gamePlay;

    [SerializeField]
    private float velocityChanger;

    private float _velocity;


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

    public void QAttack()
    {
        _playerAnimator.SetTrigger("qAbility");
    }

    public void RAttack()
    {
        _playerAnimator.SetTrigger("rAbility");
    }

    public void NormalAttack()
    {
        _playerAnimator.SetTrigger("normalAttack");
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}
