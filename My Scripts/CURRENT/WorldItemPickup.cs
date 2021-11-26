using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    [SerializeField] private Item itemToGive;
    [SerializeField] private int amountToGive;
    private bool playerNear = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerNear)
            {
                var sound = PlayerSoundManager.instance.lootGearSound;
                if (itemToGive.itemType == ItemType.Gear)
                {
                    itemToGive.hasBeenEquipped = true;
                }
                PlayerUI.instance.DeactivateHintText();

                Inventory.instance.Add(itemToGive, amountToGive);
                PlayerSoundManager.instance.PlayInterfaceSound(sound); 
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerUI.instance.ActivateHintText("Press E to pickup " + itemToGive.itemName);
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerUI.instance.DeactivateHintText();
            playerNear = false;
        }
    }
}