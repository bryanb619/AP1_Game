using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TooltipManager tooltipManager;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipManager.ShowTooltip();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }

}
