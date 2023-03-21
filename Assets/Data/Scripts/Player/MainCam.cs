using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    private GameObject player;

    private Transform playerTransform;

    [SerializeField] private LayerMask obstructionLayer;

    [SerializeField] private float alphaValue = 0.3f;

    private List<MeshRenderer> obstructedRenderers = new List<MeshRenderer>();

    private void Start()
    {
        CollectData();
    }
    private void Update()
    {
        UpdateRaycast();
    }

    private void CollectData()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    /// <summary>
    /// Raycast Update of player visibility
    /// This code manages other Game Objects mesh renderes alpha and will lower and reset according to player visibilty between camera and player
    /// </summary>
    private void UpdateRaycast()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        Vector3 direction = (playerPosition - cameraPosition).normalized;
        float distance = Vector3.Distance(cameraPosition, playerPosition);

        RaycastHit[] hits = Physics.RaycastAll(cameraPosition, direction, distance, obstructionLayer);
        Debug.DrawLine(cameraPosition, playerPosition);

        List<MeshRenderer> resetRenderers = new List<MeshRenderer>(); // Create a new list to keep track of renderers to reset

        foreach (RaycastHit hit in hits)
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer != null && !obstructedRenderers.Contains(renderer))
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaValue);
                obstructedRenderers.Add(renderer);
            }
        }

      
        foreach (MeshRenderer renderer in obstructedRenderers)
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
            if (!isObstructed)
            {
                resetRenderers.Add(renderer);
            }
        }

        // Reset the alpha value of renderers in the reset list
        foreach (MeshRenderer renderer in resetRenderers)
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f);
            obstructedRenderers.Remove(renderer);
        }

    }
}