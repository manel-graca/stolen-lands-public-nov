using System.Collections;
using System.Collections.Generic;
using Guirao.UltimateTextDamage;
using UnityEngine;
using StolenLands.Abilities;
using StolenLands.Enemy;
using UnityEngine.AI;
using UnityEngine.UI;

namespace StolenLands.Player
{
    public class PlayerResources : MonoBehaviour
    {
        #region singleton

        public static PlayerResources instance;

        private void Awake()
        {
            instance = this;
        }

        #endregion

        #region vars

        [Header("Mana")]
        [Space] 
        [SerializeField] float currentMana;
        

        [SerializeField] float maxMana;
        [Space] 
        [SerializeField] float baseManaRegenAmount;
        [SerializeField] float timeBetweenManaRegen;
        [SerializeField] float timeBetweenManaPotions;
        [SerializeField] float manaToAddPerLevel;
        [Space] 

        [Header("Stamina")]
        [Space] 

        public float currentStamina;
        public float maxStamina;
        [Range(0.00f,5.00f)] public float staminaBaseRegen;
        [Range(0.00f,5.00f)] public float staminaRegenMultiplier;
        [Range(0.00f,5.00f)] public float staminaDrainMultiplier;
        public float timeToStartStaminaRegen;
        public Image staminaFillImage;

        [Space] 

        [Header("Experience")] 
        [Space]
        public float currentExp;
        public int currentLevel = 1;
        public float bruteExperiencePoints;
        [SerializeField] float expGainMultiplier;
        [Space] 
        public float expToLevelUp;
        [SerializeField] float eachLevelXpRequirementMultiplier;
        [SerializeField] float singleExpGainLimit;
        [Space]
        public float healthMultiplierPerLevel;
        [Space]
        public GameObject levelUpParticles;
        public float expPerKill = 0;
        public float expPerHit = 0;
        [SerializeField] private Transform topXpSpawnerText;

        [Space] [Header("Ability Points")] [Space]
        public int currentPoints = 0;

        public int pointsPerLevel = 3;

        [Space] 
        [Header("UI References")] 
        [Space] 
        [SerializeField] Text t_attackDamage;
        [SerializeField] Text t_abilityPower;
        [SerializeField] Text t_manaRegen;
        [SerializeField] Text t_moveSpeed;
        [SerializeField] Text t_health;
        [SerializeField] Text t_healthRegen;
        [SerializeField] Text t_attackSpeed;
        [Space] 
        [SerializeField] Text topLevelDisplayText;
        [SerializeField] Text topExperienceDisplayText;
        [SerializeField] Text frontExpBarPercentageText;
        [Space] 
        public Image frontXpBar;
        public Image backXpBar;
        public Image frontManaBar;
        public Image backManaBar;
        [Space] 
        public float barFollowUpSpeed = 2f;
        public float xpBarFollowUpSpeed;
        public float timeUntilxpBarFollows;
        [Space] 
        public UltimateTextDamageManager textDamageManager;

        private float timeSinceUsedPotion = Mathf.Infinity;
        private float timeSinceLastManaRegen = Mathf.Infinity;

        private float lerpTimer;
        private float delayTimer;
        
        private float timeSinceStopSprint = Mathf.Infinity;
        

        PlayerSpellSystem spellSys;
        PlayerCombatController pCombat;
        PlayerHealthController pHealth;
        PlayerMover pMover;
        PlayerSoundManager soundM;
        EnemyController[] enemies;

        #endregion

        void Start()
        {
            pCombat = GetComponent<PlayerCombatController>();
            pHealth = GetComponent<PlayerHealthController>();
            pMover = GetComponent<PlayerMover>();
            spellSys = GetComponent<PlayerSpellSystem>();
            soundM = FindObjectOfType<PlayerSoundManager>();
            enemies = FindObjectsOfType<EnemyController>();

            currentMana = maxMana;
            frontManaBar.fillAmount = currentMana / maxMana;
            backManaBar.fillAmount = currentMana / maxMana;

            frontXpBar.fillAmount = currentExp / expToLevelUp;
            backXpBar.fillAmount = currentExp / expToLevelUp;
        }

        void Update()
        {
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);

            if (currentExp >= expToLevelUp)
            {
                LevelUp();
            }

            DrainStamina();
            StaminaBaseRegen();
            UpdateCharacterStatsUI();
            BaseManaRegeneration();
            UpdateManaUI();
            UpdateExperienceBar();
            
            staminaFillImage.fillAmount = currentStamina / maxStamina;

            timeSinceLastManaRegen += Time.deltaTime;
            timeSinceUsedPotion += Time.deltaTime;
            timeSinceStopSprint += Time.deltaTime;
        }

        private void DrainStamina()
        {
            if (Input.GetKey(KeyCode.LeftShift) && pMover.sprinting && GetComponent<NavMeshAgent>().hasPath)
            {
                timeSinceStopSprint = 0f;
                currentStamina -= (Time.deltaTime * staminaDrainMultiplier);
            }
        }

        private void StaminaBaseRegen()
        {
            var totalRegen = staminaBaseRegen + staminaRegenMultiplier;
            if (timeSinceStopSprint > timeToStartStaminaRegen && !pMover.sprinting && currentStamina <= maxStamina)
            {
                currentStamina += Time.deltaTime * totalRegen;
            }
        }

        #region Mana

        public float GetCurrentMana()
        {
            return currentMana;
        }

        public float GetMaxMana()
        {
            return maxMana;
        }

        public bool AddManaFromPotion(float amount)
        {
            if (timeSinceUsedPotion > timeBetweenManaPotions)
            {
                lerpTimer = 0f;
                timeSinceUsedPotion = 0f;
                currentMana += amount;
                return true;
            }
            else return false;
        }

        public void BaseManaRegeneration()
        {
            if (timeSinceLastManaRegen > timeBetweenManaRegen)
            {
                if (currentMana < maxMana)
                {
                    lerpTimer = 0f;
                    timeSinceLastManaRegen = 0f;
                    currentMana += baseManaRegenAmount;
                }
            }
        }

        private void AddToMaxManaOnLevelUp()
        {
            maxMana += manaToAddPerLevel;
            currentMana = maxMana;
        }

        public void ApplyManaCost(float manaCost)
        {
            lerpTimer = 0f;
            currentMana = currentMana - manaCost;
        }

        private void UpdateManaUI()
        {
            float fillF = frontManaBar.fillAmount;
            float fillB = backManaBar.fillAmount;
            float hFraction = currentMana / maxMana;
            if (fillB > hFraction)
            {
                frontManaBar.fillAmount = hFraction;
                backManaBar.color = Color.blue;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / barFollowUpSpeed;
                backManaBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
            }

            if (fillF < hFraction)
            {
                backManaBar.color = Color.cyan;
                backManaBar.fillAmount = hFraction;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / barFollowUpSpeed;
                percentComplete = percentComplete * percentComplete;
                frontManaBar.fillAmount = Mathf.Lerp(fillF, backManaBar.fillAmount, percentComplete);
            }
        }

        #endregion

        #region Experience

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void AddToExperience(float expAmount)
        {
            lerpTimer = 0f;
            delayTimer = 0f;
            var total = expAmount + expGainMultiplier;
            currentExp += total;
            textDamageManager.Add("+" + total.ToString("0.##") + " XP", topXpSpawnerText, "xp");
        }

        public void LevelUp()
        {
            foreach (var enemy in enemies)
            {
                enemy.EnemyLevelUp();
            }
            
            AbilityTreeSlot[] slots = FindObjectsOfType<AbilityTreeSlot>();

            UpdateStatsWhenLevelUp();

            frontXpBar.fillAmount = 0f;
            backXpBar.fillAmount = 0f;

            //currentExp = currentExp - expToLevelUp;

            currentPoints += pointsPerLevel;

            expToLevelUp += eachLevelXpRequirementMultiplier;

            currentLevel += 1;

            pHealth.IncreaseMaxHealthLevel(healthMultiplierPerLevel);

            maxMana += manaToAddPerLevel;

            currentMana = maxMana;

            soundM.PlayInterfaceSound(soundM.levelUpSound);
            GameObject ps = Instantiate(levelUpParticles, transform.position, Quaternion.identity);
            ps.transform.SetParent(transform);
            PlayerUI.instance.InstantiateWarning("<color=#00d5ff>Level up</color>");
        }

        private void UpdateStatsWhenLevelUp() // TO COMPLETE!
        {
            switch (currentLevel)
            {
                case 11:
                    pCombat.baseDamage = 68;
                    eachLevelXpRequirementMultiplier = 1200f;
                    healthMultiplierPerLevel = 169f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 10:
                    pCombat.baseDamage = 44;
                    eachLevelXpRequirementMultiplier = 1000f;
                    healthMultiplierPerLevel = 150f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 9:
                    pCombat.baseDamage = 40;
                    eachLevelXpRequirementMultiplier = 800f;
                    healthMultiplierPerLevel = 139f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 8:
                    pCombat.baseDamage = 35;
                    eachLevelXpRequirementMultiplier = 600f;
                    healthMultiplierPerLevel = 120f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 7:
                    pCombat.baseDamage = 27;
                    eachLevelXpRequirementMultiplier = 400f;
                    healthMultiplierPerLevel = 105f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 6:
                    pCombat.baseDamage = 21;
                    eachLevelXpRequirementMultiplier = 355f;
                    healthMultiplierPerLevel = 87f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 5:
                    pCombat.baseDamage = 11;
                    eachLevelXpRequirementMultiplier = 295f;
                    healthMultiplierPerLevel = 74f;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 66f;
                    expGainMultiplier = 8f;
                    expPerHit = Random.Range(0.3f, 0.6f);
                    expPerKill = Random.Range(52.56f, 74.23f);
                    break;

                case 4:
                    pCombat.baseDamage = 9;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 41f;
                    eachLevelXpRequirementMultiplier = 220f;
                    healthMultiplierPerLevel = 66f;
                    expGainMultiplier = 6.88f;
                    expPerHit = Random.Range(0.25f, 0.7f);
                    expPerKill = Random.Range(42f, 58f);
                    break;

                case 3:
                    pCombat.baseDamage = 6;
                    pointsPerLevel = 3;
                    manaToAddPerLevel = 30f;
                    eachLevelXpRequirementMultiplier = 150f;
                    healthMultiplierPerLevel = 55f;
                    expGainMultiplier = 4.21f;
                    expPerHit = Random.Range(0.25f, 0.6f);
                    expPerKill = Random.Range(30f, 44f);
                    break;

                case 2:
                    pCombat.baseDamage = 4;
                    pointsPerLevel = 2;
                    manaToAddPerLevel = 24f;
                    eachLevelXpRequirementMultiplier = 105f;
                    healthMultiplierPerLevel = 39f;
                    expGainMultiplier = 2.5f;
                    expPerHit = Random.Range(0.2f, 0.5f);
                    expPerKill = Random.Range(27f, 38f);
                    break;

                case 1:
                    pCombat.baseDamage = 0;
                    pointsPerLevel = 2;
                    manaToAddPerLevel = 13f;
                    eachLevelXpRequirementMultiplier = 55f;
                    healthMultiplierPerLevel = 25f;
                    expGainMultiplier = 0.5f;
                    expPerHit = Random.Range(0.1f, 0.5f);
                    expPerKill = Random.Range(20f, 33f);
                    break;
            }
        }

        void UpdateCharacterStatsUI()
        {
            topLevelDisplayText.text = currentLevel.ToString();
            t_attackDamage.text = pCombat.currentWeapon.damage.ToString();
            t_abilityPower.text = spellSys.currentAbility.damage.ToString();
            t_manaRegen.text = baseManaRegenAmount.ToString();
            t_moveSpeed.text = pMover.moveSpeed.ToString();
            t_health.text = pHealth.health.ToString();
            t_healthRegen.text = (pHealth.baseHealthRegen + pHealth.healthRegenMultiplier).ToString();
            t_attackSpeed.text = pCombat.timeBetweenAttacks.ToString();
        }

        void UpdateExperienceBar()
        {
            float xPFraction = currentExp / expToLevelUp;
            float frontXP = frontXpBar.fillAmount;
            if (frontXP < xPFraction)
            {
                delayTimer += Time.deltaTime;
                backXpBar.fillAmount = xPFraction;
                if (delayTimer >= timeUntilxpBarFollows)
                {
                    lerpTimer += Time.deltaTime;
                    float percentComplete = lerpTimer / xpBarFollowUpSpeed;
                    percentComplete = percentComplete * percentComplete;
                    frontXpBar.fillAmount = Mathf.Lerp(frontXP, backXpBar.fillAmount, percentComplete);
                }
            }

            topExperienceDisplayText.text = currentExp.ToString("0.##") + " / " + expToLevelUp + " XP";
            frontExpBarPercentageText.text = currentExp.ToString("0.##") + " / " + expToLevelUp + " XP";
        }

        #endregion
    }
}