using StolenLands.Enemy;
using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootWindow : MonoBehaviour
{
	public static LootWindow instance;

	public List<LootButton> lootButtons = new List<LootButton>();
	public List<Item> itemsToLoot = new List<Item>();

	GameObject[] enemies;
	GameObject[] boxes;

	PlayerCombatController pCombat;
	private void OnEnable()
	{
		int num = FindObjectsOfType<LootWindow>().Length;
		if (num > 1) Destroy(gameObject);
		instance = this;

		boxes = GameObject.FindGameObjectsWithTag("LootBox");
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		pCombat = FindObjectOfType<PlayerCombatController>();

		if (boxes.Length >= 1)
		{
			foreach (var box in boxes)
			{
				if (box != null)
				{
					if (boxes.Length > 1 && box.GetComponent<LootBox>().alreadyClickedOn)
					{
						var list = box.GetComponent<LootBox>().enemyDropping.GetComponent<LootDropSystem>().itemsToDrop;
						var targetDropSys = box.GetComponent<LootBox>().enemyDropping.GetComponent<LootDropSystem>();

						foreach (var item in lootButtons)
						{
							if (item == null) lootButtons.Remove(item);
						}

						GenerateLoot(list);
						list.Clear();
						return;
					}

					if (box.activeSelf && box.GetComponent<LootBox>().alreadyClickedOn)
					{
						var targetDropSys = box.GetComponent<LootBox>().enemyDropping.GetComponent<LootDropSystem>();
						GenerateLoot(targetDropSys.itemsToDrop);
						targetDropSys.ClearItemList();
					}
				}
			}
		}
	}

	void Update()
	{
		for(var i = lootButtons.Count - 1; i > -1; i--)
		{
			if (lootButtons[i].item == null)
			{
				lootButtons.RemoveAt(i);
			}
		}
		if (lootButtons.Count == 0)
		{
			Destroy(gameObject);
		}
		if (lootButtons.Count == 1)
		{
			if (lootButtons[0].item == null)
			{
				lootButtons.Remove(lootButtons[0]);
				foreach (var item in boxes)
				{
					if(item.GetComponent<LootBox>().alreadyClickedOn)
					{
						Destroy(item.gameObject);
					}
				}
			}
		} 
	}

	void GenerateLoot(List<Item> items)
	{
		for (int i = 0; i <= lootButtons.Count; i++)
		{
			if(lootButtons[i] != null && i <= items.Count)
			{
				lootButtons[i].icon.sprite = items[i].icon;
				lootButtons[i].titleText.text = items[i].itemName;
				lootButtons[i].item = items[i];
				if (i == items.Count - 1)
				{
					items.Clear();
					return;
				}
			}
		}
	}

	public void RemoveButtonFromWindow(LootButton button, Item item)
	{
		lootButtons.Remove(button);
		itemsToLoot.Remove(item);
		button.gameObject.SetActive(false);
		
		if (lootButtons.Count == 0 || lootButtons[0] == null)
		{
			foreach (var enemy in enemies)
			{
				EnemyController a = enemy.GetComponent<EnemyController>();
				if(a.lootBox != null)
				{
					if (a.lootBox.activeSelf && a.lootBox.GetComponent<LootBox>().alreadyClickedOn)
					{
						a.lootBox.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void CloseWindow()
	{
		
		Destroy(gameObject);
	}
}
