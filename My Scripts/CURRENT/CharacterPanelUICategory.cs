using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPanelUICategory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private float catID;
    
    [SerializeField] private Vector3 initPos;
    [SerializeField] private Vector3 finalPos;
    
    private PlayerUI ui;

    private void Start()
    {
        ui = PlayerUI.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.transform.DOLocalMove(finalPos, 0.35f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        this.gameObject.transform.DOLocalMove(initPos, 0.35f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (catID)
        {
            case 0:
                PlayerInput.instance.openInventory = true;
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 1:
                PlayerInput.instance.openSpellbook = true;
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 2:
                ui.OpenCrafting();
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 3:
                ui.OpenCloseCurrencyConverter(true);
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
        }
        
        
    }
}
