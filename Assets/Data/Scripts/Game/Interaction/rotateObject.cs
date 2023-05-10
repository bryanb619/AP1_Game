using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation")]

    [SerializeField]
    private bool            _rotate;

    [SerializeField]
    private float           _rotateSpeed;

    private bool            _gamePlay;


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }



    // Update is called once per frame
    void Update()
    {

        if(_rotate && _gamePlay) 
        {
            transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime );
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
}
