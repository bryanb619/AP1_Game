using UnityEngine;
using UnityEngine.Serialization;

public class RotateObject : MonoBehaviour
{
    [FormerlySerializedAs("_rotate")]
    [Header("Rotation")]

    [SerializeField]
    private bool            rotate;

    [FormerlySerializedAs("_rotateSpeed")] [SerializeField]
    private float           rotateSpeed;

    private bool            _gamePlay;


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }



    // Update is called once per frame
    void Update()
    {

        if(rotate && _gamePlay) 
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
