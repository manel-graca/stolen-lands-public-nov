using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingCategoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Tooltip("First is 0, Last is 4 (if there are 5)")] [SerializeField] private int orderID;
    
    private CraftingManager craftingManager;
    private PlayerUI ui;
    private PlayerSoundManager soundM;

    readonly Vector3 startingScale = new Vector3(1f,1f,1f);
    readonly Vector3 targetScale = new Vector3(1.2429f,1.2429f,1.2429f);
        
    private void Start()
    {
        craftingManager = FindObjectOfType<CraftingManager>();
        ui = PlayerUI.instance;
        soundM = PlayerSoundManager.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        soundM.PlayInterfaceSound(soundM.mouseOverButton);
        transform.DOScale(targetScale, .35f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(startingScale, .35f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        soundM.PlayInterfaceSound(soundM.mouseClickButton);
        craftingManager.CategoryClickHandler(orderID);
    }
}
