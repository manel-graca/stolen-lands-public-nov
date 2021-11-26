using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop_BuyItemMaster : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public Item item;
    public int amountToGive;
    [Space]
    [Space] 
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemCostText;
    public TextMeshProUGUI amountToGiveText;
    public Image itemIcon;
    public Image coinType;
    public Sprite[] coinTypes;

    private string requiredCurrencyName;
    private int requiredCurrencyAmount;
    
    private Inventory playerInv;
    private PlayerUI ui;

    
    private void Start()
    {
        ui = PlayerUI.instance;
        playerInv = Inventory.instance;
        requiredCurrencyAmount = item.buyPrice;
        requiredCurrencyName = item.currencyToGive;
        
        if (requiredCurrencyName == "Copper")
        {
            coinType.sprite = coinTypes[0];
        }
        if (requiredCurrencyName == "Silver")
        {
            coinType.sprite = coinTypes[1];
        }
        if (requiredCurrencyName == "Gold")
        {
            coinType.sprite = coinTypes[2];
        }
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemCostText.text = requiredCurrencyAmount.ToString();
        amountToGiveText.text = amountToGive.ToString();
        
    }

    private bool GetIfCanBuy()
    {
        var audio = PlayerSoundManager.instance;
        var clip = audio.errorClickSound;
        
        if (item.maxStack <= 1)
        {
            if (playerInv.HasItem(item))
            {
                audio.PlayInterfaceSound(clip);
                return false;
            }
        }
        var master = GetComponentInParent<Shop_MasterController>();

        if (playerInv.GetCurrencyOwned(requiredCurrencyName) >= requiredCurrencyAmount)
        {
            return true;
        }
        audio.PlayInterfaceSound(clip);
        return false;
    }

    public void BuyItem()
    {
        if (GetIfCanBuy())
        {
            if (item.itemType == ItemType.Gear)
            {
                item.hasBeenEquipped = true;
            }
            
            playerInv.RemoveFromCurrency(requiredCurrencyName, requiredCurrencyAmount);
            playerInv.Add(item, amountToGive);

            GetComponentInParent<Shop_MasterController>().ApplyTradeCooldown("Buy");
            return;
        }

        var audio = PlayerSoundManager.instance;
        var clip = audio.errorClickSound;
        ui.InstantiateWarning("Not enough money");
        audio.PlayInterfaceSound(clip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        ui.HideTooltip();
    }
    
    IEnumerator ShowTooltip()
    {
        yield return new WaitForSeconds(ui.timeUntillTooltipActivation);
        ui.ShowTooltip(transform.position, item);
    }
}
