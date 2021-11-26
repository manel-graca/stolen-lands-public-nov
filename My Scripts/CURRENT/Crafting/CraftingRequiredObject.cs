using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingRequiredObject : MonoBehaviour
{
    public string displayName;

    private void OnMouseEnter()
    {
        GetComponent<HighlightEffect>().highlighted = true;
    }

    private void OnMouseExit()
    {
        GetComponent<HighlightEffect>().highlighted = false;
    }
}