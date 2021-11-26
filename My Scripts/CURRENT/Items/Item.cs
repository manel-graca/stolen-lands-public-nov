using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Quality { Common, Uncommon, Rare, Epic, Legendary, Divine }
public enum ItemType { Tool, Resource, Food, Potion, Gear, Quest, NonUsable}

public class Item : ScriptableObject, IDescriber
{
	[PreviewField(90f, ObjectFieldAlignment.Left), HideLabel]
	[HorizontalGroup("Split", 90f)]
	public Sprite icon;
	[Space]
	[VerticalGroup("Split/Right"), LabelWidth(120f)]
	public string itemName;
	//[VerticalGroup("Split/Right"), LabelWidth(120f)]
	[HorizontalGroup("Split/Right/Center"), LabelWidth(120f)]
	public Quality itemQuality;
	[HorizontalGroup("Split/Right/Center"), LabelWidth(120f)]
	public ItemType itemType;
	[VerticalGroup("Split/Right"), LabelWidth(120f)]
	[HorizontalGroup("Split/Right/StackAmount")]
	public int maxStack = 2;

	[HorizontalGroup("Split/Right/StackAmount")] 
	[Button("1")]void MaxStack1(){maxStack = 1;}
	
	[HorizontalGroup("Split/Right/StackAmount")] 
	[Button("20")]void MaxStack20(){maxStack = 20;}
	
	[HorizontalGroup("Split/Right/StackAmount")] 
	[Button("80")]void MaxStack80(){maxStack = 80;}
	[VerticalGroup("Split/Right"), LabelWidth(120f)]
	public bool isCurrency = false;
	
	[Multiline(4)] [HideLabel]
	public string itemDescription;
	
	[Space]
	
	[BoxGroup("Trading")]
	public string currencyToGive = "Silver";
	[BoxGroup("Trading")]
	public int buyPrice = 20;
	[BoxGroup("Trading")]
	public int sellPrice = 10;

	[HideInInspector] public int amount = 0;
	[HideInInspector] public bool hasBeenEquipped;
	public const int defaultAmount = 1;
	public virtual void EquipWeapon(Transform rHand, Transform lHand, Animator animator)
	{
		Debug.Log("Equipping WEAPON!");
	}

	public virtual void EquipGear()
	{
		Debug.Log("Equipping" + itemName);
	}
	public virtual void Use()
	{
		Debug.Log("Using " + itemName);
	}
	public virtual string GetPlaceText()
	{
		return string.Empty;
	}

	public virtual string GetDescription()
	{
		string color = string.Empty;

		switch (itemQuality)
		{
			case Quality.Common: 
				color = "#A5A5A5"; // CHANGE TO GRAY / WHITE
				break;
			case Quality.Uncommon:
				color = "#FFF2CC"; // RE CHECK COLOR
				break;
			case Quality.Rare:
				color = "#5B9BD5";
				break;
			case Quality.Epic:
				color = "#66FF66";
				break;
			case Quality.Legendary:
				color = "#C06E00";
				break;
			case Quality.Divine:
				color = "#A32C9E"; // CHANGE COLOR
				break;
		}
		return string.Format("<color=#FFFCE4>{1}</color>" + "\n<color={0}>{2}</color>", color, itemName, itemQuality);
	}
}