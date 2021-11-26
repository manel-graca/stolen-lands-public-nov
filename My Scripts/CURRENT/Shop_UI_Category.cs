using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class Shop_UI_Category : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private float catID;
    
    [SerializeField] private Vector3 initPos;
    [SerializeField] private Vector3 finalPos;
    
    Shop_MasterController master;

    private void Start()
    {
        master = GetComponentInParent<Shop_MasterController>();
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
                master.itemCategories[0].SetActive(true);
                
                master.itemCategories[1].SetActive(false);
                master.itemCategories[2].SetActive(false);
                master.itemCategories[3].SetActive(false);
                
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 1:
                master.itemCategories[1].SetActive(true);
                
                master.itemCategories[0].SetActive(false);
                master.itemCategories[2].SetActive(false);
                master.itemCategories[3].SetActive(false);
                
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 2:
                master.itemCategories[2].SetActive(true);
                
                master.itemCategories[1].SetActive(false);
                master.itemCategories[0].SetActive(false);
                master.itemCategories[3].SetActive(false);
                
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
            case 3:
                master.itemCategories[3].SetActive(true);
                
                master.itemCategories[1].SetActive(false);
                master.itemCategories[2].SetActive(false);
                master.itemCategories[0].SetActive(false);
                
                this.gameObject.transform.DOLocalMove(initPos, 0.35f);
                break;
        }
        
        
    }
}
