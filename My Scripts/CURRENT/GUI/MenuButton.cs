using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StolenLands.Player;
using TMPro;
using UnityEngine;
enum ButtonFunction{ continueGame, newGame, options, quit}
public class MenuButton : MonoBehaviour
{
    [SerializeField] private ButtonFunction buttonFunction;
    [SerializeField] private MenuLevelLoader levelLoader;
    private int id;
    [SerializeField] bool canBeSelected = true;
    
    PlayerSoundManager soundM;
    private void Start()
    {
        soundM = PlayerSoundManager.instance;
        
        switch (buttonFunction)
        {
            case ButtonFunction.continueGame:
                id = 0;
                if (!FindObjectOfType<GameManager>().characterSetup)
                {
                    GetComponent<TextMeshPro>().color = new Color(1,1,1,0.15f);
                }
                else
                {
                    GetComponent<TextMeshPro>().color = new Color(1,1,1,1);
                }
                break;
            case ButtonFunction.newGame:
                id = 1;
                if (!FindObjectOfType<GameManager>().characterSetup)
                {
                    GetComponent<TextMeshPro>().color = new Color(1,1,1,1);
                }
                else
                {
                    GetComponent<TextMeshPro>().color = new Color(1,1,1,0.15f);
                }
                break;
            
            case ButtonFunction.options:
                id = 2;
                break;
            
            case ButtonFunction.quit:
                id = 3;
                break;
        }
    }

    private void OnMouseDown()
    {
        Vector3 scale = new Vector3(1f, 1f, 1f);
        if (id == 0)
        {
            if (canBeSelected)
            {
                levelLoader.ContinueGame();
                transform.DOScale(scale, 0.15f);
                return;
            }
        }
        if (id == 1)
        {
            levelLoader.NewGame();
            transform.DOScale(scale, 0.15f);
            soundM.PlayButtonMouseClickSound();
            return;
        }
        if (id == 2)
        {
            levelLoader.OpenOptions();
            transform.DOScale(scale, 0.15f);
            soundM.PlayButtonMouseClickSound();
            return;
        }
        if (id == 3)
        {
            levelLoader.ShowQuitPopup();
            transform.DOScale(scale, 0.15f);
            soundM.PlayButtonMouseClickSound();
            return;
        }
    }

    private void OnMouseEnter()
    {
        Vector3 scale = new Vector3(1.075f, 1.075f, 1.075f);
        if (canBeSelected)
        {
            transform.DOScale(scale, 0.2f);
            soundM.PlayButtonMouseOverSound();
        }
    }

    private void OnMouseExit()
    {
        Vector3 scale = new Vector3(1f, 1f, 1f);
        transform.DOScale(scale, 0.15f);
    }
}
