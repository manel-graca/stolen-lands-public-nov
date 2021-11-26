using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Guirao.UltimateTextDamage;
using HighlightPlus;
using PixelCrushers.DialogueSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace StolenLands.Player
{
    public class PlayerHealthController : MonoBehaviour
    {
        #region Singleton

        public static PlayerHealthController instance;

        private void Awake()
        {
            instance = this;
        }

        #endregion

        [SerializeField] private UltimateTextDamageManager statusTextSpawner;
        public float health;
        public float maxHealth;
        public float baseHealthRegen = 1f;
        public float healthRegenMultiplier = 0.5f;
        public float barFollowUpSpeed = 2f;
        public float timeBetweenHealthPotions;
        public float timeBetweenFoods;
        public bool wasHit = false;
        public float hitAnimationTime;

        private float timeSinceHealthPotion = Mathf.Infinity;
        private float timeSinceFoodEaten = Mathf.Infinity;
        private float timeSinceGotHit = Mathf.Infinity;

        private float timeBetweenHitAnimation = 1.5f;
        float timeBetweenHeals = 1.5f;
        float timeSinceLastHeal = Mathf.Infinity;

        public Image frontHealthBar;
        public Image backHealthBar;


        bool isDead = false;
        bool shieldActive = false;
        float lerpTimer;
        PlayerSoundManager soundM;
        PlayerControllerV2 playerController;
        Animator myAnimator;
        AudioSource audioS;
        PlayerMover playerMover;
        PlayerInput pInput;
        PlayerResources playerR;
        PlayerSpellSystem spellSys;
        PlayerUI ui;
        CharacterStats stats;

        void Start()
        {
            soundM = PlayerSoundManager.instance;
            playerMover = GetComponent<PlayerMover>();
            audioS = GetComponent<AudioSource>();
            playerController = GetComponent<PlayerControllerV2>();
            myAnimator = GetComponent<Animator>();
            pInput = GetComponent<PlayerInput>();
            playerR = GetComponent<PlayerResources>();
            spellSys = GetComponent<PlayerSpellSystem>();
            stats = GetComponent<CharacterStats>();
            ui = PlayerUI.instance;

            health = maxHealth;
        }

        private void Update()
        {
            shieldActive = spellSys.shieldActivated;
            health = Mathf.Clamp(health, 0, maxHealth);
            if (health < maxHealth)
            {
                HealthRegeneration();
            }

            UpdateHealthUI();
            PlayHeartbeatSoundFX();
            timeSinceLastHeal += Time.deltaTime;
            timeSinceFoodEaten += Time.deltaTime;
            timeSinceHealthPotion += Time.deltaTime;
            timeSinceGotHit += Time.deltaTime;
        }

        public bool IsDead()
        {
            return isDead;
        }

        private void PlayHeartbeatSoundFX()
        {
            if (health <= 30f) // heartbeat sfx
            {
                soundM.PlayLowHealthSound();
                if (IsDead())
                    audioS.Stop();
            }
        }

        private void UpdateHealthUI()
        {
            float fillF = frontHealthBar.fillAmount;
            float fillB = backHealthBar.fillAmount;
            float hFraction = health / maxHealth;
            if (fillB > hFraction)
            {
                frontHealthBar.fillAmount = hFraction;
                backHealthBar.color = Color.red;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / barFollowUpSpeed;
                percentComplete = percentComplete * percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
            }

            if (fillF < hFraction)
            {
                backHealthBar.color = Color.green;
                backHealthBar.fillAmount = hFraction;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / barFollowUpSpeed;
                percentComplete = percentComplete * percentComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
            }
        }

        public void GeralHeal(float amount)
        {
            string sHeal = "+" + amount.ToString("0.##");
            lerpTimer = 0f;
            health += amount;
            statusTextSpawner.Add(sHeal, ui.GetRandomSpawnerPos().transform.position, "basehp");
        }

        public void HealthRegeneration()
        {
            var endurance = stats.enduranceAmount / 100f;
            var vitality = stats.vitalityAmount / 100f;
            var stamina = stats.staminaAmount / 100f;

            var e_d_s = endurance + vitality + stamina;
            var percentage = baseHealthRegen * e_d_s;
            var totalRegen = baseHealthRegen + percentage;
            healthRegenMultiplier = totalRegen;
            healthRegenMultiplier = Random.Range(healthRegenMultiplier, (healthRegenMultiplier + totalRegen));

            string sRegen = "+" + healthRegenMultiplier.ToString("0.##");

            if (timeSinceLastHeal >= timeBetweenHeals && health < (maxHealth - baseHealthRegen + healthRegenMultiplier))
            {
                timeSinceLastHeal = 0f;
                lerpTimer = 0f;
                statusTextSpawner.Add(sRegen, ui.GetRandomSpawnerPos().transform.position, "basehp");
                health += baseHealthRegen + healthRegenMultiplier;
                healthRegenMultiplier = 0f;
            }
        }

        public bool HealFood(float amount, float time)
        {
            if (timeSinceFoodEaten > timeBetweenFoods)
            {
                timeSinceFoodEaten = 0f;
                lerpTimer = 0f;
                StartCoroutine(FoodHealRoutine(amount, time));
                return true;
            }
            else return false;
        }

        IEnumerator FoodHealRoutine(float amount, float time)
        {
            string sRegen = "+" + amount.ToString("0.##");
            while (timeSinceFoodEaten < time)
            {
                yield return new WaitForSeconds(1f);
                HealPotion(amount);
                statusTextSpawner.Add(sRegen, ui.GetRandomSpawnerPos().transform.position, "basehp");
            }
        }

        public bool HealPotion(float amount)
        {
            string sHeal = "+" + amount.ToString("0.##");
            if (timeSinceHealthPotion > timeBetweenHealthPotions)
            {
                timeSinceHealthPotion = 0f;
                lerpTimer = 0f;
                health += amount;
                statusTextSpawner.Add(sHeal, ui.GetRandomSpawnerPos().transform.position, "heal");
                return true;
            }
            else return false;
        }

        private float GetFinalDamageResist()
        {
            var armor = stats.armorAmount / 100f;
            var dmgResist = armor * 100f / 10f;
            var resistAmountFloat = dmgResist + (armor / 2) + playerR.currentLevel;
            
            var minValue = dmgResist - resistAmountFloat;
            var maxValue = dmgResist + resistAmountFloat;

            var finalDmgResist = Random.Range(minValue, maxValue);
            
            finalDmgResist = Mathf.Clamp(finalDmgResist, 0.1f, Mathf.Infinity);

            return finalDmgResist;
        }

        public void TakeDamage(float damageAmount, float enemyBaseDamage)
        {
            if (playerController.isGodMode) return;
            if (shieldActive) return;
            if (IsDead()) return;
            if (playerMover.blocking)
            {
                playerMover.StopBlocking();
                playerMover.timeSinceBlockedAttack = 0f;
                return;
            }

            var finalDmgCalc = damageAmount - GetFinalDamageResist() + enemyBaseDamage;
            finalDmgCalc = Mathf.Clamp(finalDmgCalc, 1, Mathf.Infinity);

            string sDamage = "-" + finalDmgCalc.ToString("0.##");

            statusTextSpawner.Add(sDamage, ui.topPlayerTransform, "ondamage");
            lerpTimer = 0f;

            health = health - finalDmgCalc;
            health = Mathf.Clamp(health, 0, maxHealth);

            if (timeSinceGotHit > timeBetweenHitAnimation && !spellSys.isNowCasting || !spellSys.preCasting)
            {
                StartCoroutine(OnHitRoutine());
            }

            if (health == 0 && !IsDead())
            {
                if (IsDead()) return;
                Die();
            }
        }

        public void SetWasHitAnimParamOff()
        {
            myAnimator.SetBool("wasHit", false);
        }

        IEnumerator OnHitRoutine()
        {
            timeSinceGotHit = 0f;
            wasHit = true;
            GetComponent<HighlightEffect>().HitFX(Color.red, 0.2f, 0.2f);
            soundM.PlayHurtSound();
            if (PlayerCombatController.instance.isAttacking || spellSys.preCasting || spellSys.isNowCasting)
            {
                myAnimator.SetBool("wasHit", true);
            }

            yield return new WaitForSeconds(hitAnimationTime);

            wasHit = false;
            myAnimator.SetBool("wasHit", false);
        }

        private void Die()
        {
            isDead = true;
            myAnimator.SetTrigger("dead");
            soundM.PlayDeathSound();
            playerController.enabled = false;
            this.enabled = false;
            playerMover.enabled = false;
            playerR.enabled = false;
            PlayerInteract.instance.enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            Invoke("LoadMainMenu", 3f);
            
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene("Character Setup");
        }

        public void IncreaseMaxHealthLevel(float amount)
        {
            maxHealth += amount;
            health = maxHealth;
        }
    }
}