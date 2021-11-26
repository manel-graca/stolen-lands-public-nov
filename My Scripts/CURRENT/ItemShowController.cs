using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemShowController : MonoBehaviour
{
    [SerializeField] private Item[] allItems;
    [SerializeField] private ShowItemIcon[] itemsSquares;

    private void Start()
    {
        itemsSquares = FindObjectsOfType<ShowItemIcon>();
        for (int i = 0; i < allItems.Length; i++)
        {
            itemsSquares[i].item = allItems[i];
            var icon = allItems[i].icon;
            var squareIcon =  itemsSquares[i].GetComponent<Image>();
            squareIcon.sprite = icon;
            squareIcon.color = Color.white;
        }
        
        for (int i = 0; i < itemsSquares.Length; i++)
        {
            itemsSquares[i].GetComponent<Image>().sprite = allItems[i].icon;
        }
    }


    
}
