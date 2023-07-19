using UnityEngine;
using Cinemachine; 

public class CameraZoom : MonoBehaviour
{
/*

    [Header("Zoom Settings")]
    [Tooltip("Values change only camera FOV")]

    [SerializeField] private float zoomMin = 10f; 

    [Tooltip("Values change only camera FOV")]  

    [SerializeField] private float zoomMax = 60f; 

    [SerializeField] private float zoomSpeed = 5f; 
    private float targetFOV; 

*/

    private Vector3 _followOffset; 

    private float _followMin = 8f; 

    private float _followMax = 15f; 

    [SerializeField] private CinemachineVirtualCamera cam;

    private GameState _state;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        _followOffset = cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset; 
        CinemachineCollider collider = cam.GetComponentInChildren<CinemachineCollider>();
    }

    void Start()
    {
        //cam = GetComponent<CinemachineVirtualCamera>();

       
       
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }

        switch(_state)
        {
            case GameState.Gameplay: 
                {
                    _state = GameState.Gameplay; break;
                }
            case GameState.Paused: 
                {
                    _state = GameState.Paused; break;
                }
        }
    }


    void Update()
    {
       //HandleZoom();
       HandleTransformZoom(); 
       
       //transform.LookAt(target); 
   
    }


/// <summary>
///  ZOOM Handle function
/// </summary>
    private void HandleZoom()
    {
        /*
        float fiedOfViewIncreaseAmount = 5f; 

        if(Input.mouseScrollDelta.y >0)
        {
            targetFOV -= fiedOfViewIncreaseAmount;

        }
        else if(Input.mouseScrollDelta.y <0)
        {
            targetFOV += fiedOfViewIncreaseAmount; 
             
        }
        
        targetFOV = Mathf.Clamp(targetFOV, zoomMin, zoomMax);

        cvc.m_Lens.FieldOfView = targetFOV = Mathf.Lerp(cvc.m_Lens.FieldOfView, targetFOV,  zoomSpeed * Time.deltaTime);

    */

    }

    private void HandleTransformZoom()
    {
        if(_state == GameState.Gameplay) 
        {
            Vector3 zoomdir = _followOffset.normalized;


            float zoomSpeed = 1.5F;

            if (Input.mouseScrollDelta.y > 0)
            {
                _followOffset -= zoomdir;
            }

            else if (Input.mouseScrollDelta.y < 0)
            {
                _followOffset += zoomdir;
            }

            if (_followOffset.magnitude < _followMin)
            {

                _followOffset = zoomdir * _followMin;
            }

            if (_followOffset.magnitude > _followMax)
            {
                _followOffset = zoomdir * _followMax;
            }

            
            cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                _followOffset, Time.deltaTime * zoomSpeed);
        }

    }

    private void GameManager_OnGameStateChanged(GameState state)
    {

        switch (state)
        {
            case GameState.Gameplay:
                {
                    _state = GameState.Gameplay;
                    break;
                }
            case GameState.Paused:
                {
                    _state = GameState.Paused;    
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
