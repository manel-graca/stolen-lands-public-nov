using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class LootButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Image icon;
	public Text titleText;
	public Item item;
	public Text itemAmountText;

	public int amount = 1;
	
	
	private Inventory inv;
	private PlayerUI playerUI;
	private PlayerSoundManager soundManager;
	
	private void OnEnable()
	{
		if (item == null) return;
		if (item.itemType == ItemType.Gear)
		{
			item.hasBeenEquipped = true;
		}
		if (amount == 0) amount = 1;
		int maxAmount = item.maxStack;

		if (item.maxStack > 1)
		{
			if(item.itemType == ItemType.Potion)
			{
				amount = Random.Range(1, maxAmount - 1);
				amount = Mathf.Clamp(amount, 1, maxAmount);
				return;
			}
			amount = Random.Range(1, 6);
			amount = Mathf.Clamp(amount, 1, maxAmount);
			return;
		}
		else amount = 1;
	}

	private void Start()
	{
		inv = Inventory.instance;
		soundManager = PlayerSoundManager.instance;
		playerUI = PlayerUI.instance;
		if (item == null) Destroy(gameObject);
		Invoke("AssignTextValues", .02f); // could change script order too
	}

	private void AssignTextValues()
	{
		if (item == null) return;
			
		itemAmountText.text = amount.ToString();
	}
	public void OnPointerEnter(PointerEventData eventData) 
	{ 
		StartCoroutine(ShowTooltip());
	}

	IEnumerator ShowTooltip()
	{
		yield return new WaitForSeconds(playerUI.timeUntillTooltipActivation);
		playerUI.ShowTooltip(transform.position, item);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StopAllCoroutines();
		playerUI.HideTooltip();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (item != null)
		{
			if (item.isCurrency)
			{
				inv.AddToCurrency(item.itemName, amount);
				PlayerUI.instance.HideTooltip();
				LootWindow.instance.RemoveButtonFromWindow(this, item);
				soundManager.PlayInterfaceSound(soundManager.lootItemSound);
				return;
			}
			
			
			bool wasPickedUp = inv.Add(item, amount);
			if (wasPickedUp)
			{
				PlayerUI.instance.HideTooltip();
				LootWindow.instance.RemoveButtonFromWindow(this, item);
				if (item.itemType == ItemType.Gear)
				{
					soundManager.PlayInterfaceSound(soundManager.lootGearSound);
					return;
				}
				soundManager.PlayInterfaceSound(soundManager.lootItemSound);
				return;
			}
			else
			{
				playerUI.InstantiateWarning("You can't add that item to your inventory");
			}
		}
	}
}
