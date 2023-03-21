using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    #region Variables
    private GameObject player;

    private Transform playerTransform;

    [SerializeField] private LayerMask obstructionLayer;

    [SerializeField] private float alphaValue = 0.3f;

    private List<MeshRenderer> meshInObstruction = new List<MeshRenderer>();

    #endregion

    #region Start
    private void Start()
    {
        CollectData();
    }

    private void CollectData()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    #endregion

    #region Update
    private void Update()
    {
        UpdateRaycast();
    }

    

    /// <summary>
    /// Raycast Update of player visibility
    /// This code manages other Game Objects mesh renderes alpha and will lower and reset according to player visibilty between camera and player
    /// </summary>
    private void UpdateRaycast()
    {
        // player position 
        Vector3 playerPos = playerTransform.position;

        // camera position
        Vector3 cameraPos = transform.position;
        

        // 
        Vector3 direction = (playerPos - cameraPos).normalized;


        float distance = Vector3.Distance(cameraPos, playerPos);

        RaycastHit[] hits = Physics.RaycastAll(cameraPos, direction, distance, obstructionLayer); // raycast
        //Debug.DrawLine(cameraPosition, playerPosition);

        // list
        List<MeshRenderer> resetRenderers = new List<MeshRenderer>(); 

        // for each object reduce alpha
        foreach (RaycastHit hit in hits)
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer != null && !meshInObstruction.Contains(renderer))
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaValue);
                meshInObstruction.Add(renderer);
            }
        }


        foreach (MeshRenderer renderer in meshInObstruction)
        {
            bool isObstructed = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<MeshRenderer>() == renderer)
                {
                    isObstructed = true;
                    break;
                }
            }
            // add renderer to remove list
            if (!isObstructed)
            {
                resetRenderers.Add(renderer);
            }
        }

        // Reset alpha value
        foreach (MeshRenderer renderer in resetRenderers)
        {
            
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f);
            meshInObstruction.Remove(renderer);
        }

    }
    #endregion
}