using StolenLands.Abilities;
using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField] Text abilityName;
	[SerializeField] Image lockImage;
	[SerializeField] bool isLocked = true;
	public Ability ability;

	Image icon;
	PlayerUI playerUI;
	PlayerResources playerR;
	ItemMoverUI itemMover;
	PlayerSoundManager soundM;
	private void Start()
	{
		icon = GetComponent<Image>();
		playerUI = PlayerUI.instance;
		playerR = PlayerResources.instance;
		itemMover = ItemMoverUI.instance;
		soundM = PlayerSoundManager.instance;
		
		if (ability != null)
		{
			icon.sprite = ability.abilityImage;
			
			if(ability.GetRequiredLevel() <= playerR.GetCurrentLevel() && playerR.currentPoints >= ability.requiredPointsToUnlock)
			{
				UnlockAbilityUI();
			}
			if(ability.GetRequiredLevel() > playerR.GetCurrentLevel() || playerR.currentPoints < ability.requiredPointsToUnlock)
			{
				LockAbilityUI();
			}
		}
		else
		{
			GetComponentInChildren<Text>().text = null;
		}
	}

	public void ClickToUnlockAbility() // UI BUTTON
	{
		
		if (playerR.currentPoints >= ability.requiredPointsToUnlock)
		{
			playerR.currentPoints -= ability.requiredPointsToUnlock;
			soundM.PlayInterfaceSound(soundM.abilityUnlockSound);
			itemMover.hasAbilityMoving = true;
			itemMover.movingAbility = ability;
			itemMover.GetComponent<Image>().sprite = ability.abilityImage;
			itemMover.GetComponent<Image>().color = Color.white;
		}
		else
		{
			if (playerR.currentPoints < ability.requiredPointsToUnlock)
			{
				playerUI.InstantiateWarning("Not enough spell points");
				return;
			}
			if (playerR.currentPoints < ability.requiredPointsToUnlock)
			{
				playerUI.InstantiateWarning("Level too low");
				return;
			}
		}
	}

	public void UnlockAbilityUI()
	{
		isLocked = false;
		lockImage.enabled = false;
		abilityName.text = ability.abilityName;
		icon.color = Color.white;
	}

	public void LockAbilityUI()
	{
		isLocked = true;
		lockImage.enabled = true;
		abilityName.text = ability.abilityName;
		icon.color = Color.gray;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if(ability == null) return;
		AudioClip clip = soundM.mousePickupAbilityTree;
		
		playerUI.lastSpell = ability;
		playerUI.lastSpellSlot = this;
		if (isLocked)
		{
			playerUI.OpenSpendSpellPointsPopup(ability);
		}
		else
		{
			soundM.PlayInterfaceSound(clip);
			itemMover.hasAbilityMoving = true;
			itemMover.movingAbility = ability;
			itemMover.GetComponent<Image>().sprite = ability.abilityImage;
			itemMover.GetComponent<Image>().color = Color.white;
		}
		
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (ability != null)
		{
			StartCoroutine(ShowTooltip());
			soundM.PlayInterfaceSound(soundM.mouseOverAbilityTree);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StopAllCoroutines();
		playerUI.HideTooltip();
	}

	IEnumerator ShowTooltip()
	{
		yield return new WaitForSeconds(playerUI.timeUntillTooltipActivation);
		playerUI.ShowTooltip(transform.position, ability);
	}

	
}
