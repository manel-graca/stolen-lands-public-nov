using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using StolenLands.Cinematics;
using StolenLands.Player;
using UnityEditor;
using UnityEngine;

public class LuaRegisterManager : MonoBehaviour
{
    void OnEnable()
    {
        Lua.RegisterFunction("CallSwitchVelkjir", this, SymbolExtensions.GetMethodInfo(() => CallSwitchVelkjir((double)0,(double)0)));
        Lua.RegisterFunction("GiveExperience", this, SymbolExtensions.GetMethodInfo(() => GiveExperience((double)0)));
        Lua.RegisterFunction("GiveItem", this, SymbolExtensions.GetMethodInfo(() => GiveItem(string.Empty,(double)0)));
        Lua.RegisterFunction("RemoveItem", this, SymbolExtensions.GetMethodInfo(() => RemoveItem(string.Empty,(double)0)));
        Lua.RegisterFunction("HasItem", this, SymbolExtensions.GetMethodInfo(() => HasItem(string.Empty)));
        Lua.RegisterFunction("GiveCurrency", this, SymbolExtensions.GetMethodInfo(() => GiveCurrency(string.Empty,(double)0)));
        Lua.RegisterFunction("Cinematic", this, SymbolExtensions.GetMethodInfo(() => Cinematic()));
        Lua.RegisterFunction("OpenShop", this, SymbolExtensions.GetMethodInfo(()=> OpenShop()));
    }

    public void CallSwitchVelkjir(double toDestroy, double toActivate)
    {
        FindObjectOfType<PlayerInteract>().SwitchVelkjirs((int)toDestroy,(int)toActivate);
    }
    public bool HasItem(string requiredItemName)
    {
        var inv = FindObjectOfType<Inventory>();
        for (int i = 0; i < inv.items.Count; i++)
        {
            if (inv.items[i].itemName == requiredItemName)
            {
                return true;
            }
        }
        return false;
    }
    public void GiveItem(string itemName, double amount)
    {
        Item item = (Item)Resources.Load(itemName);
        
        if (item.itemType == ItemType.Gear)
        {
            var gear = (Gear)item;
            if (gear != null)
            {
                gear.hasBeenEquipped = true;
            }
        }
        FindObjectOfType<Inventory>().Add(item, (int)amount);
    }
    public void RemoveItem(string itemName, double amount)
    {
        var inv = FindObjectOfType<Inventory>();
        Item itemToRemove = null;
        for (int i = 0; i < inv.items.Count; i++)
        {
            if (inv.items[i].itemName == itemName)
            {
                itemToRemove = inv.items[i];
            }
            else
            {
                itemToRemove = null;
            }
        }
        if (itemToRemove != null)
        {
            inv.Remove(itemToRemove, (int)amount);
        }
        else
        {
            Debug.Log("You dont have that item! " + itemName);
        }
    }

    public void GiveCurrency(string currencyName, double amount)
    {
        FindObjectOfType<Inventory>().AddToCurrency(currencyName, (int) amount);
    }
    public void GiveExperience(double amount)
    {
        FindObjectOfType<PlayerResources>().AddToExperience((float)amount);
    }
    public void Cinematic()
    {
        FindObjectOfType<Cinematics>().PlayCinematic();
    }
    public void OpenShop()
    {
        PlayerUI.instance.OpenCloseMasterShop(true);
    }
    
}
