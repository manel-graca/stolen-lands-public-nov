using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CursorManager : MonoBehaviour
{
	public static CursorManager instance;
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			return;
		}
		
		if(instance != null)
		{
			Destroy(instance);
			instance = this;
		}
	}

	public Texture2D defaultCursor;
	[Space]
	public Texture2D overEnemyCursor;
	public Texture2D interactCursor;
	public Texture2D overGatherCursor;
	public Texture2D lootCursor;

	public void SetCombatCursor()
	{
		Cursor.SetCursor(overEnemyCursor, Vector2.zero, CursorMode.Auto);
	}

	public void SetGatherCursor()
	{
		Cursor.SetCursor(overGatherCursor, Vector2.zero, CursorMode.Auto);
	}

	public void SetDefaultCursor()
	{
		Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
	}

	public void SetInteractCursor()
	{
		Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
	}

	public void SetLootCursor()
	{
		Cursor.SetCursor(lootCursor, Vector2.zero, CursorMode.Auto);
	}

}
