using UnityEngine;


public class PlayerAnimationHandler : MonoBehaviour
{
    private Vector3 previousPosition;
    public float speed;

    private float crossFadeTime = 0.2f; 

    private Animator playerAnimator;

    private AnimatorStateInfo stateInfo;

    public bool CANMOVE; 

    private void Start()
    {
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
        Vector3 curMove = transform.position - previousPosition;
        speed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        if (speed >= 1)
        {
            // The object is moving
            CANMOVE = true;
        }
        else if(speed <=1)
        {
            // The object is not moving
            CANMOVE = false;
        }
        HandleAnimation(); 
    }

    private void HandleAnimation()
    {
        if (CANMOVE)
        {
            playerAnimator.SetBool("_canRun", true);
        }
        else
        {
            playerAnimator.SetBool("_canRun", false);
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

}
