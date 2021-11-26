using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftedItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    private CraftingRecipe parentRecipe;
    private PlayerUI ui;
    
    private void Start()
    {
        ui = PlayerUI.instance;
        parentRecipe = GetComponentInParent<CraftingRecipe>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowTooltipRoutine());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.HideTooltip();
        StopAllCoroutines();
    }

    IEnumerator ShowTooltipRoutine()
    {
        yield return new WaitForSeconds(ui.timeUntillTooltipActivation);
        ui.ShowTooltip(transform.position, parentRecipe.itemToCraft);
    }
}
