using System.Collections;
using System.Collections.Generic;
using StolenLands.Enemy;
using StolenLands.Player;
using UnityEngine;

public class CheatsManager : MonoBehaviour
{
    [SerializeField] private GameObject cheatsWindow;

    private GameObject player;
    private PlayerInput pInput;
    private PlayerCombatController pCombat;
    private PlayerMover pMover;
    private PlayerHealthController pHealth;
    private PlayerControllerV2 pController;
    private PlayerResources pResources;
    private OptionsManager options;
    private PlayerUI ui;

    private float defaultMoveSpeed;
    private float defaultHealth;
    private float defaultMaxHealth;
    private int oldLevel;
    private int defaultPlayerPoints;

    private bool cheatWindowOn = false;

    void Start()
    {
        cheatsWindow.SetActive(false);

        options = FindObjectOfType<OptionsManager>();
        player = GameObject.FindWithTag("Player");
        ui = PlayerUI.instance;
        pCombat = player.GetComponent<PlayerCombatController>();
        pHealth = player.GetComponent<PlayerHealthController>();
        pMover = player.GetComponent<PlayerMover>();
        pController = player.GetComponent<PlayerControllerV2>();
        pResources = player.GetComponent<PlayerResources>();
        pInput = player.GetComponent<PlayerInput>();

        defaultMoveSpeed = pMover.moveSpeed;
        defaultHealth = pHealth.health;
    }

    void Update()
    {
        if (options.cheatsAllowed)
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.X))
            {
                cheatWindowOn = !cheatWindowOn;
                if (cheatWindowOn)
                {
                    cheatsWindow.SetActive(true);
                    ui.SetTimeScale(0);
                    pController.StopReadInput();
                }
                else
                {
                    ui.SetTimeScale(1);
                    cheatsWindow.SetActive(false);
                    pController.StartReadingInput();
                }
            }
        }
    }

    public void TurnOnIsTyping()
    {
        pInput.isTyping = true;
    }

    public void KillAllEnemies()
    {
        var enemies = FindObjectsOfType<EnemyController>();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].CheatDie();
        }
    }

    public void AddCheatItem(string input)
    {
        var inv = GetComponentInParent<Inventory>();
        pInput.isTyping = false;
        
        if (input == "Gold" || input == "Silver" || input == "Copper")
        {
            inv.AddToCurrency(input, 36);
            return;
        }
        
        Item item = (Item) Resources.Load(input);
        if (item != null && inv != null)
        {
            if (inv.GetIfCanAddItem())
            {
                if (item.itemType == ItemType.Gear)
                {
                    var gear = (Gear) item;
                    if (gear != null)
                    {
                        gear.hasBeenEquipped = true;
                    }
                }
                inv.Add(item, 1);
            }
            else
            {
                PlayerUI.instance.InstantiateWarning("Inventory is full");
            }
        }
    }

    public void CloseCheatWindowButton() // BUTTON
    {
        ui.SetTimeScale(1);
        cheatsWindow.SetActive(false);
        pController.StartReadingInput();
    }

    public void ToggleGodMode(bool boolean)
    {
        pController.isGodMode = boolean;
        
        ui.InstantiateWarning("God mode: " + boolean);
    }

    public void ToggleInvisibility(bool boolean)
    {
        pController.playerInvisible = boolean;
        
        ui.InstantiateWarning("Invisible: " + boolean);
    }

    public void AddHealth(string input)
    {
        int newHpValue = 0;
        if (int.TryParse(input, out newHpValue))
        {
            if (newHpValue == 0)
            {
                DefaultHealth();
                return;
            }

            pHealth.health = newHpValue;
            pHealth.maxHealth = newHpValue;
            return;
        }

        DefaultHealth();
        return;
    }

    public void AddSpeed(string input)
    {
        int newSpeedValue;
        if (int.TryParse(input, out newSpeedValue))
        {
            if (newSpeedValue == 0)
            {
                DefaultMovementSpeed();
                return;
            }

            pMover.moveSpeed = newSpeedValue;
            return;
        }

        DefaultMovementSpeed();
        return;
    }

    public void AddDamage(string input)
    {
        int newDamageValue;
        if (int.TryParse(input, out newDamageValue))
        {
            if (newDamageValue == 0)
            {
                DefaultDamage();
                return;
            }

            pCombat.baseDamage = newDamageValue;
            return;
        }

        DefaultDamage();
        return;
    }

    public void AddLevel(string input)
    {
        oldLevel = pResources.GetCurrentLevel();

        int newLevel;
        if (int.TryParse(input, out newLevel))
        {
            if (newLevel == 0)
            {
                DefaultOldLevel();
                return;
            }

            pResources.currentLevel = newLevel;
            return;
        }

        DefaultOldLevel();
        return;
    }

    public void AddPlayerAbilityPoints(string input)
    {
        defaultPlayerPoints = pResources.currentPoints;

        int newPoints;
        if (int.TryParse(input, out newPoints))
        {
            if (newPoints == 0)
            {
                DefaultAbilityPoints();
                return;
            }

            pResources.currentPoints = newPoints;
            return;
        }

        DefaultAbilityPoints();
        return;
    }

    public void AddOneLevel()
    {
        pResources.currentExp = pResources.expToLevelUp;
        
        pResources.LevelUp();
    }

    public void DefaultDamage()
    {
        pCombat.baseDamage = 0;
    }

    public void DefaultMovementSpeed()
    {
        pMover.moveSpeed = defaultMoveSpeed;
    }

    public void DefaultHealth()
    {
        pHealth.health = defaultHealth;
        pHealth.maxHealth = defaultMaxHealth;
    }

    public void DefaultOldLevel()
    {
        pResources.currentLevel = oldLevel;
    }

    public void DefaultAbilityPoints()
    {
        pResources.currentPoints = defaultPlayerPoints;
    }
}