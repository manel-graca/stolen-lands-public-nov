using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
public enum GearPlace { Head, Neck, Back, Chest, Bracers, Hands, Ring, Legs, Feet, Rune, Trinket, Weapon, Shield, NoPlace }
public enum GearType { Weapon, Shield, Gear, Bag, Cosmetic }
// head neck back chest legs bracers foot
// melee shield bow
// hands ring trinket trinket rune rune

[CreateAssetMenu(fileName = "New Gear Piece", menuName = "Items/Gear/New Gear Piece")]
public class Gear : Item
{
	[Space]
	
	[InfoBox("The name that's displayed in tooltip top-right place.", InfoMessageType.Info)]
	[Space]
	public string tooltipPlaceText;
	[Space]
	public GearPlace gearPlace;
	
	[Space(25)]
	
	[EnumToggleButtons] public GearType gearType;
	
	[Space(25)]
	[Title("STATS TO ADD ON EQUIP")]
	public StatsType statType;
	[Space(5)]	
	public float statValueToAdd;
	
	
	string statColorHex;
	string statName;
	
	public override void EquipGear()
	{
		base.EquipGear();
		CharacterStats.instance.AddToStat(this, statValueToAdd);
	}

	public override string GetPlaceText()
	{
		return base.GetPlaceText() + gearPlace.ToString();
	}

	public override string GetDescription()
	{
		string baseStringColor = "#F5F3DF";
		switch (gearType)
		{
			case GearType.Bag:
				statColorHex = "#FFFFFF";
				statName = "Bag";
				break;
			case GearType.Cosmetic:
				statColorHex = "#9700F5";
				statName = "Cosmetic";
				break;
		}
		switch (statType)
		{
			case StatsType.Armor:
				statColorHex = "#ffbf00";
				statName = "Armor";
				break;
			case StatsType.Agility:
				statColorHex = "#C5FF61";
				statName = "Agility";
				break;
			case StatsType.Stamina:
				statColorHex = "#FFF861";
				statName = "Stamina";
				break;
			case StatsType.AttackDamage:
				statName = "Attack Damage";
				statColorHex = "#F53C16";
				break;
			case StatsType.MagicDamage:
				statName = "Magic Damage";
				statColorHex = "#483DF5";
				break;
			case StatsType.MagicResist:
				statName = "Magic Resist";
				statColorHex = "#8273F5";
				break;
			case StatsType.Intellect:
				statColorHex = "#149CF8";
				statName = "Intellect";
				break;
			case StatsType.Focus:
				statColorHex = "#F59C3C";
				statName = "Focus";
				break;
			case StatsType.Strength:
				statColorHex = "#FCDF5B";
				statName = "Strength";
				break;
			case StatsType.Dexterity:
				statColorHex = "#3FFCF6";
				statName = "Dexterity";
				break;
			case StatsType.Endurance:
				statColorHex = "#3FFCF6";
				statName = "Endurance";
				break;
			case StatsType.Vitality:
				statColorHex = "#3FFCF6";
				statName = "Vitality";
				break;
		}

		if (gearType == GearType.Bag)
		{
			var bag = (Bag) this;
			return base.GetDescription() + string.Format("\n<color={0}>{1} slots</color>" + "\n<color=4#FFFFFF>{2}</color>",
				statColorHex,  bag.spaceToAdd, itemDescription);
		}

		if(statType == StatsType.NoStat)
		{
			return base.GetDescription() + string.Format("\n<color={0}>{1}</color>" + "\n<color={4}>{3}</color>",
			statColorHex, statName, statValueToAdd, itemDescription, baseStringColor);
		}

		return base.GetDescription() + string.Format("\n<color={0}>+{2} {1}</color>" + "\n<color={4}>{3}</color>",
			statColorHex, statName, statValueToAdd, itemDescription, baseStringColor);
	}

}
