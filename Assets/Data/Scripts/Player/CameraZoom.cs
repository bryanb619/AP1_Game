using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    
    float ZoomAmount  = 0; //With Positive and negative values
    float MaxToClamp  = 5;
    float ROTSpeed = 10;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ZoomAmount += Input.GetAxis("MouseScrollWheel");
        ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);

        float translate = Mathf.Min(Mathf.Abs(Input.GetAxis("MouseScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
        this.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("MouseScrollWheel")));
    }

    
}
