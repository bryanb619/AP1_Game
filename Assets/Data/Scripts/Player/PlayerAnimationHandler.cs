using UnityEngine;


public class PlayerAnimationHandler : MonoBehaviour
{
    private Vector3 previousPosition;
    public float speed;

    private float crossFadeTime = 0.2f; 

    private Animator playerAnimator;

    private AnimatorStateInfo stateInfo;

    public bool CANMOVE;

    private bool _gamePlay; 


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
            playerAnimator.SetBool("_canRun", true);
            return;
        }
        else
        {
            playerAnimator.SetBool("_canRun", false);
            return;
        }


        /*if (stateInfo.IsName("idle"))
        {
            playerAnimator.SetFloat("Blend", 0.2f);

            playerAnimator.CrossFade("run", crossFadeTime);


        }

    }
    else
    {

        if (stateInfo.IsName("run"))
        {
            playerAnimator.SetFloat("Blend", 0.0f);

            playerAnimator.CrossFade("idle", crossFadeTime);

            playerAnimator.SetBool("_canRun", false);
        }


    }
         */
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

        //throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }



}
