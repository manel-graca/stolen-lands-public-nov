using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipe : MonoBehaviour
{
    public GameObject requiredObjectNearby;
    public string requiredObjectName = null;
    public TextMeshProUGUI requiredObjectText;
    public float maxCraftingStationDistance;
    [Space]
    public Item[] requiredItems;
    public CraftingRequiredItem[] requiredItemsScript;
    public int[] reqItemsEachAmount;

    [Space]
    public Item itemToCraft;
    public int itemToCraftAmount;

    [Space]
    public float onCraftXPReward;
    public float craftingTime;
    public TextMeshProUGUI craftingTimeText;
    public Image craftTimeImage;
    
    public GameObject cancelCraftButton;
    public GameObject beginCraftingButton;

    [Space]
    public TextMeshProUGUI itemToCraftName;
    public Image itemToCraftIcon;
    public TextMeshProUGUI itemToCraftAmountText;

    public bool isNearbyRequiredObject = false;
    public bool canCraft;
    
    private Inventory inventory;
    private PlayerUI ui;
    private PlayerCombatController pCombat;
    private PlayerSoundManager soundM;
    
    private float timeSinceObjectCheck = Mathf.Infinity;
    private const float timeBetweenObjectChecks = 0.3f;

    private void Start()
    {
        inventory = Inventory.instance;
        ui = PlayerUI.instance;
        pCombat = PlayerCombatController.instance;
        soundM = PlayerSoundManager.instance;
        
        ResetCraftingCountdowns();
        cancelCraftButton.SetActive(false);
        
        itemToCraftAmountText.text = itemToCraftAmount.ToString();
        itemToCraftName.text = itemToCraft.itemName;
        itemToCraftIcon.sprite = itemToCraft.icon;
        if (requiredObjectNearby != null)
        {
            requiredObjectText.text = "No " + requiredObjectNearby.GetComponent<CraftingRequiredObject>().displayName.ToString() + " nearby";
        }
        else
        {
            requiredObjectText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (timeSinceObjectCheck > timeBetweenObjectChecks)
        {
            timeSinceObjectCheck = 0f;
            GetIfNearRequiredObject();
        }
        
        timeSinceObjectCheck += Time.deltaTime;
    }

    public bool GetIfNeedToUpdateData()
    {
        if (ui.craftingOn)
        {
            return true;
        }
        else return false;
    }

    public void CraftItem()
    {
        if (GetIfCanCraft())
        {
            craftTimeImage.fillAmount = 0f;
            StartCoroutine(CraftItemRoutine());
        }
        else
        {
            ui.InstantiateWarning("You don't have the required items");
            return;
        }
    }

    private bool GetIfNearbyRequiredObject()
    {
        return Vector3.Distance(pCombat.transform.position, requiredObjectNearby.transform.position) <
            maxCraftingStationDistance;
    }

    private void GetIfNearRequiredObject()
    {
        if (requiredObjectNearby != null)
        {
            if (GetIfNearbyRequiredObject())
            {
                requiredObjectText.gameObject.SetActive(false);
                beginCraftingButton.SetActive(true);
            }
            else
            {
                requiredObjectText.gameObject.SetActive(true);
                beginCraftingButton.SetActive(false);
            }
        }
    }
    
    private bool GetIfCanCraft() 
    {
        for ( int i = 0; i < requiredItemsScript.Length; ++i ) 
        {
            if (requiredItemsScript[i].GetItemAmount() < reqItemsEachAmount[i]) {
                return false;
            }
        }
        return true;
    }


    public void CancelCrafting()
    {
        StopAllCoroutines();
        cancelCraftButton.SetActive(false);
        beginCraftingButton.SetActive(true);
        ResetCraftingCountdowns();
        
    }

    private void ResetCraftingCountdowns()
    {
        craftTimeImage.fillAmount = 0f;
        craftingTimeText.text = null;
    }

    IEnumerator CraftItemRoutine()
    {
        var clip = PlayerSoundManager.instance.itemCraftedSound;
        
        beginCraftingButton.SetActive(false);
        cancelCraftButton.SetActive(true);
        itemToCraftAmountText.gameObject.SetActive(false);
        
        float t = 0f;
        float i = 0f;
        while (t < 1f)
        {
            i += Time.deltaTime;
            t += Time.deltaTime / craftingTime;
            
            i = Mathf.Clamp(i,0f,craftingTime);
            
            craftTimeImage.fillAmount = Mathf.Lerp(0, 1, t);
            craftingTimeText.text = i.ToString("F") + "/" + craftingTime.ToString() + " s";
            yield return null;
            
        }
        
        yield return t >= 1f;
        
        ResetCraftingCountdowns();
        cancelCraftButton.SetActive(false);
        beginCraftingButton.SetActive(true);
        itemToCraftAmountText.gameObject.SetActive(true);
        foreach (var a in requiredItemsScript)
        {
            a.OnCrafting();
        }

        if (itemToCraft.itemType == ItemType.Gear)
        {
            itemToCraft.hasBeenEquipped = true;
        }
        inventory.Add(itemToCraft, itemToCraftAmount);
        PlayerResources.instance.AddToExperience(onCraftXPReward);
        soundM.PlayInterfaceSound(clip);
    }
}