using UnityEngine;

public class DoorHandler : MonoBehaviour
{

    //private GameObject door;

    private float speed = 1.5f;

    [SerializeField] private GameObject Door;
    //[SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject EndPos;

    //[SerializeField]private Player_test PlayerScript;

   // private bool PlayerIsActive;


    private void Start()
    {
        //Door = GetComponentInChildren<GameObject>();
        //EndPos = GetComponentInChildren<GameObject>();

        //PlayerScript = FindObjectOfType<Player_test>();
        //PlayerIsActive = true;

        //gameManager = FindObjectOfType<GameManager>();

    }

    private void Update()
    {

        MinimalDistCheck();

    }

    private void MinimalDistCheck()
    {
        float minDist = 1.5f;

        if ((EndPos.transform.position - Door.transform.position).magnitude < minDist)
        {
            Door.transform.position = EndPos.transform.position;
            //PlayerIsActive = true;
            /*
            if (PlayerIsActive == true)
            {
                ActivatePlayer();
            }
            */
        }
    }

    public void OpenDoor()
    {

        //gameManager.PlayerMovement = false;
        Door.transform.Translate(Vector3.up * speed * Time.deltaTime);
        //HandleDoor();   


    }

    /*
    private void HandleDoor()
    {
        PlayerIsActive = false;

        if (PlayerIsActive == false)  
        {
            PlayerScript.CanMove = false;   
        }


    }

    private void ActivatePlayer()
    {
        //PlayerScript.CanMove = true; 
        //gameManager.PlayerMovement = true;
       // print("1st step");

        

    }
    */


}
