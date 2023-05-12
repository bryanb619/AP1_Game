using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    #region Variables
                        private GameObject              _player;
                        private Transform               _playerTransform;

    [SerializeField]    private LayerMask              obstructionLayer;
    [SerializeField]    private Transform              leftPos, rightPos; 

    [SerializeField]    private float                  alphaValue = 0.3f;

                        private List<MeshRenderer>      _obstructedMesh = new List<MeshRenderer>();


    #endregion

    #region Start
    private void Start()
    {
        CollectData();
    }

    private void CollectData()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.transform;
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
        Vector3 playerPos = _playerTransform.position;

        // camera position
        Vector3 cameraPos = transform.position;
        

        // 
        Vector3 direction = (playerPos - cameraPos).normalized;


        float distance = Vector3.Distance(cameraPos, playerPos);

        RaycastHit[] hits = Physics.RaycastAll(cameraPos, direction, distance, obstructionLayer);

        RaycastHit[] hitsRight = Physics.RaycastAll(rightPos.position, direction, distance, obstructionLayer);

        // raycast
        Debug.DrawLine(cameraPos, playerPos);

    
        // list
        List<MeshRenderer> resetRenderers = new List<MeshRenderer>(); 

        // for each object reduce alpha
        foreach (RaycastHit hit in hits)
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer != null && !_obstructedMesh.Contains(renderer))
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaValue);
                _obstructedMesh.Add(renderer);
            }
        }

        foreach(RaycastHit hit in hitsRight) 
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer != null && !_obstructedMesh.Contains(renderer))
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaValue);
                _obstructedMesh.Add(renderer);
            }
        }


        foreach (MeshRenderer renderer in _obstructedMesh)
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
            
            // remove renderer
            _obstructedMesh.Remove(renderer);
        }

    }
    #endregion
}