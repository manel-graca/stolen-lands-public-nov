using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Food", menuName = "Items/NonUsable/New Item")]
public class NonUsableItem : Item
{
	public override string GetDescription()
	{
		return base.GetDescription() + string.Format("\n{0}", itemDescription);
	}
}
