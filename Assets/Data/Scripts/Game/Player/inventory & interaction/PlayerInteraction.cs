using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    private const float MaxInteractionDistance = 1.0f;


    [FormerlySerializedAs("_canvasManager")] [SerializeField] private CanvasManager canvasManager;

    [FormerlySerializedAs("_cameraTransform")] [SerializeField]private Transform cameraTransform;
    private Interactive _currentInteractive;
    private bool _playerHasRequirements;
    private List<Interactive> _inventory;
     


    [FormerlySerializedAs("Pick_Up")] [SerializeField]
    private AudioSource pickUp;

    void Start()
    {
       // _cameraTransform = FindObjectOfType<Camera>().transform;
        _currentInteractive = null;
        _playerHasRequirements = false;
        _inventory = new List<Interactive>();
    }

    void Update()
    {
        LookForInteractive();
        CheckForPlayerInteraction();
    }

    private void LookForInteractive()
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward,
                            out RaycastHit hitInfo, MaxInteractionDistance))
        {
            Interactive interactive = hitInfo.collider.GetComponent<Interactive>();

            if (interactive == null || !interactive.IsActive())
                ClearCurrentInteractive();
            else if (interactive != _currentInteractive)
                SetCurrentInteractive(interactive);
        }
        else
            ClearCurrentInteractive();
    }

    private void ClearCurrentInteractive()
    {
        _currentInteractive = null;

        canvasManager.HideInteractionPanel();
    }

    private void SetCurrentInteractive(Interactive interactive)
    {
        _currentInteractive = interactive;

        if (PlayerHasInteractionRequirements())
            canvasManager.ShowInteractionPanel(interactive.GetCurrentInteractionText());
        else
            canvasManager.ShowInteractionPanel(interactive.GetRequirementText());

    }

    private bool PlayerHasInteractionRequirements()
    {
        _playerHasRequirements = false;

        Interactive[] requirements = _currentInteractive.GetRequirements();

        if (requirements != null)
            for (int i = 0; i < requirements.Length; ++i)
                if (!IsInInventory(requirements[i]))
                    return false;

        _playerHasRequirements = true;
        return true;
    }

    private void CheckForPlayerInteraction()
    {
        // interact key is F
        if (Input.GetButtonDown("Interact") && _currentInteractive != null && _playerHasRequirements)
        {
            if (_currentInteractive.GetInteractiveType() == Interactive.InteractiveType.Pickable)
                PickCurrentInteractive();


            else
                InteractWithCurrentInteractive();

        }

    }


    private void PickCurrentInteractive()
    {
        _currentInteractive.Interact();
        

        //int[] intArray;
        AddToInventory(_currentInteractive);
     
    }

    private void AddToInventory(Interactive item)
    {
        
        //_inventory.Add(item);
        //_canvasManager.SetInventoryIcon(_inventory.Count - 1, item.GetIcon());
        //Pick_Up.Play();

    }

    private void RemoveFromInventory(Interactive item)
    {
        //_inventory.Remove(item);
        //_canvasManager.ClearInventoryIcons();

        //for (int i = 0; i < _inventory.Count; ++i)
            //_canvasManager.SetInventoryIcon(i, _inventory[i].GetIcon());

    }

    private bool IsInInventory(Interactive item)
    {
        return _inventory.Contains(item);
    }

    private void InteractWithCurrentInteractive()
    {
        Interactive[] requirements = _currentInteractive.GetRequirements();

        if (requirements != null)
        {
            for (int i = 0; i < requirements.Length; ++i)
            {
                requirements[i].gameObject.SetActive(true);
                RemoveFromInventory(requirements[i]);
            }
        }

        _currentInteractive.Interact();
    }
}
