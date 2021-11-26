using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		PlayerUI.instance.HideTooltip();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		PlayerUI.instance.HideTooltip();
	}
}
