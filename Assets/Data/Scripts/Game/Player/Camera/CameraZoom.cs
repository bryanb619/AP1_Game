using System;
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

                        private Vector3                     _followOffset; 

    [Header("Zoom Settings")]
    [Range(5,25)]
    [SerializeField]    private float                       followMin = 13f; 

    [Range(10,35)]
    [SerializeField]    private float                       followMax = 20f; 

    [SerializeField]    private CinemachineVirtualCamera    cam;

                        private GameState                   _state;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        _followOffset = cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset; 
        CinemachineCollider collider = cam.GetComponentInChildren<CinemachineCollider>();
    }

    void Start()
    {
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void LateUpdate()
    {
        HandleTransformZoom();
    }


    /// <summary>
    ///  ZOOM Handle function
    /// </summary>
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

            if (_followOffset.magnitude < followMin)
            {

                _followOffset = zoomdir * followMin;
            }

            if (_followOffset.magnitude > followMax)
            {
                _followOffset = zoomdir * followMax;
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
