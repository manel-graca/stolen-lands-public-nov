using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler
{
	public RectTransform dragRectTransform;

	public Canvas canvas;


	public void OnPointerEnter(PointerEventData eventData)
	{
		if (CursorManager.instance != null)
		{
			Texture2D cursor = CursorManager.instance.defaultCursor;
			Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
		}
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if (PlayerUI.instance != null && !PlayerUI.instance.canMoveUI) return;
		if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle) return;
		dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if (PlayerUI.instance != null && !PlayerUI.instance.canMoveUI) return;
		if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle) return;
		dragRectTransform.SetAsLastSibling();
	}
}
