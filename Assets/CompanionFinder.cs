using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CompanionFinder : MonoBehaviour
{

    private RaycastHit hit;
    [SerializeField] private Transform _CameraPos;


    // there is something obstructing the view.
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        var rayDirection = _CameraPos.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection, hit))
        {
            if (hit.transform == player)
            {
                // enemy can see the player!
            }
        }
        else
        {
        }*/
    }

}
