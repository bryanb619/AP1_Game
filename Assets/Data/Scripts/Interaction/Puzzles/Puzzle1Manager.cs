using UnityEngine;

public class Puzzle1Manager : MonoBehaviour
{
    public bool LeftTriggerAnim_IsPlaying = false;
    public bool RightTriggerAnim_IsPlaying = true;

    [SerializeField] private Animator button_1;
    private Animator button_2;

    private DoorHandler Door;

    //public GameObject Mirror;

    private void Start()
    {
        Door = FindObjectOfType<DoorHandler>();       
    }

    // Update is called once per frame
    void Update()
    {
        // check puzzle condition
            PuzzleUpdate();
    }

    // Puzzle Handler
    private void PuzzleUpdate()
    {
        // check for Left Trigger
        if (this.button_1.GetCurrentAnimatorStateInfo(0).IsName("left_trigger"))
        {
            // Make animation true
            LeftTriggerAnim_IsPlaying = true;
            // play interect sound
            //Puzzle_Interact.Play();

            //print("got here");


        }
        /*
        // check for Right Trigger
        if (this.button_2.GetCurrentAnimatorStateInfo(0).IsName("Right_trigger"))
        {
            // Make animation true
            RightTriggerAnim_IsPlaying = true;
            // play interect sound
            //Puzzle_Interact.Play();

        }
        */
        // if all true place piece of glass in UI
        if (LeftTriggerAnim_IsPlaying && RightTriggerAnim_IsPlaying == true)
        {
            // call UI 
            AtivateDoor();

        }
    }
    // Door Manager Manager
    private void AtivateDoor()
    {
        // set mirror active
        //Door.OpenDoor();

    }
}
