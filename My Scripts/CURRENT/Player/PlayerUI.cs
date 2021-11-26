using System;
using System.Collections;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using StolenLands.Abilities;
using StolenLands.Cinematics;
using UnityEngine;
using UnityEngine.UI;
using StolenLands.Player;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class PlayerUI : MonoBehaviour
{
    #region Singleton

    public static PlayerUI instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion
    
    [Title("Dialogue System")]
    [Space] 
    public GameObject dialogueManager;

    [Title("Visual Warnings")] 
    [Space] 
    [SerializeField]
    private float timeBetweenTextSpawns;

    [SerializeField] private GameObject warningTextPrefab;
    [SerializeField] private Transform warningParent;
    [SerializeField] private GameObject hintTextObj;
    [SerializeField] private Transform infoTextParent;
    [SerializeField] private GameObject infoTextPrefab;
    [SerializeField] private GameObject statusTextPrefab;
    [SerializeField] private GameObject buffTextPrefab;
    [SerializeField] private Transform statusParent;
    [SerializeField] private GameObject inCombatWarningHolder;

    [Space] 
    [Title("Double Verification Popups")]
    public GameObject itemDeletePopup;

    [Space] 
    public GameObject spendSpellPointsPopup;
    [SerializeField] private GameObject spellPointsPopupText;
    public Ability lastSpell = null;
    public AbilityTreeSlot lastSpellSlot = null;

    [Space]
    [Title("Currencies and Shops")]
    [Space]
    [SerializeField] private GameObject masterShopWindow;
    [SerializeField] private GameObject currencyConverter;
    [Space]
    [Title("Inv and Loot")] 
    [Space] 
    [SerializeField] Transform itemDropCanvas;
    [SerializeField] Canvas dropCanvas;
    
    public GameObject inventorySlotPrefab;
    public Transform inventorySlotsHolder;

    public TextMeshProUGUI goldOwnedText;
    public TextMeshProUGUI silverOwnedText;
    public TextMeshProUGUI copperOwnedText;
    
    [Space] 
    [Title("Tooltip")] 
    [Space] 
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Text tooltipGearPlaceText;
    public float timeUntillTooltipActivation = 0.5f;
    private Text tooltipText;

    [Space] 
    [Title("Player")] 
    [Space] 
    public TextMeshProUGUI characterWindowName;
    public Transform topPlayerTransform;
    public Transform leftPlayerSpawner;
    public Transform rightPlayerSpawner;
    public GameObject resourceTooltip;
    public Text resourceNameText;
    public Text resourceTypeText;
    public Image resourceIcon;

    [Space] 
    [Title("Stats Panel Texts")] 
    [Space] 
    [SerializeField] private TextMeshProUGUI spellPointsText;
    [SerializeField] private TextMeshProUGUI spellPointsPerLevelText;

    [Space] 
    [Title("Stats Panel Texts")]
    [Space] 
    [SerializeField] private Text agilityText;
    [SerializeField] private Text armorText;
    [SerializeField] private Text attackDmgText;
    [SerializeField] private Text dexterityText;
    [SerializeField] private Text focusText;
    [SerializeField] private Text enduranceText;
    [SerializeField] private Text intellectText;
    [SerializeField] private Text magicDmgText;
    [SerializeField] private Text magicResistText;
    [SerializeField] private Text staminaText;
    [SerializeField] private Text strengthText;
    [SerializeField] private Text vitalityText;

    [Space] 
    [Title("Butons")] 
    [Space]
    [SerializeField]
    private GameObject openSpellPointsButton;

    [SerializeField] GameObject closeSpellPointsButton;

    [Title("Windows and other player UI Elements")] 
    [Space] 
    [SerializeField] GameObject abilityTreeWindow;
    [SerializeField] GameObject craftingWindow;
    [SerializeField] GameObject spellPointsWindow;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject characterWindow;
    [SerializeField] GameObject lootWindow;
    [SerializeField] GameObject controlsWindow;
    [SerializeField] GameObject allItemsWindow;
    [Space] 
    [SerializeField] private GameObject playerTopPanel;
    [SerializeField] private GameObject actionBar;

    [Title("Arrays and Lists")] 
    [SerializeField]
    private AbilitySlot[] abilitySlots;

    [Space] 
    [Title("Booleans")] 
    public bool canMoveUI;
    public bool inventoryOn;
    public bool abilityTreeOn;
    public bool characterOn;
    public bool craftingOn;

    private float timeSinceTextSpawn = Mathf.Infinity;

    private readonly Vector3 spellPointsInitialPos = new Vector3(-84.7f, 0.8f, 0f); // unsure about readonly, was suggestion of IDE
    private readonly Vector3 spellPointsTargetPos = new Vector3(0f, 0.8f, 0f); // unsure about readonly, was suggestion of IDE

    // cached refs
    PlayerInput pInput;
    private CraftingManager craftManager;
    private PlayerSoundManager soundM;
    private CharacterStats cStats;
    private PlayerResources pResources;
    private ItemMoverUI itemMover;
    private PlayerCombatController pCombat;
    private PlayerControllerV2 pController;
    private Inventory inv;
    OptionsManager options;
    GameManager manager;
    [SerializeField] private Cinematics cinematic;
    
    void Start()
    {
        manager = GameManager.instance;
        inv = Inventory.instance;
        options = FindObjectOfType<OptionsManager>();
        craftManager = GetComponent<CraftingManager>();
        pInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        cStats = CharacterStats.instance;
        soundM = PlayerSoundManager.instance;
        pResources = PlayerResources.instance;
        pCombat = PlayerCombatController.instance;
        
        pController = pInput.GetComponent<PlayerControllerV2>();
        itemMover = FindObjectOfType<ItemMoverUI>();
        tooltipText = tooltip.GetComponentInChildren<Text>();
        tooltip.gameObject.SetActive(false);
        closeSpellPointsButton.SetActive(false);
        controlsWindow.SetActive(false);
        allItemsWindow.SetActive(false);
        spellPointsWindow.transform.localPosition = spellPointsInitialPos;
        
        string name = PlayerPrefs.GetString("playerName");
        if (name.Length > 1)
        {
            DialogueLua.SetVariable("PlayerName", name);
            characterWindowName.text = name;
        }
        else
        {
            DialogueLua.SetVariable("PlayerName", "Player");
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Level 1");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SceneManager.LoadScene("Sandbox");
            }
        } // debug restart & sandbox scene

        EscToCloseWindows();

        HandleInCombatUI();

        if (abilityTreeOn)
        {
            UpdatePlayerSpellPoints();
        }

        if (characterOn)
        {
            UpdateCharacterStatsWindow();
        }
        GetBools();
        WindowsHandler();
        timeSinceTextSpawn += Time.deltaTime;
    }

    private void EscToCloseWindows()
    {
        if (characterOn || inventoryOn || abilityTreeOn || craftingOn)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (characterOn)
                {
                    OpenCloseCharacter(false);
                }

                if (inventoryOn)
                {
                    OpenCloseInventory(false);
                }

                if (abilityTreeOn)
                {
                    OpenCloseAbility(false);
                }

                if (craftingOn)
                {
                    CloseCrafting();
                }
            }
        } // press esc to close windows handler
    }

    private void HandleInCombatUI()
    {
        if (pCombat.inCombat)
        {
            inCombatWarningHolder.SetActive(true);
        }
        else
        {
            inCombatWarningHolder.SetActive(false);
        }
    }

    public void SetBookQuestState()
    {
        QuestLog.SetQuestEntryState("The sacred book", 0, QuestState.ReturnToNPC);
    }
    public void SetTimeScale(int i)
    {
        Time.timeScale = i;
    }
    private void UpdatePlayerSpellPoints()
    {
        spellPointsText.text = pResources.currentPoints.ToString();
        spellPointsPerLevelText.text = "+" + pResources.pointsPerLevel.ToString();
    }
    private void UpdateCharacterStatsWindow()
    {
        agilityText.text = cStats.agilityAmount.ToString();
        armorText.text = cStats.armorAmount.ToString();
        attackDmgText.text = cStats.attackDmgAmount.ToString();
        dexterityText.text = cStats.dexterityAmount.ToString();
        focusText.text = cStats.focusAmount.ToString();
        enduranceText.text = cStats.enduranceAmount.ToString();
        intellectText.text = cStats.intellectAmount.ToString();
        magicDmgText.text = cStats.magicDmgAmount.ToString();
        magicResistText.text = cStats.magicResistAmount.ToString();
        staminaText.text = cStats.staminaAmount.ToString();
        strengthText.text = cStats.strengthAmount.ToString();
        vitalityText.text = cStats.vitalityAmount.ToString();
    }
    private void GetBools()
    {
        if(pInput.isTyping) return;
        if(pController.isInputBlock) return;
        
        inventoryOn = pInput.GetInput(pInput.openInventory);
        abilityTreeOn = pInput.GetInput(pInput.openSpellbook);
        characterOn = pInput.GetInput(pInput.openCharacterWindow);
        craftingOn = pInput.GetInput(pInput.openCraftWindow);
    }
    public void ActivateDeactivateUI(bool onOff)
    {
        OpenCloseAbility(onOff);
        OpenCloseCharacter(onOff);
        OpenCloseInventory(onOff);
        OpenCloseAllItems(onOff);
        OpenCloseCurrencyConverter(onOff);
        OpenControls(onOff);
        CloseCrafting();
        CloseSpellPointsWindow();
        CloseSpendSpellPointsPopup();
        
        lootWindow.SetActive(onOff);
        actionBar.SetActive(onOff);
        playerTopPanel.SetActive(onOff);
        options.HideShowMinimap(onOff);
        options.HideShowCompass(onOff);
        
        Debug.Log("Activasting UI");
    }

    public void ActivateUIWithDelay()
    {
        if (!cinematic.isCinematicRunning)
        {
            StartCoroutine(ActivateUIRoutine());
        }
    }
    IEnumerator ActivateUIRoutine()
    {
        characterWindow.SetActive(false);
        inventoryWindow.SetActive(false);
        lootWindow.SetActive(false);
        abilityTreeWindow.SetActive(false);
        actionBar.SetActive(false);
        playerTopPanel.SetActive(false);
        options.HideShowMinimap(false);
        options.HideShowCompass(false);
        craftingWindow.SetActive(false);

        yield return new WaitForSeconds(1f);

        lootWindow.SetActive(true);
        actionBar.SetActive(true);
        playerTopPanel.SetActive(true);
        options.HideShowMinimap(true);
        options.HideShowCompass(true);
    }
    private void WindowsHandler()
    {
        if (inventoryOn || characterOn || abilityTreeOn || craftingOn)
        {
            manager.anyWindowOpen = true;
        }
        else
        {
            manager.anyWindowOpen = false;
        }
        
        if (inventoryOn)
        {
            inventoryWindow.SetActive(true);
            CurrencyOwnedUpdate();
        }
        else
        {
            inventoryWindow.SetActive(false);
        }

        if (characterOn)
        {
            characterWindow.SetActive(true);
        }
        else
        {
            characterWindow.SetActive(false);
        }

        if (abilityTreeOn)
        {
            abilityTreeWindow.SetActive(true);
        }
        else
        {
            abilityTreeWindow.SetActive(false);
        }

        if (craftingOn)
        {
            OpenCrafting();
        }
        else
        {
            CloseCrafting();
        }
    }

    public void CurrencyOwnedUpdate()
    {
        var goldOwned = inv.GetCurrencyOwned("Gold");
        var silverOwned = inv.GetCurrencyOwned("Silver");
        var copperOwned = inv.GetCurrencyOwned("Copper");
        
        goldOwnedText.text = goldOwned.ToString();
        silverOwnedText.text = silverOwned.ToString();
        copperOwnedText.text = copperOwned.ToString();
    }

    public void ClickToUnlockAbility() // UI BUTTON
    {
        if (pResources.currentPoints >= lastSpell.requiredPointsToUnlock)
        {
            pResources.currentPoints -= lastSpell.requiredPointsToUnlock;
            lastSpellSlot.UnlockAbilityUI();
            CloseSpendSpellPointsPopup();
            PlayerSoundManager.instance.PlayOnAbilityUnlockSound();
        }
        else
        {
            if (pResources.currentPoints < lastSpell.requiredPointsToUnlock)
            {
                InstantiateWarning("Not enough spell points");
                return;
            }

            if (pResources.currentPoints < lastSpell.requiredPointsToUnlock)
            {
                InstantiateWarning("Level too low");
                return;
            }
        }
    }
    public void OpenSpendSpellPointsPopup(Ability ability)
    {
        spellPointsPopupText.SetActive(true);
        spendSpellPointsPopup.SetActive(true);
        spellPointsPopupText.GetComponent<TextMeshProUGUI>().text = String.Format("Spend {0} points to unlock {1}?",
            ability.requiredPointsToUnlock, ability.abilityName);
    }
    public void CloseSpendSpellPointsPopup()
    {
        spendSpellPointsPopup.SetActive(false);
        spellPointsPopupText.gameObject.SetActive(false);
        lastSpell = null;
        lastSpellSlot = null;
    }

    public void OpenSpellPointsWindow()
    {
        spellPointsWindow.transform.DOLocalMove(spellPointsTargetPos, 0.4f);
        openSpellPointsButton.SetActive(false);
        closeSpellPointsButton.SetActive(true);
    }

    public void CloseSpellPointsWindow()
    {
        spellPointsWindow.transform.DOLocalMove(spellPointsInitialPos, 0.4f);
        closeSpellPointsButton.SetActive(false);
        openSpellPointsButton.SetActive(true);
    }
    public void InstantiateLootWindow()
    {
        int existing = FindObjectsOfType<LootWindow>().Length;
        if (existing == 0)
        {
            GameObject ltWnd = Instantiate(lootWindow);
            ltWnd.SetActive(true);
            ltWnd.GetComponent<MouseDragUI>().canvas = dropCanvas;
            ltWnd.transform.SetParent(itemDropCanvas);
            ltWnd.transform.localPosition = new Vector2(0f, 0f);
            ltWnd.transform.localScale = new Vector2(0.6f, 0.6f);
            return;
        }
        else
        {
            InstantiateWarning("One loot window already open");
        }
    }
    public void ActivateHintText(string hintText)
    {
        hintTextObj.GetComponentInChildren<TMP_Text>().text = hintText;
        hintTextObj.SetActive(true);
    }
    public void DeactivateHintText()
    {
        hintTextObj.GetComponentInChildren<TMP_Text>().text = String.Empty;
        hintTextObj.SetActive(false);
    }

    public GameObject InstantiateBuffMessage(string buffText)
    {
        if (timeSinceTextSpawn > timeBetweenTextSpawns)
        {
            timeSinceTextSpawn = 0f;
            GameObject newStatus = Instantiate(buffTextPrefab) as GameObject;
            newStatus.transform.SetParent(statusParent, false);
            newStatus.GetComponent<TMP_Text>().text = buffText;
            return newStatus;
        }

        return null;
    }
    public GameObject InstantiateStatusMessage(string statusText)
    {
        if (timeSinceTextSpawn > timeBetweenTextSpawns)
        {
            timeSinceTextSpawn = 0f;
            GameObject newStatus = Instantiate(statusTextPrefab) as GameObject;
            newStatus.transform.SetParent(statusParent, false);
            newStatus.GetComponent<TMP_Text>().text = statusText;
            return newStatus;
        }

        return null;
    }
    public void InstantiateInformationMessage(string infoText)
    {
        GameObject newInfo = Instantiate(infoTextPrefab) as GameObject;
        
        newInfo.transform.SetParent(infoTextParent, false);
        newInfo.GetComponent<TMP_Text>().text = infoText;
        Destroy(newInfo, 3f);
    }
    public void InstantiateWarning(string warningText)
    {
        if (timeSinceTextSpawn > timeBetweenTextSpawns)
        {
            timeSinceTextSpawn = 0f;
            GameObject newWarning = Instantiate(warningTextPrefab) as GameObject;
            newWarning.transform.SetParent(warningParent, false);
            newWarning.GetComponent<TMP_Text>().text = warningText;
            soundM.PlayInterfaceSound(soundM.errorClickSound);
        }
    }
    public void ShowTooltipNoInformation(Vector3 pos, string text)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = pos;
        tooltipText.text = text;
    }
    public void ShowTooltip(Vector2 pos, IDescriber description)
    {
        if (description != null)
        {
            var rect1 = tooltip.GetComponent<RectTransform>();
            var xOffset = 250f;
            
            if(Input.mousePosition.x > Screen.width / 2f) // right side
            {
                tooltip.transform.position = pos;
            }
            else // left side
            {
                var newPosL = pos + new Vector2(xOffset, 0f);
                tooltip.transform.position = newPosL;
            }
            
            tooltip.SetActive(true);
            tooltipText.text = description.GetDescription();
            tooltipGearPlaceText.text = description.GetPlaceText();
        }
    }
    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public GameObject GetRandomSpawnerPos() // FOR TEXT DAMAGE NUMBERS
    {
        int a = 0;
        int b = 1;
        var rnd = Random.Range(a, b + 1);
        switch (rnd)
        {
            case 0:
                if (rnd == 0)
                {
                    return leftPlayerSpawner.gameObject;
                }

                break;
            case 1:
                if (rnd == 1)
                {
                    return rightPlayerSpawner.gameObject;
                }

                break;
        }

        return null;
    }

    public void OpenCloseInventory(bool on)
    {
        inventoryOn = on;
        pInput.openInventory = on;
    }

    public void OpenCloseCharacter(bool on)
    {
        characterOn = on;
        pInput.openCharacterWindow = on;
    }

    public void OpenCloseAbility(bool on)
    {
        abilityTreeOn = on;
        pInput.openSpellbook = on;
    }

    public void OpenCloseAllItems(bool on)
    {
        FindObjectOfType<CheatsManager>().CloseCheatWindowButton();
        allItemsWindow.SetActive(on);
    }

    public void OpenControls(bool on)
    {
        controlsWindow.SetActive(on);
    }
    public void OpenCrafting()
    {
        var cg = craftingWindow.GetComponent<CanvasGroup>();
        pInput.openCraftWindow = true;
        craftingOn = true;
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    public void CloseCrafting()
    {
        var cg = craftingWindow.GetComponent<CanvasGroup>();
        pInput.openCraftWindow = false;
        craftingOn = false;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public void OpenCloseCurrencyConverter(bool a)
    {
        currencyConverter.SetActive(a);
    }

    public void OpenCloseMasterShop(bool a)
    {
        masterShopWindow.SetActive(a);
        pInput.openInventory = a;
    }

    public void EnableNecessaryUI()
    {
        actionBar.SetActive(true);
        playerTopPanel.SetActive(true);
        options.HideShowCompass(true);
        options.HideShowMinimap(true);
    }

}