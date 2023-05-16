using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation")]

    [SerializeField]
    private bool            rotate;

    [SerializeField]
    private float           rotateSpeed;

    private GameState       _gamePlay;


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }



    // Update is called once per frame
    void Update()
    {

        if(rotate && _gamePlay == GameState.Gameplay) 
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime );
        }
 
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                {
                    _gamePlay = GameState.Gameplay;
                    break;
                }
            case GameState.Paused:
                {
                    _gamePlay = GameState.Paused;
                    break;
                }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
