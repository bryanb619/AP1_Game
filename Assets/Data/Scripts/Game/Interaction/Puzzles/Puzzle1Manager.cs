using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle1Manager : MonoBehaviour
{
    [FormerlySerializedAs("LeftTriggerAnim_IsPlaying")] public bool leftTriggerAnimIsPlaying = false;
    [FormerlySerializedAs("RightTriggerAnim_IsPlaying")] public bool rightTriggerAnimIsPlaying = true;

    [FormerlySerializedAs("button_1")] [SerializeField] private Animator button1;
    private Animator _button2;

    private DoorHandler _door;

    //public GameObject Mirror;

    private void Start()
    {
        _door = FindObjectOfType<DoorHandler>();       
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
        if (this.button1.GetCurrentAnimatorStateInfo(0).IsName("left_trigger"))
        {
            // Make animation true
            leftTriggerAnimIsPlaying = true;
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
        if (leftTriggerAnimIsPlaying && rightTriggerAnimIsPlaying == true)
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
