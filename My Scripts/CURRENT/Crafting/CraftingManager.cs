using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingManager : MonoBehaviour
{
    [Tooltip("The order must be the same as in hierarchy!")] 
    [SerializeField] GameObject[] categoryObjects;
    
    public void CategoryClickHandler(int index)
    {
        var cg0 = categoryObjects[0].GetComponent<CanvasGroup>();
        var cg1 = categoryObjects[1].GetComponent<CanvasGroup>();
        var cg2 = categoryObjects[2].GetComponent<CanvasGroup>();
        var cg3 = categoryObjects[3].GetComponent<CanvasGroup>();
        var cg4 = categoryObjects[4].GetComponent<CanvasGroup>();
        var cg5 = categoryObjects[5].GetComponent<CanvasGroup>();
        switch (index)
        {
            // gear
            case 0: 
                cg0.interactable = true; cg0.alpha = 1f; cg0.blocksRaycasts = true;
                
                cg1.interactable = false; cg1.alpha = 0f; cg1.blocksRaycasts = false;
                cg2.interactable = false; cg2.alpha = 0f; cg2.blocksRaycasts = false;
                cg3.interactable = false; cg3.alpha = 0f; cg3.blocksRaycasts = false;
                cg4.interactable = false; cg4.alpha = 0f; cg4.blocksRaycasts = false;
                cg5.interactable = false; cg5.alpha = 0f; cg5.blocksRaycasts = false;
                break;
            // weapons
            case 1: 
                cg1.interactable = true; cg1.alpha = 1f; cg1.blocksRaycasts = true;
                
                cg0.interactable = false; cg0.alpha = 0f; cg0.blocksRaycasts = false;
                cg2.interactable = false; cg2.alpha = 0f; cg2.blocksRaycasts = false;
                cg3.interactable = false; cg3.alpha = 0f; cg3.blocksRaycasts = false;
                cg4.interactable = false; cg4.alpha = 0f; cg4.blocksRaycasts = false;
                cg5.interactable = false; cg5.alpha = 0f; cg5.blocksRaycasts = false;
                break;
            // foods
            case 2: 
                cg2.interactable = true; cg2.alpha = 1f; cg2.blocksRaycasts = true;
                
                cg0.interactable = false; cg0.alpha = 0f; cg0.blocksRaycasts = false;
                cg1.interactable = false; cg1.alpha = 0f; cg1.blocksRaycasts = false;
                cg3.interactable = false; cg3.alpha = 0f; cg3.blocksRaycasts = false;
                cg4.interactable = false; cg4.alpha = 0f; cg4.blocksRaycasts = false;
                cg5.interactable = false; cg5.alpha = 0f; cg5.blocksRaycasts = false;
                break;
            // alchemy
            case 3: 
                cg3.interactable = true; cg3.alpha = 1f; cg3.blocksRaycasts = true;
                
                cg0.interactable = false; cg0.alpha = 0f; cg0.blocksRaycasts = false;
                cg1.interactable = false; cg1.alpha = 0f; cg1.blocksRaycasts = false;
                cg2.interactable = false; cg2.alpha = 0f; cg2.blocksRaycasts = false;
                cg4.interactable = false; cg4.alpha = 0f; cg4.blocksRaycasts = false;
                cg5.interactable = false; cg5.alpha = 0f; cg5.blocksRaycasts = false;
                break;
            // tools
            case 4: 
                cg4.interactable = true; cg4.alpha = 1f; cg4.blocksRaycasts = true;
                
                cg0.interactable = false; cg0.alpha = 0f; cg0.blocksRaycasts = false;
                cg1.interactable = false; cg1.alpha = 0f; cg1.blocksRaycasts = false;
                cg2.interactable = false; cg2.alpha = 0f; cg2.blocksRaycasts = false;
                cg3.interactable = false; cg3.alpha = 0f; cg3.blocksRaycasts = false;
                cg5.interactable = false; cg5.alpha = 0f; cg5.blocksRaycasts = false;
                break;
            case 5: 
                cg5.interactable = true; cg5.alpha = 1f; cg5.blocksRaycasts = true;
                
                cg0.interactable = false; cg0.alpha = 0f; cg0.blocksRaycasts = false;
                cg1.interactable = false; cg1.alpha = 0f; cg1.blocksRaycasts = false;
                cg2.interactable = false; cg2.alpha = 0f; cg2.blocksRaycasts = false;
                cg3.interactable = false; cg3.alpha = 0f; cg3.blocksRaycasts = false;
                cg4.interactable = false; cg4.alpha = 0f; cg4.blocksRaycasts = false;
                break;
        }
    }
}
