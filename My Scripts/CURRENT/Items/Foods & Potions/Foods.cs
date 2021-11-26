using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Food", menuName ="Items/Foods/New Food")]
public class Foods : Item
{
    [Title("Healing Properties")]
    
    [Tooltip("Heal amount per second")] 
	[SerializeField] float healAmount;
    
    [Tooltip("Time in seconds")] 
    [SerializeField] float maxTimeHealing;

	public override void Use()
	{
		var soundM = PlayerSoundManager.instance;
		
		if (PlayerHealthController.instance.health < PlayerHealthController.instance.maxHealth)
		{
			PlayerUI.instance.HideTooltip();
			if(PlayerHealthController.instance.HealFood(healAmount, maxTimeHealing))
			{
				Inventory.instance.Remove(this, 1);
				soundM.PlayFoodEatSound();
			}
		}
	}

	public override string GetDescription()
	{
		return base.GetDescription() + string.Format("\nUse: Restores <color=#7FFF00>{0} HP</color> each second during {1} seconds" + "\n{2}", 
			healAmount, maxTimeHealing, itemDescription);
	}
}
