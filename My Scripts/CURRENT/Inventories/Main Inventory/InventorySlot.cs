using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using StolenLands.Player;
using System.Collections;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	public Image icon;
	public Text itemAmountText;
	public Item item;
	public int itemAmount;
	public bool isDefault;
	[SerializeField] private Image background;
	private Color notHighlightColor;
	Inventory inventory;
	PlayerUI playerUI;
	ItemMoverUI itemMover;
	private PlayerSoundManager soundM;

	private void Start()
	{
		itemAmountText.text = String.Empty;
		notHighlightColor = background.color;
		inventory = Inventory.instance;
		playerUI = PlayerUI.instance;
		itemMover = ItemMoverUI.instance;
		soundM = PlayerSoundManager.instance;
	}

	private void Update()
	{
		if (item == null)
		{
			itemAmountText.text = null;
			icon.enabled = false;
			return;
		}
		if (item != null)
		{
			icon.enabled = true;
			icon.sprite = item.icon;
			if (item.maxStack <= 1)
			{
				itemAmountText.text = null;
				return;
			}
			itemAmountText.text = itemAmount.ToString() + "/" + item.maxStack.ToString();
			return;
		}
	}


	public int GetItemAmount()
	{
		return itemAmount;
	}
	public void AddItemToSlot(Item newItem)
	{
		item = newItem;
		icon.enabled = true;
		icon.sprite = item.icon;
		itemAmount = item.amount;
		if (item.itemType == ItemType.Gear) return;
		
		itemAmountText.text = itemAmount.ToString() + "/" + item.maxStack.ToString();
	}
	public void ClearSlot()
	{
		if (item != null)
		{
			item = null;
			icon.sprite = null;
			icon.enabled = false;
			itemAmountText.text = null;
			Debug.Log("cleaning slot");
		}
	}
	public void UseItem()
	{
		if (item != null)
		{
			if (item.itemType == ItemType.Gear)
			{
				playerUI.InstantiateWarning("You can't use this item!");
				return;
			}
			else
			{
				item.Use();
				//itemAmountText.text = itemAmount.ToString() + "/" + item.maxStack.ToString();
			}
		}
	}

	public void OnRemoveButton()
	{
		if (item == null) return;
		inventory.Remove(item, 1);
	}

	private void HandleItemMover()
	{
		if (itemMover.hasItemMoving && item == null)
		{
			if (itemMover.movingItem.itemType == ItemType.Gear)
			{
				var gear = (Gear) itemMover.movingItem;
				if (gear.gearType == GearType.Bag)
				{
					var b = (Bag) itemMover.movingItem;
					icon.sprite = b.icon;
						
					if (b.hasBeenEquipped)
					{
						inventory.Add(b, 1);
						itemMover.RemoveFromMover();
						return;
					}
					AddItemToSlot(itemMover.movingItem);
					if (item != null)
					{
						itemMover.RemoveFromMover();
					}
					return;
				}
				if (gear.gearType == GearType.Weapon)
				{
					item = itemMover.movingItem;
					icon.sprite = item.icon;
					if (item.hasBeenEquipped)
					{
						inventory.Add(item, 1);
						itemMover.RemoveFromMover();
						return;
					}
				}
			}
			AddItemToSlot(itemMover.movingItem);
			item = itemMover.movingItem;
			icon.sprite = item.icon;
			itemMover.RemoveFromMover();
			return;
		}

		if (!itemMover.hasItemMoving)
		{
			if(item != null)
			{
				if (item.itemType == ItemType.Gear)
				{
					itemMover.hasItemMoving = true;
					itemMover.movingItem = item;
					itemMover.movingItem.amount = 1;
					itemMover.GetComponent<Image>().sprite = item.icon;
					itemMover.GetComponent<Image>().color = Color.white;
					ClearSlot();
					return;
				}

				var amountToMove = item.amount;
				itemMover.hasItemMoving = true;
				itemMover.movingItem = item;
				itemMover.movingItem.amount = amountToMove;
				itemMover.GetComponent<Image>().sprite = item.icon;
				itemMover.GetComponent<Image>().color = Color.white;
				ClearSlot();
				return;
			}
		}
		else return;
	}
	
	

	public void OnPointerDown(PointerEventData eventData)
	{
		
		
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			HandleItemMover();
			soundM.PlayMouseClickSlot();
		}
		else if (eventData.button == PointerEventData.InputButton.Middle)
		{
			if (item == null)
			{
				return;
			}
			
			var shop = Shop_MasterController.instance;
			
			if (shop.isShopOpen)
			{
				shop.SellItem(inventory, item.currencyToGive, item.sellPrice, item, false);
				// do stuff
				return;
			}
			else
			{
				OnRemoveButton();
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (item == null)
			{
				return;
			}
			
			var shop = Shop_MasterController.instance;
			
			if (shop.isShopOpen)
			{
				Debug.Log(inventory.GetItemAmount(item));
				
				shop.SellItem(inventory, item.currencyToGive, item.sellPrice, item, true);
				ClearSlot();
				// do stuff
				return;
			}
			else
			{
				UseItem();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		background.color = Color.white;
		if (item != null)
		{
			StartCoroutine(ShowTooltip());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		background.color = notHighlightColor;
		playerUI.HideTooltip();
		StopAllCoroutines();
	}

	IEnumerator ShowTooltip()
	{
		yield return new WaitForSeconds(playerUI.timeUntillTooltipActivation);
		playerUI.ShowTooltip(transform.position, item);
	}
}