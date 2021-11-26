using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class AbilityTreeCategoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] GameObject defensive;
	[SerializeField] GameObject attack;
	[SerializeField] GameObject effects;

	public void OnPointerEnter(PointerEventData eventData)
	{
		transform.DOLocalMoveX(-18f, 0.25f, false);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.DOLocalMoveX(0, 0.25f, false);
	}

	public void ShowDefensive()
	{
		defensive.GetComponentInChildren<AbilityTreeUIMasterController>().SetFirstPage();
		defensive.SetActive(true);
		attack.SetActive(false);
		effects.SetActive(false);
	}
	public void ShowAttack()
	{
		attack.GetComponentInChildren<AbilityTreeUIMasterController>().SetFirstPage();
		defensive.SetActive(false);
		attack.SetActive(true);
		effects.SetActive(false);
	}
	public void ShowEffects()
	{
		effects.GetComponentInChildren<AbilityTreeUIMasterController>().SetFirstPage();
		defensive.SetActive(false);
		attack.SetActive(false);
		effects.SetActive(true);
	}
}
