using System;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public enum DoorState { Closed, Open, Closing, Opening }

    public DoorState state;

    [Header("Door Configuration")]
    [Range(1, 5)]
    private float _speed = 1f;

    [SerializeField] private GameObject door, startPos;
     [SerializeField] private GameObject endpos; 
     
     [SerializeField] private GameObject[] barrier;
     


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ConditionCheck();
        //StateCheck();
    }
    
    
    public void DisableBarrier(int index)
    {
        barrier[index].SetActive(false);
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
            //bool Open = false;


            //CheatBarrier.DeactivateColl();

            if ((startPos.transform.position - door.transform.position).magnitude < minDist)
            {
                door.transform.position = startPos.transform.position;

                return;

            }
        }

        else if (state == DoorState.Closing)
        {
            float closeDist = 0.1f;

            //bool Closed = false;


            //CheatBarrier.ActivateColl();

            if ((endpos.transform.position - door.transform.position).magnitude < closeDist)
            {
                door.transform.position = endpos.transform.position;
                return; 
            }


        }
    }

    private void DoorOpen()
    {
        //Debug.Log(
        door.transform.Translate(Vector3.up * _speed * Time.deltaTime); // move left door left
        //doorRight.transform.Translate(Vector3.right * speed * Time.deltaTime);// move right door rigt
    }

    private void DoorClose()
    {
        //Debug.Log("FECHA");
        door.transform.Translate(Vector3.down * _speed * Time.deltaTime); // move left door right
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
