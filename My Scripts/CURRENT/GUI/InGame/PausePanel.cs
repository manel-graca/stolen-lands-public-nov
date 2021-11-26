using System;
using UnityEngine;
using StolenLands.Player;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace StolenLands.UI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameObject inGameOptionsPanel;
        [SerializeField] private GameObject gamePauseMenu;
        [SerializeField] private Canvas playerCanvas;
        
        public bool isMenuActive = false;
        public bool optionsPressed = false;

        private CanvasGroup inGameOptionsGroup;
        private PlayerControllerV2 playerController;
        private PlayerUI ui;

        private void Start()
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerControllerV2>();
            ui = PlayerUI.instance;
            inGameOptionsGroup = inGameOptionsPanel.GetComponent<CanvasGroup>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !ui.characterOn && !ui.inventoryOn && !ui.abilityTreeOn && !ui.craftingOn)
            {
                TogglePauseMenu();
            }
        }

        private void TogglePauseMenu()
        {
            if (!isMenuActive)
            {
                if (inGameOptionsPanel.activeSelf || optionsPressed)
                {
                    HideOptions();
                }
                isMenuActive = true;
                gamePauseMenu.SetActive(true);
                playerController.isInputBlock = true;
                ui.dialogueManager.SetActive(false);
                ui.SetTimeScale(0);
            }
            else
            {
                optionsPressed = false;
                isMenuActive = false;
                HideOptions();
                gamePauseMenu.SetActive(false);
                playerController.isInputBlock = false;
                ui.dialogueManager.SetActive(true);
                ui.SetTimeScale(1);
            }
        }

        public void GoToMainMenu()
		{
            SceneManager.LoadSceneAsync("Character Setup");
		}

        public void ContinueGame()
		{
            optionsPressed = false;
            isMenuActive = false;
            HideOptions();
            gamePauseMenu.SetActive(false);
            playerController.isInputBlock = false;
            //ui.dialogueManager.SetActive(true);
            ui.SetTimeScale(1);
        }

        public void BackToPausePanel() // button
        {
            isMenuActive = true;
            HideOptions();
            gamePauseMenu.SetActive(true);
        }

        public void ShowOptions()
        {
            optionsPressed = true;
            isMenuActive = false;
            gamePauseMenu.SetActive(false);
            inGameOptionsGroup.alpha = 1;
            inGameOptionsGroup.interactable = true;
            inGameOptionsGroup.blocksRaycasts = true;
        }

        public void HideOptions()
        {
            optionsPressed = false;
            inGameOptionsGroup.alpha = 0;
            inGameOptionsGroup.interactable = false;
            inGameOptionsGroup.blocksRaycasts = false;
        }


       

    }
}

