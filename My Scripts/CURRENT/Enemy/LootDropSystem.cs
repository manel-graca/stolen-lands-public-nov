using System.Collections.Generic;
using UnityEngine;

public class LootDropSystem : MonoBehaviour
{
	public List<Item> itemsPool = new List<Item>();
	public int[] table = { 60, 30, 10 };
	int total;
	int randomNumber;
	int randomAmountNumber;
	bool alreadyLooted = false;
	[Space]
	public List<Item> itemsToDrop = new List<Item>();

	private void Start()
	{
		if (!alreadyLooted)
		{
			if (itemsPool != null && itemsPool.Count > 0)
			{
				foreach (var item in table)
				{
					total += item;
				}

				randomAmountNumber = Random.Range(1, 4);
				randomNumber = Random.Range(0, total);

				for (int i = 0; i < table.Length; i++)
				{
					if (randomNumber <= table[i])
					{
						itemsToDrop.Add(itemsPool[i]);
					}
					else
					{
						randomNumber -= table[i];
					}
				}
			}
		}
	}

	public void ClearItemList()
	{
		alreadyLooted = true;
		itemsToDrop.Clear();
	}

}
