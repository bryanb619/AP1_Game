//using UnityEditor;
using UnityEngine;

/// <summary>
/// Script used for Realtime lightning optimization & Peformance
/// </summary>
public class LightOptimize : MonoBehaviour
{
    [SerializeField] private LightOptimizeData  data;

    private float                               minDist;

    private float                               distance;
  
    private Light                               Lightcomponent;

    public GameObject                           Player;

    //private bool                                _isActive;
    
    private void Start()
    {
        CollectData();
    }

    private void CollectData()
    {
        Lightcomponent = gameObject.GetComponent<Light>();
        Player = GameObject.FindGameObjectWithTag("Player");

        minDist = data.Distance;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLightState();
    }


    private void UpdateLightState()
    {
        distance = Vector3.Distance(Player.transform.position, transform.position);

        //print(distance);
        if (distance >= minDist)
        {
            DisableLight();
        }
        else if (distance <= minDist) 
        {
            EnableLight();
        }
    }

    private void DisableLight()
    {
        Lightcomponent.enabled = false;
        
    }

    private void EnableLight()
    {
        Lightcomponent.enabled = true;
        
    }

 
/*
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        GUIStyle green = new GUIStyle();
        green.normal.textColor = Color.green;

        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;

        switch (_isActive)
        {
            case true:
                {
                    Handles.Label(transform.position + Vector3.up, "Light Active", green);
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
*/
}
