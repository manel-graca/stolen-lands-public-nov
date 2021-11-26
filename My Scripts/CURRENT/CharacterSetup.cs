using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Michsky.LSS;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSetup : MonoBehaviour
{
    [SerializeField] private GameObject character;

    public string playerName;

    [SerializeField] private GameObject storyPanelPlayButton;
    [SerializeField] private GameObject storyPanelBackButton;
    [SerializeField] private GameObject characterSetupWindow;
    [SerializeField] private GameObject nameSavedObject;
    [SerializeField] private GameObject enterNameFirstObject;
    [SerializeField] private GameObject playerHelmet;
    [SerializeField] private TextMeshProUGUI playButtonCounter;
    [SerializeField] private Button playButton;
    [SerializeField] private Material beardMaterial;
    [SerializeField] private Slider beardSlider;
    [SerializeField] private float beardStrength;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private float standAnimTime;
    [SerializeField] private float negativeAnimTime;
    [SerializeField] private GameObject storyPanel;

    private Animator animator;
    public LoadingScreenManager lsm;
    private bool nameHasBeenSaved;

    private void Start()
    {
        animator = character.GetComponent<Animator>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (nameHasBeenSaved)
        {
            playButton.interactable = true;
        }
    }

    public void ChangeBeardStrength(float value)
    {
        beardStrength = value;
        beardMaterial.SetFloat("_Cutoff", beardStrength);
        PlayerPrefs.SetFloat("beard", value);
    }

    public void HideShowHelmet(bool boolean)
    {
        playerHelmet.SetActive(boolean);
        if (boolean)
        {
            PlayerPrefs.SetString("helmet", "true");
            return;
        }
        else
        {
            PlayerPrefs.SetString("helmet", "false");
        }
    }

    public void GetAndSavePlayerName(string name)
    {
        if (name.Length <= 1)
        {
            enterNameFirstObject.SetActive(true);
            StartCoroutine(ToggleNegativeBool());
            return;
        }

        playerName = name;

        nameHasBeenSaved = true;
        PlayerPrefs.SetString("playerName", name);

        enterNameFirstObject.SetActive(false);
        nameSavedObject.SetActive(true);

        playerNameText.text = name + "."; // story panel
    }

    public void PlayGame()
    {
        lsm.LoadScene("Level 1");
        FindObjectOfType<PlayerSoundManager>().PlaySpecialClickSound();
    }

    public void NextButton() // BUTTON
    {
        if (!nameHasBeenSaved)
        {
            enterNameFirstObject.SetActive(true);
            StartCoroutine(ToggleNegativeBool());
            return;
        }

        animator.SetBool("standUp", true);
        StartCoroutine(AfterNextButtonRoutine());
    }

    public void OpenCharacterSetupWindow()
    {
        StartCoroutine(OpenCharacterSetupWindowRoutine());
    }

    public void CloseCharacterSetupWindow()
    {
        characterSetupWindow.transform.DOScale(Vector3.zero, 0.35f);
    }

    public void CloseStoryPanel()
    {
        storyPanel.transform.DOScale(Vector3.zero, 0.35f);
        storyPanelPlayButton.SetActive(false);
        storyPanelBackButton.SetActive(false);
    }

    IEnumerator AfterNextButtonRoutine()
    {
        Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);
        characterSetupWindow.transform.DOScale(Vector3.zero, 0.35f);

        yield return new WaitForSeconds(3.05f);

        storyPanel.transform.DOScale(scale, 0.8f);
        
        yield return new WaitForSeconds(0.8f);
        
        StartCoroutine(PlayButtonTextCountdownRoutine());
        storyPanelBackButton.SetActive(true);

        yield return new WaitForSeconds(4f);

        storyPanelPlayButton.SetActive(true);
    }

    IEnumerator OpenCharacterSetupWindowRoutine()
    {
        Vector3 scale = new Vector3(0.4f, 0.4f, 0.4f);
        yield return new WaitForSeconds(1.25f);
        characterSetupWindow.SetActive(true);
        characterSetupWindow.transform.DOScale(scale, 0.8f);
    }

    IEnumerator ToggleNegativeBool()
    {
        animator.SetBool("negative", true);
        yield return new WaitForSeconds(negativeAnimTime);
        animator.SetBool("negative", false);
        StartCoroutine(EnterNameFirstAlertRoutine());
    }

    IEnumerator EnterNameFirstAlertRoutine()
    {
        yield return new WaitForSeconds(4f);
        enterNameFirstObject.SetActive(false);
    }

    IEnumerator PlayButtonTextCountdownRoutine()
    {
        playButtonCounter.gameObject.SetActive(true);
        playButtonCounter.text = 4f.ToString();
        float t = 0f;
        float i = 4f;
        while (t < 4f)
        {
            i -= Time.deltaTime;
            t += Time.deltaTime;
            
            i = Mathf.Clamp(i,0f,4f);
            
            playButtonCounter.text = i.ToString("F");
            yield return null;
        }
        playButtonCounter.gameObject.SetActive(false);
    }
}