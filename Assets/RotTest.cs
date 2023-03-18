
using UnityEngine;

public class RotTest : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;

    private bool _gameplay; 
    private GameState _gameState;

    [SerializeField] Transform companionAim;
    [SerializeField] Transform end; 

    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    // Create the FSM
    private void Start()
    {

        switch (_gameState)
        {
            case GameState.Paused:
                {
                    _gameplay = false;
                    break;
                }
            case GameState.Gameplay:
                {
                    _gameplay = true;
                    break;
                }
        }
    }

        // Update is called once per frame
    void Update()
    {
        if (_gameplay) 
        {
            Aim();
        }
    }


    private void Aim()
    {

        Ray cameraRay = m_Camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {

            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.magenta);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            companionAim.transform.position = pointToLook;  
        }
    }
    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                {
                    _gameplay = false;
                    break;
                }
            case GameState.Gameplay:
                {
                    _gameplay = true;
                    break;
                }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

}
