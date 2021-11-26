using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagSlot : MonoBehaviour, IPointerClickHandler
{
    private ItemMoverUI itemMover;
    [SerializeField] private GameObject invSlot;
    [SerializeField] private Transform parent;
    [SerializeField] private Image icon;
    [SerializeField] private Bag bag;
    [SerializeField] private List<InventorySlot> tempSlotAdded = new List<InventorySlot>();
    private Inventory inventory;
    private PlayerSoundManager soundM;
    private void Start()
    {
        itemMover = ItemMoverUI.instance;
        inventory = Inventory.instance;
        soundM = PlayerSoundManager.instance;
        icon.color = Color.clear;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (bag != null)
            {
                UnEquipBag();
                return;
            }
            EquipBag();
            return;
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
            return;
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (bag == null) return;
            UnEquipBag();
            return;
        }
    }
    private void UnEquipBag()
    {
        if (itemMover.movingItem == null)
        {
            if (bag != null)
            {
                foreach (var slot in tempSlotAdded) // guard
                {
                    if (slot.item != null)
                    {
                        PlayerUI.instance.InstantiateWarning("The bag you are trying to remove still has items!");
                        return;
                    }
                } // GUARD
                
                var gear = (Gear) bag;
                
                bag.hasBeenEquipped = true;
                itemMover.AssignItemToMover(bag, 1);
                inventory.space -= bag.spaceToAdd;
                icon.sprite = null;
                icon.color = Color.clear;
                for (int i = 0; i < tempSlotAdded.Count; i++)
                {
                    Destroy(tempSlotAdded[i].gameObject);
                }
                tempSlotAdded.Clear();
                soundM.PlaySimpleSound(soundM.unEquipBagSound);
                EquippedGearInventory.instance.RemoveBag(bag);
                bag = null;
                return;
            }
            else return;
        }
    }
    private void EquipBag()
    {
        if (itemMover.movingItem != null)
        {
            if (itemMover.movingItem.itemType != ItemType.Gear) return;
            
            var itemCheck = (Gear)itemMover.movingItem;
            if (itemCheck.gearType != GearType.Bag) return;
            
            if (bag == null)
            {
                var newBag = (Bag) itemMover.movingItem;
                var item = (Item) itemMover.movingItem;
                
                for (int i = 0; i < newBag.spaceToAdd; i++)
                {
                    GameObject inst_slot = Instantiate(invSlot) as GameObject;
                    InventorySlot newSlot = inst_slot.GetComponent<InventorySlot>();
                    inventory.invSlots.Add(newSlot); 
                    tempSlotAdded.Add(newSlot);
                    inst_slot.transform.SetParent(parent, false);
                    inst_slot.transform.SetAsLastSibling();
                    inst_slot.name = "Inv_Slot_instance";
                }
                inventory.Remove(item,1);
                EquippedGearInventory.instance.AddBag(newBag);
                bag = newBag;
                inventory.space += bag.spaceToAdd;
                icon.sprite = bag.icon;
                icon.color = Color.white;
                itemMover.RemoveFromMover();
                soundM.PlaySimpleSound(soundM.equipBagSound);
                return;
            }
            else return;
        }
    }
}
