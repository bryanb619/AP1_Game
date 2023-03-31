using UnityEngine;


public class PlayerAnimationHandler : MonoBehaviour
{
    private Vector3 previousPosition;
    
    [HideInInspector]
    public float speed;

    private float crossFadeTime = 0.2f; 

    private Animator playerAnimator;

    private AnimatorStateInfo stateInfo;

    [HideInInspector] 
    public bool CANMOVE;

    private bool _gamePlay;

    [SerializeField]
    private float velocityChanger;

    private float velocity;


    private void Awake()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {

        _gamePlay = true;

        CANMOVE = false;
 
        previousPosition = transform.position;

        playerAnimator = GetComponent<Animator>();

        playerAnimator.SetBool("_canRun", false);

         stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        velocity = 0f;
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
            Vector3 curMove = transform.position - previousPosition;
            speed = curMove.magnitude / Time.deltaTime;
            previousPosition = transform.position;

            if (speed >= 1)
            {
                // The object is moving
                CANMOVE = true;
                
            }
            else if (speed <= 1)
            {
                // The object is not moving
                CANMOVE = false;
                
            }
            HandleAnimation();
        }
        
    }

    private void HandleAnimation()
    {
        if (CANMOVE)
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

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void BlendTreeStartup()
    {
        if(velocity < 1f)
        {
            velocity += velocityChanger * Time.deltaTime;
            playerAnimator.SetFloat("Velocity", velocity);
        }
    }
    private void BlendTreeSlowdown()
    {
        if (velocity > 0)
        {
            velocity -= velocityChanger * Time.deltaTime;
            playerAnimator.SetFloat("Velocity", velocity);
        }
        else
            velocity = 0f;
    }

}
