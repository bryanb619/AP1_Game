using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private Player_test PlayerScript;
    private GameObject Player; 

    [HideInInspector] public bool PlayerMovement;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = FindObjectOfType<Player_test>();
        PlayerMovement = true;


    }


    private void Update()
    {

        UpdatePlayer();
        
    }


    private void UpdatePlayer()
    {
        
        if(PlayerMovement)
        {
            PlayerScript.CanMove = true; 
            Player.SetActive(true);
        }
        else
        {
            PlayerScript.CanMove= false;
        }



    }


}



/* Testing New Game Manager
public static GameManager Instance;

public GameState State;

private GameObject player;


public static event Action<GameState> OnGameStateChanged;


private void Awake ()
{
    Instance= this;


}

private void Start()
{
    player.GetComponent<Player_test>().enabled = true;
    UpdateGameState(GameState.None);
}

public void UpdateGameState(GameState newstate)
{
    State = newstate;

    switch(newstate)
    {
        case GameState.DoorCutscene:
            DoorScene();
            break;

        default:
            break;  

    }

    OnGameStateChanged?.Invoke(newstate);
}


private void DoorScene()
{

    player.GetComponent<Player_test>().enabled = false; 
}


public enum GameState
{
    None,
    DoorCutscene

}
*/






