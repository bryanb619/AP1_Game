using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [FormerlySerializedAs("_interactionPanel")] [SerializeField] private GameObject interactionPanel;
    [FormerlySerializedAs("_interactionText")] [SerializeField] private Text interactionText;
    [FormerlySerializedAs("_inventoryIcons")] [SerializeField] private Image[] inventoryIcons;

    public static bool InventoryActive = false;
    [FormerlySerializedAs("Inventory_Canvas")] public GameObject inventoryCanvas;

    void Start()
    {
        HideInteractionPanel();
    }
    private void Update()
    {
        // look for tab to activate/deactivate inventory
        //LookForTAB_KeyCode();

    }

    public void HideInteractionPanel()
    {
        interactionPanel.SetActive(false);
    }

    // Activate Interaction Panel with X message
    public void ShowInteractionPanel(string message)
    {
        interactionText.text = message;
        interactionPanel.SetActive(true);


    }


    public void SetInventoryIcon(int i, Sprite icon)
    {
        inventoryIcons[i].sprite = icon;
        inventoryIcons[i].color = Color.white;
    }

    public void ClearInventoryIcons()
    {
        // if(numbers!= null)
        for (int i = 0; i < inventoryIcons.Length; ++i)
        {
            inventoryIcons[i].sprite = null;
            inventoryIcons[i].color = Color.clear;
        }



    }
    private void LookForTAB_KeyCode()
    {
        // Get Tab input to manage inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // is active hide inventory
            if (InventoryActive)
            {
                Hide_Inventory();
            }

            // if not active show inventory
            else
            {
                Show_Inventory();
            }
        }
    }


    // Verificar a necessidade de inventario

    public void Hide_Inventory()
    {
        // canvas settings
        inventoryCanvas.SetActive(false);
        InventoryActive = false;

        /* cursor settings
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */
    }

    public void Show_Inventory()
    {
        // Canvas settings
        inventoryCanvas.SetActive(true);
        InventoryActive = true;

        /* cursor setting
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */
    }
}
