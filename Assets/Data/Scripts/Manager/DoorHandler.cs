using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public enum DoorState { Closed, Open, Closing, Opening }

    public DoorState state;

    [Header("Door Configuration")]
    [Range(1, 5)]
    private float speed = 1f;

    [SerializeField] private GameObject door, startPos, Endpos; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ConditionCheck();
        StateCheck();
    }

    private void ConditionCheck()
    {
        if (state == DoorState.Opening)
        {
            //print("opening");
            DoorOpen();
        }

        else if (state == DoorState.Closing)
        {
            //print("closing");
            DoorClose();
        }

    }

    private void StateCheck()
    {
        if (state == DoorState.Opening)
        {
            float minDist = 0.1f;
            bool Open = false;


            //CheatBarrier.DeactivateColl();

            if ((startPos.transform.position - door.transform.position).magnitude < minDist)
            {
                door.transform.position = startPos.transform.position;

                Open = true;

            }
        }

        else if (state == DoorState.Closing)
        {
            float CloseDist = 0.1f;

            bool Closed = false;


            //CheatBarrier.ActivateColl();

            if ((Endpos.transform.position - door.transform.position).magnitude < CloseDist)
            {
                door.transform.position = Endpos.transform.position;
                Closed = true;
            }


        }
    }

    private void DoorOpen()
    {
        //Debug.Log(
        door.transform.Translate(Vector3.up * speed * Time.deltaTime); // move left door left
        //doorRight.transform.Translate(Vector3.right * speed * Time.deltaTime);// move right door rigt
    }

    private void DoorClose()
    {
        //Debug.Log("FECHA");
        door.transform.Translate(Vector3.down * speed * Time.deltaTime); // move left door right
        //doorRight.transform.Translate(Vector3.left * speed * Time.deltaTime); // move right door left
    }

    public void Action(DoorState state)
    {
        switch (state) 
        {
            case DoorState.Opening:
                {
                    break;
                }
            case DoorState.Closing:
                {
                    break; 
                }
        }
    }
}
