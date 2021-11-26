using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using StolenLands.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shop_MasterController : MonoBehaviour
{
    #region Singleton

    public static Shop_MasterController instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    private Item buyBackItem = null;
    private int buyBackAmount = 0;
    private string buyBackCurrency = null;
    
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private GameObject shopArea;
    [SerializeField] private float shopRadius;
    [SerializeField] private float tradeCooldown;

    [SerializeField] private GameObject tooFarWarning;

    public GameObject[] itemCategories;
    
    [SerializeField] private Image buyBackIcon;
    [SerializeField] private GameObject buyBackButton;
    [SerializeField] private TextMeshProUGUI buyBackAmountText;
    
    public bool isShopOpen;
    public bool isSelling;
    public bool isBuying;

    private PlayerUI ui;
    private GameObject player;
    private Inventory inv;

    private void Start()
    {
        ui = PlayerUI.instance;
        inv = Inventory.instance;
        player = GameObject.FindWithTag("Player");
        
        buyBackItem = null;
        buyBackAmount = 0;
        buyBackCurrency = null;
        buyBackAmountText.text = null;
        buyBackIcon.color = Color.clear;
        
        itemCategories[0].SetActive(true);
        itemCategories[1].SetActive(false);
        itemCategories[2].SetActive(false);
        itemCategories[3].SetActive(false);
    }

    private void Update()
    {
        if (shopWindow.activeSelf)
        {
            isShopOpen = true;
        }
        else
        {
            isShopOpen = false;
        }
        
        if (isShopOpen && Vector3.Distance(player.transform.position, shopArea.transform.position) > (shopRadius - 3f))
        {
            tooFarWarning.SetActive(true);
        }
        else
        {
            tooFarWarning.SetActive(false);
        }

        if (isShopOpen && Vector3.Distance(player.transform.position, shopArea.transform.position) > shopRadius)
        {
            ui.OpenCloseMasterShop(false);
        }
    }

    public void SellItem(Inventory inv, string coinName, int price, Item item, bool sellStack)
    {
        ApplyTradeCooldown("Sell");

        if (sellStack)
        {
            var totalItemAmount = inv.GetItemAmount(item);
            var totalPrice = price * totalItemAmount;
            inv.AddToCurrency(coinName, totalPrice);
            inv.Remove(item, totalItemAmount);
            ui.InstantiateInformationMessage("Sold x" + totalItemAmount + " " + item.itemName + " for " + totalPrice + " " + coinName);
            AssignBuyBackItem(item, totalItemAmount, coinName);
            return;
        }

        ui.InstantiateInformationMessage("Sold 1 " + item.itemName + " for " + price + " " + coinName);
        inv.AddToCurrency(coinName, price);
        inv.Remove(item, 1);
        AssignBuyBackItem(item, 1, coinName);
    }

    private void AssignBuyBackItem(Item item, int amount, string currency)
    {
        buyBackItem = item;
        buyBackAmount = amount;
        buyBackCurrency = currency;
        buyBackAmountText.text = amount.ToString();

        buyBackButton.SetActive(true);
        buyBackIcon.sprite = item.icon;
        buyBackIcon.color = Color.white;
    }

    public void CleanBuyBackItem()
    {
        buyBackItem = null;
        buyBackAmount = 0;
        buyBackCurrency = null;
        buyBackAmountText.text = null;

        buyBackButton.SetActive(false);
        buyBackIcon.sprite = null;
        buyBackIcon.color = Color.clear;
    }

    public void BuyBackSoldItem()
    {
        if (buyBackItem == null || buyBackAmount <= 0) return;
        
        ApplyTradeCooldown("Buy");

        if (buyBackItem.maxStack <= 1)
        {
            buyBackItem.hasBeenEquipped = true;
            inv.Add(buyBackItem, 1);
            inv.RemoveFromCurrency(buyBackCurrency, buyBackItem.sellPrice);
            CleanBuyBackItem();
            return;
        }

        var priceToSubtract = buyBackItem.sellPrice * buyBackAmount;

        inv.Add(buyBackItem, buyBackAmount);
        inv.RemoveFromCurrency(buyBackCurrency, priceToSubtract);
        CleanBuyBackItem();
    }

    public void ApplyTradeCooldown(string tradeType)
    {
        if (tradeType == "Sell" || tradeType == "sell")
        {
            StartCoroutine(ApplySellRoutine());
            return;
        }
        if (tradeType == "Buy" || tradeType == "buy")
        {
            StartCoroutine(ApplyBuyRoutine());
            return;
        }
    }

    IEnumerator ApplyBuyRoutine()
    {
        isBuying = true;
        yield return new WaitForSeconds(tradeCooldown);
        isBuying = false;
    }
    IEnumerator ApplySellRoutine()
    {
        isSelling = true;
        yield return new WaitForSeconds(tradeCooldown);
        isSelling = false;

    }        

}

