using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void Start()
    {
        gameObject.transform.GetChild(0).GameObject().SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (gameObject.transform.GetChild(0).GameObject().activeSelf)
        {
            gameObject.transform.GetChild(0).transform.position = Input.mousePosition;
        }*/
    }

    public void ShowTooltip()
    {
        gameObject.transform.GetChild(0).GameObject().SetActive(true);
    }
    
    public void HideTooltip()
    {
        gameObject.transform.GetChild(0).GameObject().SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }
}
