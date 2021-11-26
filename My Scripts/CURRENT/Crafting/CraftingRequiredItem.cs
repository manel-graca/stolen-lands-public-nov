using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingRequiredItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("0 if is the first in the row, 4 if it's the last, and so on")] [Range(0, 4)]
    public int orderID;

    public Item item;
    public Image itemIcon;
    public TextMeshProUGUI amountText;

    public CraftingRecipe parentRecipe;
    public bool destroyOnCraft = false;
    [HideInInspector] public int itemAmount = 0;

    private CraftingManager craftManager;
    private Inventory inventory;
    private PlayerUI ui;

    private void Start()
    {
        craftManager = FindObjectOfType<CraftingManager>();
        inventory = Inventory.instance;
        ui = PlayerUI.instance;

        itemIcon.sprite = item.icon;
        amountText.text = String.Format("{0}/{1}", inventory.GetItemAmount(item).ToString(),
            parentRecipe.reqItemsEachAmount[orderID].ToString());
    }

    private void Update()
    {
        itemAmount = inventory.GetItemAmount(item);
        
        if (itemAmount <= 0)
        {
            parentRecipe.canCraft = false;
        }
        
        if (itemAmount >= parentRecipe.reqItemsEachAmount[orderID])
        {
            amountText.text = String.Format("<color=#59981A>{0}/{1}</color>", inventory.GetItemAmount(item).ToString(),
                parentRecipe.reqItemsEachAmount[orderID].ToString());
            parentRecipe.canCraft = true;
        }
        else
        {
            amountText.text = String.Format("<color=#FFFFFF>{0}/{1}</color>", inventory.GetItemAmount(item).ToString(),
                parentRecipe.reqItemsEachAmount[orderID].ToString());
            parentRecipe.canCraft = false;
        }

        if (parentRecipe.GetIfNeedToUpdateData())
        {
            amountText.text = String.Format("{0}/{1}", inventory.GetItemAmount(item).ToString(),
                parentRecipe.reqItemsEachAmount[orderID].ToString());
        }
    }

    public int GetItemAmount()
    {
        return itemAmount;
    }

    public void OnCrafting()
    {
        if (destroyOnCraft)
        {
            inventory.Remove(item, parentRecipe.reqItemsEachAmount[orderID]);
        }
        
        amountText.text = String.Format("{0}/{1}", inventory.GetItemAmount(item).ToString(),
            parentRecipe.reqItemsEachAmount[orderID].ToString());
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
        ui.ShowTooltip(transform.position, item);
    }
}