using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelLoader : MonoBehaviour
{
	[SerializeField] private CharacterSetup characterSetup;
	
	[SerializeField] private GameObject characterSetupPanel;
	[SerializeField] private GameObject optionsPanel;
	[SerializeField] private CanvasGroup optionsCanvasGroup;
	[SerializeField] private GameObject menuCamera;
	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject menuMenhir;
	[SerializeField] private GameObject menuTextHolder;
	
	public GameObject quitGamePopup;
	
	Scene currentScene;

	public void ContinueGame()
	{
		SceneManager.LoadSceneAsync("Level 1");
	}
	public void NewGame()
	{
		playerCamera.SetActive(true);
		menuCamera.SetActive(false);
		characterSetup.OpenCharacterSetupWindow();
		CloseOptions();
	}

	public void BackToMenu()
	{
		playerCamera.SetActive(false);
		menuCamera.SetActive(true);
		
		characterSetupPanel.transform.DOScale(Vector3.zero, 0.35f);
		characterSetup.CloseStoryPanel();
	}


	public void OpenOptions()
	{
		Vector3 scale = new Vector3(0.45f, 0.45f, 0.45f);
		
		playerCamera.SetActive(false);
		menuCamera.SetActive(true);
		optionsPanel.transform.DOScale(scale, 0.25f);
		optionsCanvasGroup.alpha = 1;
		optionsCanvasGroup.interactable = true;
		optionsCanvasGroup.blocksRaycasts = true;
	}

	public void CloseOptions()
	{
		Vector3 scale = new Vector3(0, 0, 0);
		optionsPanel.transform.DOScale(scale, 0.25f);
	}
	public void ShowQuitPopup()
	{
		Vector3 scale = new Vector3(1f, 1f, 1f);
		
		quitGamePopup.SetActive(true);
		quitGamePopup.transform.DOScale(scale, 0.25f);
	}

	public void CloseQuitPopup()
	{
		Vector3 scale = new Vector3(0f, 0f, 0f);
		
		quitGamePopup.SetActive(false);
		quitGamePopup.transform.DOScale(scale, 0.25f);
	}

	public void Quit() // QUIT POPUP BUTTON, CONFIRMATION ONLY
	{
		Application.Quit();
	}
	
}
