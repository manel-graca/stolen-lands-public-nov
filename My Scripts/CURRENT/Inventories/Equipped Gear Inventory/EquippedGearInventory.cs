using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedGearInventory : MonoBehaviour
{
	public static EquippedGearInventory instance;
	private void Awake()
	{
		instance = this;
	}
	public List<Gear> gearEquipped = new List<Gear>();
	public List<Bag> bagsEquipped = new List<Bag>();
    public int space;
    [SerializeField] Transform gearSlotsHolder;
    public GearSlot[] gearSlots;
    public BagSlot[] bagSlots;

	private void Start()
	{
		gearSlots = gearSlotsHolder.GetComponentsInChildren<GearSlot>();
	}

	public void AddBag(Bag bag)
	{
		if (bag == null) return;
		bagsEquipped.Add(bag);
	}
	public bool Add(Gear gear)
	{
        if (gear == null) return false;
        
		gearEquipped.Add(gear);
		for (int i = 0; i < gearSlots.Length; i++)
		{
			if (gearSlots[i].gearPlace == gear.gearPlace)
			{
				gearSlots[i].ReceiveGear();
				return true;
			}
		}
		return false;
	}

	public void RemoveBag(Bag bag)
	{
		if (bag == null) return;
		bagsEquipped.Remove(bag);
	}
	
	public bool Remove(Gear gear)
	{
		if (gear == null) return false;
		gearEquipped.Remove(gear);

		for (int i = 0; i < gearSlots.Length; i++)
		{
			if (gearSlots[i].gearPlace == gear.gearPlace)
			{
				gearSlots[i].ClearSlot();
				return true;
			}
		}
		return false;
	}
}
