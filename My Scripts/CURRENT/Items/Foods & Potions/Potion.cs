using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum PotionType { Mana, Health, MoveSpeed, AttackDamage, MagicDamage, Resist }
[CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potions/New Potion")]
public class Potion : Item
{
	[Title("Potion Type")][HideLabel]
	[EnumToggleButtons]
	public PotionType potionType;
	[Space]
	
	[Title("Potion Properties")]
	public float amountToGive;
	public float duration;
	

	bool nowUsing;
	string sentenceStart;
	string valueAddedType;
	string color;

	public override void Use()
	{
		switch (potionType)
		{
			case PotionType.Mana:
				if (PlayerResources.instance.GetCurrentMana() < PlayerResources.instance.GetMaxMana())
				{
					if(PlayerResources.instance.AddManaFromPotion(amountToGive))
					{
						PlayerUI.instance.HideTooltip();
						Inventory.instance.Remove(this, 1);
						PlayerSoundManager.instance.PlayPotionUseSound();
					}
				}
				break;
			case PotionType.Health:
				if (PlayerHealthController.instance.health < PlayerHealthController.instance.maxHealth)
				{
					if(PlayerHealthController.instance.HealPotion(amountToGive))
					{
						PlayerUI.instance.HideTooltip();
						Inventory.instance.Remove(this, 1);
						PlayerSoundManager.instance.PlayPotionUseSound();
					}
				}
				break;
			case PotionType.MoveSpeed:
				if(!PlayerMover.instance.potionActive)
				{
					if(PlayerMover.instance.AddSpeedPotion(amountToGive, duration))
					{
						PlayerUI.instance.HideTooltip();
						Inventory.instance.Remove(this, 1);
						PlayerSoundManager.instance.PlayPotionUseSound();
					}
				}
				break;
			case PotionType.AttackDamage:
				break;
			case PotionType.MagicDamage:
				break;
			case PotionType.Resist:
				break;
		}
	}
	public override string GetDescription()
	{
		switch (potionType)
		{
			case PotionType.Mana:
				sentenceStart = "Restores ";
				valueAddedType = " mana";
				color = "#4275f5";
				break;
			case PotionType.Health:
				sentenceStart = "Restores ";
				valueAddedType = " HP";
				color = "#7FFF00";
				break;
			case PotionType.MoveSpeed:
				sentenceStart = "Increases ";
				valueAddedType = " movement speed";
				color = "#D4E349";
				break;
			case PotionType.AttackDamage:
				break;
			case PotionType.MagicDamage:
				break;
			case PotionType.Resist:
				break;
		}
		return base.GetDescription() + string.Format("\nUse:{0} <color={3}>{1} {2}</color>", sentenceStart, amountToGive, valueAddedType, color);
	}
}
