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


    private Vector3 followOffset; 

    private float followMin = 5f; 

    private float followMax = 15f; 

    [SerializeField] private CinemachineVirtualCamera cam; 


    private void Awake()
    { 
        
        followOffset = cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset; 
    }

    void Start()
    {
        //cam = GetComponent<CinemachineVirtualCamera>();

        //targetFOV = 50f; 

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

        Debug.Log(Input.mouseScrollDelta); 

        Vector3 ZOOMDIR =  followOffset.normalized;


        float  ZoomSpeed = 2F;

        if(Input.mouseScrollDelta.y > 0)
        {
            followOffset -= ZOOMDIR; 
        }

        else if(Input.mouseScrollDelta.y < 0)
        {
            followOffset += ZOOMDIR; 
        }

        if(followOffset.magnitude < followMin)
        {
            
            followOffset = ZOOMDIR * followMin; 
        }

        if(followOffset.magnitude> followMax)
        {
            followOffset = ZOOMDIR * followMax; 
        }

       

        cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = 
            Vector3.Lerp(cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, 
            followOffset, Time.deltaTime * ZoomSpeed);
        
        
    }

}

    /*
    private float ZoomAmount  = 0; //With Positive and negative values
    private float minClamp = 5f; 

    private float MaxToClamp  = 15f;
    [SerializeField] private float ROTSpeed = 10;

    public Transform parent; 

    private Vector3 camPos; 

    private void Start()
    {
        transform.SetParent(parent); 
        camPos = new Vector3(0,7,-4); 
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camPos; 


        ZoomAmount += Input.GetAxis("MouseScrollWheel");
        ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);

        float translate = Mathf.Min(Mathf.Abs(Input.GetAxis("MouseScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
        translate = Mathf.Clamp(translate, minClamp, MaxToClamp);

        this.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("MouseScrollWheel")));
    }

    */

