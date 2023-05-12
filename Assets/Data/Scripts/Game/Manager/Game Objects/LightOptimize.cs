// using UnityEditor;

using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Script used for Realtime lightning optimization & Peformance
/// </summary>
public class LightOptimize : MonoBehaviour
{
    [SerializeField] private LightOptimizeData  data;

    //private float                               minDist;

    //private float                               distance;
  
    [FormerlySerializedAs("_lightcomponent")] [SerializeField] private Light              lightcomponent;

    [FormerlySerializedAs("Player")] public GameObject                           player;

    private bool                                _isActive;

    [FormerlySerializedAs("_lightScript")] [SerializeField] private LightFlick         lightScript;
    
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
        lightcomponent.enabled = false;
        lightScript.enabled = false;   

        return;
    }

    private void OnBecameVisible()
    {
        //this.gameObject.gameObject.SetActive(true);
        lightcomponent.enabled = true;
        lightScript.enabled = true;
        
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
