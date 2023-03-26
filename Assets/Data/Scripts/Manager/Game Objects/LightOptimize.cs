// using UnityEditor;

using UnityEngine;

/// <summary>
/// Script used for Realtime lightning optimization & Peformance
/// </summary>
public class LightOptimize : MonoBehaviour
{
    [SerializeField] private LightOptimizeData  data;

    //private float                               minDist;

    //private float                               distance;
  
    [SerializeField] private Light              _lightcomponent;

    public GameObject                           Player;

    private bool                                _isActive;

    [SerializeField] private LightFlick         _lightScript;
    
    private void Start()
    {
        CollectData();
    }

    private void CollectData()
    {
        //Lightcomponent = gameObject.GetComponent<Light>();
        //Player = GameObject.FindGameObjectWithTag("Player");

        //minDist = data.Distance;

        //_lightcomponent = GetComponentInChildren<Light>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        //UpdateLightState();
    }


    private void OnBecameInvisible()
    {
        //this.gameObject.SetActive(false);
        _lightcomponent.enabled = false;
        _lightScript.enabled = false;   

        return;
    }

    private void OnBecameVisible()
    {
        //this.gameObject.gameObject.SetActive(true);
        _lightcomponent.enabled = true;
        _lightScript.enabled = true;
        
        return;
    }

    /*
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

    */
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
