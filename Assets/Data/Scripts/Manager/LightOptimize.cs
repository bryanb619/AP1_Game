using UnityEditor;
using UnityEngine;

public class LightOptimize : MonoBehaviour
{
    public float maxDistance;
    private float distance;
    private Light Lightcomponent;
    public GameObject Player;

    private bool _isActive; 

    [SerializeField] private LightOptimizeData data;

    
    private void Start()
    {
        Lightcomponent = gameObject.GetComponent<Light>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        distance = Vector3.Distance(Player.transform.position, transform.position);

        print(distance);

       if(distance <= maxDistance) 
       {
            Lightcomponent.enabled = false;
            _isActive = false;
       }
       else if(distance >= maxDistance) 
       {
            Lightcomponent.enabled = true;
            _isActive = true;
       }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        GUIStyle white = new GUIStyle();
        white.normal.textColor = Color.white;

        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;

        switch (_isActive)
        {
            case true:
                {
                    Handles.Label(transform.position + Vector3.up, "Light Active", white);
                    break;
                }
            case false:
                {
                    Handles.Label(transform.position + Vector3.up, "Light Deactivated", red);
                    break; 
                }
        }
        

    }

#endif
}
