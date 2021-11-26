using StolenLands.Abilities;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Ability ability;
	public Image icon;
	public Text cooldownText;


	Sprite initialIcon;
	
	PlayerUI playerUI;
	PlayerSpellSystem spellSys;
	ItemMoverUI itemMover;
	AbilitySlot[] slots;
	PlayerSoundManager soundM;

	
	private void Start()
	{
		initialIcon = icon.sprite;
		soundM = PlayerSoundManager.instance;
		itemMover = ItemMoverUI.instance;
		playerUI = PlayerUI.instance;
		spellSys = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpellSystem>();
		cooldownText.text = null;
		slots = FindObjectsOfType<AbilitySlot>();
		if (ability != null) icon.sprite = ability.abilityImage;
	}

	private void Update()
	{
		UpdateCooldown();
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (ability != null)
		{
			playerUI.ShowTooltip(transform.position, ability);
		}
		else
		{
			playerUI.ShowTooltipNoInformation(transform.position, "Ability Slot");
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		PlayerUI.instance.HideTooltip();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (itemMover.hasAbilityMoving && itemMover.movingAbility != null)
		{
			if (ability == null)
			{
				ability = itemMover.movingAbility;
				icon.sprite = ability.abilityImage;
				itemMover.RemoveFromMover();
				soundM.PlayInterfaceSound(soundM.mousePlaceAbilityTree);
				return;
			}
		}
		if (!itemMover.hasAbilityMoving && ability != null)
		{
			itemMover.hasAbilityMoving = true;
			itemMover.movingAbility = ability;
			itemMover.GetComponent<Image>().sprite = ability.abilityImage;
			itemMover.GetComponent<Image>().color = Color.white;
			icon.sprite = initialIcon;
			ability = null;
			soundM.PlayInterfaceSound(soundM.mousePickupAbilityTree);
		}
	}

	void UpdateCooldown()
	{
		if(ability != null)
		{
			if(ability.currentCooldown > 0)
			{
				cooldownText.text = ability.currentCooldown.ToString("#.##");
				icon.color = Color.grey;
			}
			else
			{
				icon.color = Color.white;
				cooldownText.text = null;
			}
		}
	}
}
