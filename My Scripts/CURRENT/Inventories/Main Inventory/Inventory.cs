using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Guirao.UltimateTextDamage;
using StolenLands.Player;
using UnityEngine;
using Sirenix.OdinInspector;
public class Inventory : MonoBehaviour
{
	#region Singleton
	public static Inventory instance;
	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("!!!!!!more than 1 instance of inventory found!!!!!!!"); 
			return;
		}

		instance = this;
	}
	#endregion
	
	[BoxGroup("Currency Manager")]
	public int goldOwned;
	[BoxGroup("Currency Manager")]
	public int silverOwned;
	[BoxGroup("Currency Manager")]
	public int copperOwned;
	
	[TitleGroup("Inventory Manager - Core")]
	public int defaultSlotsCount = 10;
	public int space;
	[TitleGroup("Inventory Manager - Lists")]
	
	public List<Item> items = new List<Item>();
	public List<InventorySlot> invSlots = new List<InventorySlot>();
	
	[HideInInspector] public Item copyItem;
	[HideInInspector] public Item copyMovingItem;
	
	[Space]
	
	[TitleGroup("Inventory Manager - UI")]
	[SerializeField] Transform slotsHolder;
	
	[Space]
	
	[TitleGroup("Inventory Manager - Cheats")]
	[SerializeField] bool cheatDrop = false;
	[SerializeField] List<Item> cheatItemList = new List<Item>();

	private float timeSinceCheckCleanup = 0;
	private float timeBetweenCleanups = 0.75f;
	
	PlayerUI ui;
	void Start()
	{
		ui = FindObjectOfType<PlayerUI>();
		space = defaultSlotsCount;
		PopulateDefaultInvSlotsList();
		
		if (cheatDrop) // !!!!!!!!!!!!!!!!!!!!!!!!!!!     REMOVE OR CHANGE TO DEFAULT ITEM STARTING LIST     !!!!!!!!!!!!!!!!!!!!!!!!!!!
		{
			foreach (var item in cheatItemList)
			{
				if (item.itemType == ItemType.Gear)
				{
					var a = (Gear) item;
					a.hasBeenEquipped = true;
				}
				Add(item, 3);
			}
		}
	}
	private void Update()
	{
		for (int i = items.Count - 1; i >= 0; i--)
		{
			if (items[i].amount <= 0)
			{
				Remove(items[i], 1);
			}
		}
		
		if (timeSinceCheckCleanup > timeBetweenCleanups && invSlots.Count > 10)
		{
			CleanListInvSlots();
		}
		timeSinceCheckCleanup += Time.deltaTime;
	}
	public void CleanListInvSlots()
	{
		timeSinceCheckCleanup = 0f;
		for(var i = invSlots.Count - 1; i > -1; i--)
		{
			if (invSlots[i] == null)
			{
				invSlots.RemoveAt(i);
			}
		}
	}
	
	public bool Add(Item item, int amount)
	{
		if (item.isCurrency)
		{
			AddToCurrency(item.itemName, amount);
			return true;
		}
		
		bool stacking = false;
		for (int i = 0; i < items.Count; i++)
		{
			if (item.itemName == items[i].itemName && item.maxStack == 1)
			{
				PlayerUI.instance.InstantiateWarning("You already own this item");
				return false;
			}
			
			if (items[i].itemName == item.itemName && items[i].itemType != ItemType.Gear)
			{
				stacking = true;
				break;
			}
			else stacking = false;
		}
		if (items.Count >= space && !stacking) // guard for inv full
		{
			string color = "#FF4112";                                                            
			string invFull = String.Format("<color={0}>Inventory is full</color>", color);
			PlayerUI.instance.InstantiateWarning(invFull);
			return false;
		}
		
		// if item is any piece of gear
		if (item.itemType == ItemType.Gear)
		{
			var a = (Gear) item;

			if (EquippedGearInventory.instance.gearEquipped.Contains(a))
			{
				return false;
			}
			
			string info = amount.ToString() + "x " + item.itemName + " added";
			if (a.hasBeenEquipped)
			{
				items.Add(item);
				item.amount = 1;
				UpdateUI();
				a.hasBeenEquipped = false;
				ui.InstantiateInformationMessage(info);
				return true;
			}
			return false;
		}
		
		// when item is NOT gear and will be able to stack 
		copyItem = Instantiate(item);
		item = copyItem;
		
		// in case we already have that item and we can stack it
		for (int i = 0; i < items.Count; i++) 
		{
			if (item.itemName == items[i].itemName && (items[i].amount + amount) <= items[i].maxStack)
			{
				string info = amount.ToString() + "x " + item.itemName + " stacked";
				items[i].amount += amount;
				UpdateUI();
				ui.InstantiateInformationMessage(info);
				return true;
			}
		}
		// in case we DONT have the item, and we are adding item with amount greater than 1
		if (amount > 1)
		{
			items.Add(item);
			item.amount = amount;
			UpdateUI();
			ui.InstantiateInformationMessage(amount.ToString() + "x " + item.itemName + " added");
			return true;
		}
		// in case we DONT have the item, and we are adding item with 1 amount
		items.Add(item);
		item.amount = 1;
		UpdateUI();
		ui.InstantiateInformationMessage("1x " + item.itemName + " added");
		return true;
	}
	
	public void Remove(Item item, int amount)
	{
		for (int i = items.Count - 1; i >= 0; i--)
		{
			if(item.itemName == items[i].itemName)
			{
				amount = Mathf.Clamp(amount, -1, 999);
				items[i].amount -= amount;
				items[i].amount = Mathf.Clamp(items[i].amount, -1, 999);

				if (items[i].amount >= 1)
				{
					UpdateUI();
					ui.InstantiateInformationMessage(amount.ToString() + " " + item.itemName + " removed");
				}
				else
				{
					items.Remove(item);
					invSlots[i].ClearSlot();
					return;
				}
			}
		}
	}

	public void GoldToSilver()
	{
		if (goldOwned >= 1)
		{
			goldOwned -= 1;
			silverOwned += 100;
			// play sound
			ui.InstantiateInformationMessage("Converted 1 gold to 100 silver");
			return;
		}
		ui.InstantiateWarning("Not enough gold");
	}

	public void SilverToGold()
	{
		if (silverOwned >= 100)
		{
			silverOwned -= 100;
			goldOwned += 1;
			// play sound
			ui.InstantiateInformationMessage("Converted 100 silver to 1 gold");
			return;
		}
		ui.InstantiateWarning("Not enough silver");
	}

	public void SilverToCopper()
	{
		if (silverOwned >= 1)
		{
			silverOwned -= 1;
			copperOwned += 100;
			// play sound
			ui.InstantiateInformationMessage("Converted 1 silver to 100 copper");
			return;
		}
		ui.InstantiateWarning("Not enough silver");
	}

	public void CopperToSilver()
	{
		if (copperOwned >= 100)
		{
			copperOwned -= 100;
			silverOwned += 1;
			// play sound
			ui.InstantiateInformationMessage("Converted 100 copper to 1 silver");
			return;
		}
		ui.InstantiateWarning("Not enough copper");
	}


	public void RemoveFromCurrency(string currencyName, int amount)
	{
		if (currencyName == "Gold")
		{
			goldOwned -= amount;
		}
		if (currencyName == "Silver")
		{
			silverOwned -= amount;
		}
		if (currencyName == "Copper")
		{
			copperOwned -= amount;
		}
	}

	public void AddToCurrency(string currencyName, int amount)
	{
		if (currencyName == "Gold")
		{
			goldOwned += amount;
		}
		if (currencyName == "Silver")
		{
			silverOwned += amount;
		}
		if (currencyName == "Copper")
		{
			copperOwned += amount;
		}
		ui.CurrencyOwnedUpdate();
	}

	public int GetCurrencyOwned(string currencyName)
	{
		if (currencyName == "Gold")
		{
			return goldOwned;
		}
		if (currencyName == "Silver")
		{
			return silverOwned;
		}
		if (currencyName == "Copper")
		{
			return copperOwned;
		}

		return 0;
	}

	public bool GetIfCanAddItem()
	{
		int a = space - items.Count;
		if (a >= 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool HasItem(Item item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].itemName == item.itemName)
			{
				return true;
			}
		}
		return false;
	}
	public int GetItemAmount(Item item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].itemName == item.itemName)
			{
				return items[i].amount;
			}
		}
		return 0;
	}
	
	private void PopulateDefaultInvSlotsList()
	{
		var a = slotsHolder.GetComponentsInChildren<InventorySlot>();
		for (int i = 0; i < a.Length; i++)
		{
			if (i <= defaultSlotsCount - 1)
			{
				if (a[i].isDefault)
				{
					invSlots.Add(a[i]);
				}
			}
			else return;
		}
	}
	void UpdateUI()
	{
		for (int i = 0; i < invSlots.Count; i++)
		{
			if (i < items.Count)
			{
				invSlots[i].AddItemToSlot(items[i]);
			}
			else
			{
				invSlots[i].ClearSlot();
			}
		}
	}
}
