using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowItemIcon : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (item.itemType == ItemType.Gear)
        {
            item.hasBeenEquipped = true;
        }
        FindObjectOfType<Inventory>().Add(item,1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<PlayerUI>().ShowTooltip(transform.position, item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<PlayerUI>().HideTooltip();
    }
}