using System.Collections;
using UnityEngine;

public class CompanionSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _companion;

    private CompanionBehaviour _companionBehaviour;

    [SerializeField] private float SpawnTime = 1.5f;

    private bool _gamePlay;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        Spawn(); 
    }

    private void Spawn()
    {
        /*
        if(_gamePlay)
        {
            if (Input.GetKeyUp(KeyCode.L)) 
            {
                _companionBehaviour = FindObjectOfType<CompanionBehaviour>();

                _companionBehaviour.Replace(); 

                StartCoroutine(Timer());
                

            }
           
        } */
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(SpawnTime);

        Instantiate(_companion, transform.position, Quaternion.identity);
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

        //throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
