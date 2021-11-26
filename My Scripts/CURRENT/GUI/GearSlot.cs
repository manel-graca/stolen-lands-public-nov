using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using StolenLands.Player;

public class GearSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon;
    public Gear gear;

	public GearPlace gearPlace;

	float timeBetweenEquipGear = 0.75f;

	float timeSinceEquippedGear = Mathf.Infinity;

	bool equipping;

	Inventory inventory;
	CharacterStats playerStats;
	PlayerUI playerUI;
	ItemMoverUI itemMover;
	PlayerCombatController pCombat;
	EquippedGearInventory equippedGearInventory;
	PlayerSoundManager soundM;
	private void Start()
	{
		inventory = Inventory.instance;
		playerStats = CharacterStats.instance;
		playerUI = PlayerUI.instance;
		itemMover = ItemMoverUI.instance;
		pCombat = PlayerCombatController.instance;
		soundM = PlayerSoundManager.instance;
		equippedGearInventory = EquippedGearInventory.instance;
	}

	void Update()
	{
		if (timeSinceEquippedGear > timeBetweenEquipGear)
		{
			equipping = false;
		}
		timeSinceEquippedGear += Time.deltaTime;

	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if(gear == null)
			{
				if (!equipping)
				{
					EquipGear();
					return;
				}
			}
			else
			{
				if (!equipping)
				{
					if (itemMover.hasItemMoving)
					{
						playerUI.InstantiateWarning("Unequip gear first");
						return;
					}
					UnEquipGear();
					return;
				}
				return;
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Middle)
			return;
		else if (eventData.button == PointerEventData.InputButton.Right)
			return;
	}
	private void EquipGear()
	{
		if (gear != null)
		{
			playerUI.InstantiateWarning("Remove equipped gear first");
			return;
		}
		if(itemMover.hasItemMoving)
		{
			if(itemMover.movingItem.itemType == ItemType.Gear) //  || itemMover.movingItem.itemType == ItemType.Weapon
			{
				Gear incomingGear = (Gear)itemMover.movingItem;
				if(incomingGear.gearPlace == gearPlace)
				{
					equippedGearInventory.Add(incomingGear);
					inventory.Remove(incomingGear,1);
				}
			}
		}
	}
	private void UnEquipGear()
	{
		if (gear != null && !equipping)
		{
			gear.hasBeenEquipped = true;
			Debug.Log(gear.itemName + " " + gear.hasBeenEquipped);
			if (gear.gearType == GearType.Weapon)
			{
				var weap = (Weap)gear;
				if (weap == null) return;
				StartCoroutine(UnequipRoutine());
				timeSinceEquippedGear = 0f;
				itemMover.hasItemMoving = true;
				itemMover.movingItem = gear;
				pCombat.UnEquipWeapon(weap);
				equippedGearInventory.Remove(weap);
				soundM.PlaySwordEquipSound(false);
				return;
			}
			StartCoroutine(UnequipRoutine());
			timeSinceEquippedGear = 0f;
			itemMover.hasItemMoving = true;
			itemMover.movingItem = gear;
			equippedGearInventory.Remove(gear);
			soundM.PlaySimpleSound(soundM.gearUnEquipSound);
			return;
		}
	}

	#region routines
	IEnumerator EquipRoutine()
	{
		equipping = true;
		yield return new WaitForSeconds(timeBetweenEquipGear);
		equipping = false;
	}

	IEnumerator UnequipRoutine()
	{
		equipping = true;
		yield return new WaitForSeconds(timeBetweenEquipGear);
		equipping = false;
	}
	#endregion
	public void ClearSlot()
	{
		playerStats.RemoveFromStat(gear, gear.statValueToAdd);
		gear = null;
		icon.sprite = null;
		icon.color = Color.clear;
	}

	public void ReceiveGear()
	{
		if (itemMover.movingItem == null || itemMover.movingAbility != null) return;
		if(!equipping)
		{
			if (gear != null)
			{
				playerUI.InstantiateWarning("Remove equipped gear first");
				return;
			}
			
			if (gear == null)
			{
				var gearToAdd = (Gear)itemMover.movingItem;
				if (gearToAdd.gearPlace == gearPlace && gearToAdd != null && gearToAdd.gearPlace != GearPlace.Weapon) // NON-weapon gear
				{
					if (gearToAdd.gearPlace == GearPlace.Shield)
					{
						Debug.Log("Equipping shield");
						pCombat.EquipShield();
						StartCoroutine(EquipRoutine());
						timeSinceEquippedGear = 0f;
						gear = gearToAdd;
						icon.color = Color.white;
						icon.sprite = gearToAdd.icon;
						playerStats.AddToStat(gear, gear.statValueToAdd);
						itemMover.RemoveFromMover();
						soundM.PlaySimpleSound(soundM.gearEquipSound);
						return;
					}
					
					StartCoroutine(EquipRoutine());
					timeSinceEquippedGear = 0f;
					gear = gearToAdd;
					icon.color = Color.white;
					icon.sprite = gearToAdd.icon;
					playerStats.AddToStat(gear, gear.statValueToAdd);
					itemMover.RemoveFromMover();
					soundM.PlaySimpleSound(soundM.gearEquipSound);

					return;
				}
				var weaponToAdd = (Weap)itemMover.movingItem; // handles weapon gear
				if (weaponToAdd != null && weaponToAdd.gearPlace == GearPlace.Weapon) // double guard
				{
					pCombat.EquipWeapon(weaponToAdd);
					StartCoroutine(EquipRoutine());
					timeSinceEquippedGear = 0f;
					gear = weaponToAdd;
					icon.color = Color.white;
					icon.sprite = gearToAdd.icon;
					playerStats.AddToStat(gear, gear.statValueToAdd);
					itemMover.RemoveFromMover();
					soundM.PlaySwordEquipSound(true);
					return;
				}
				
			}
		}
	} 

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(gear != null)
		{
			playerUI.ShowTooltip(transform.position, gear);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		playerUI.HideTooltip();
	}
}
