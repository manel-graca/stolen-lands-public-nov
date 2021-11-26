using System;
using UnityEngine;
using StolenLands.Player;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Guirao.UltimateTextDamage;
using HighlightPlus;
using PixelCrushers.DialogueSystem;
using StolenLands.Abilities;
using Random = UnityEngine.Random;

namespace StolenLands.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        
        public List<Item> enemyItems = new List<Item>();
        public GameObject lootBox;
        [Space] 
        [SerializeField] string enemyName;

        [Space] 
        [Header("Enemy UI")]
        [SerializeField] private UltimateTextDamageManager ultimateTextManager;
        [SerializeField] private Transform damageTextTransform;
        [SerializeField] GameObject selectionDisplay;
        [SerializeField] Image backBar;
        [SerializeField] Image frontBar;
        [SerializeField] Text selectionDisplayNameText;
        [SerializeField] Text selectionDisplayHealthText;
        [SerializeField] Text selectionDisplayLevelText;

        [Space] 
        [Header("Blood")] 
        [Space] 
        [SerializeField] private float bloodWorldTime = 8f;
        [SerializeField] private GameObject[] bloodPrefabs;
        [SerializeField] private Transform[] bloodSpawns;
        [SerializeField] private float maxBloodRotation;

        [Space] 
        [Header("Enemy Stats")] 
        [Space] 
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField] private float health = 10f;
        [SerializeField] private int level = 1;

        [Space] 
        [Header("AudioClips")] 
        [Space]
        public AudioClip deathSound = null;
        public AudioClip attackSound = null;
        public AudioClip hitSound = null;
        public float soundFXVolume = 2f;
        [Space] 
        [Header("Booleans")] 
        [Space] 
        public bool isArcher = false;
        public bool hasHitPlayer = false;
        public bool wasHit = false;
        public bool isStunned = false;
        public bool isSlowed = false;
        public bool isMouseOverEnemy = false;
        public bool isSelected = false;
        [Space] 
        public bool isTrainingDoll = false;
        private float timeSinceTookDamage;
        float xpPerKill;
        float xpPerHit;
        float barFollowUpSpeed = 2f;
        float lerpTimer;
        
        [SerializeField] private GameObject enemyOnDisableDialogue;
        [HideInInspector] public Animator enemyAnimator;

        EnemyAIntelligence enemyAI;
        NavMeshAgent enemyNavAgent;
        PlayerControllerV2 playerTarget;
        PlayerCombatController playerCombat;
        PlayerSpellSystem spellSys;
        AudioSource enemyAudioSource;
        PlayerResources playerR;
        CursorManager cursorManager;
        OptionsManager options;

        void Start()
        {
            enemyAnimator = GetComponent<Animator>();
            enemyNavAgent = GetComponent<NavMeshAgent>();
            enemyAudioSource = GetComponent<AudioSource>();
            enemyAI = GetComponent<EnemyAIntelligence>();

            playerTarget = FindObjectOfType<PlayerControllerV2>();
            playerCombat = FindObjectOfType<PlayerCombatController>();
            spellSys = FindObjectOfType<PlayerSpellSystem>();
            playerR = FindObjectOfType<PlayerResources>();
            xpPerHit = playerR.expPerHit;
            xpPerKill = playerR.expPerKill;

            cursorManager = CursorManager.instance;
            options = FindObjectOfType<OptionsManager>();

            SelectAnimationLayer();
        }

        void Update()
        {
            if (health <= 0)
            {
                return;
            }
            if (isSlowed)
            {
                enemyNavAgent.speed = 1f;
            }
            else
            {
                enemyNavAgent.speed = 3.5f;
            }
            UpdateAnimator();
            GetPlayerExpGainStats();
            level = Mathf.Clamp(playerR.currentLevel - 1, 1, 999);
            timeSinceTookDamage += Time.deltaTime;
        }

        private void ResetAnimTriggersAndBools()
        {
            enemyAnimator.ResetTrigger("attack");
            enemyAnimator.ResetTrigger("stopAttack");
            enemyAnimator.ResetTrigger("dead");
            enemyAnimator.SetBool("isAttacking", false);
        }

        void SelectAnimationLayer()
        {
            if (isArcher)
            {
                enemyAnimator.SetLayerWeight(2, 0f);
                enemyAnimator.SetLayerWeight(1, 1f);
            }
            else
            {
                enemyAnimator.SetLayerWeight(1, 0f);
                enemyAnimator.SetLayerWeight(2, 1f);
            }
        }

        #region MouseEvents

        private void OnMouseDown()
        {
            if (IsDead()) return;
            ShowEnemyTooltip();
        }

        private void OnMouseOver()
        {
            if (IsDead())
            {
                cursorManager.SetDefaultCursor();
                return;
            }

            UpdateEnemyTooltipInfo();
            cursorManager.SetCombatCursor();
            isMouseOverEnemy = true;
        }

        private void OnMouseExit()
        {
            cursorManager.SetDefaultCursor();
            isMouseOverEnemy = false;
        }

        #endregion

        private void GetPlayerExpGainStats()
        {
            xpPerHit = playerR.expPerHit;
            xpPerKill = playerR.expPerKill;
        }

        public void EnemyLevelUp()
        {
            if (IsDead()) return;
            if (health >= maxHealth - 0.5f)
            {
                maxHealth = maxHealth + (level * 7f);
                enemyAI.baseDamage = enemyAI.baseDamage + (level * 1.5f);
                health = maxHealth;
            }
            UpdateEnemyTooltipInfo();
        }

        public void ShowEnemyTooltip()
        {
            if (selectionDisplay == null) return;
            UpdateEnemyTooltipInfo();
            selectionDisplay.SetActive(true);
        }

        public void HideEnemyTooltip()
        {
            selectionDisplay.SetActive(false);
            selectionDisplayHealthText.text = null;
            selectionDisplayLevelText.text = null;
        }

        private void UpdateEnemyTooltipInfo()
        {
            if (selectionDisplay == null) return;
            selectionDisplayNameText.text = enemyName.ToString();
            selectionDisplayLevelText.text = level.ToString();
            selectionDisplayHealthText.text = $"{health}/{maxHealth}";

            if (selectionDisplay.activeSelf)
            {
                float fillF = frontBar.fillAmount;
                float fillB = backBar.fillAmount;
                float hFraction = health / maxHealth;

                if (fillB > hFraction)
                {
                    frontBar.fillAmount = hFraction;
                    backBar.color = Color.red;
                    lerpTimer += Time.deltaTime;
                    float percentComplete = lerpTimer / barFollowUpSpeed;
                    percentComplete = percentComplete * percentComplete;
                    backBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
                }

                if (fillF < hFraction)
                {
                    backBar.color = Color.green;
                    backBar.fillAmount = hFraction;
                    lerpTimer += Time.deltaTime;
                    float percentComplete = lerpTimer / barFollowUpSpeed;
                    percentComplete = percentComplete * percentComplete;
                    frontBar.fillAmount = Mathf.Lerp(fillF, backBar.fillAmount, percentComplete);
                }
            }

            if (health > 0)
            {
                selectionDisplayHealthText.text = health.ToString("#.##") + "/" + maxHealth.ToString();
            }
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = enemyNavAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            enemyAnimator.SetFloat("speed", speed);
        }

        public void EnemyMoveTo(Vector3 wantedDestination)
        {
            if (!isStunned)
            {
                enemyNavAgent.isStopped = false;
                enemyNavAgent.destination = wantedDestination;
            }
        }

        public bool IsDead()
        {
            if (health <= 0)
            {
                return true;
            }
            return false;
        }

        public bool IsEnemyArcher()
        {
            if (isArcher)
            {
                return true;
            }
            else return false;
        }

        private Transform GetRandomBloodPosition()
        {
            int rnd = Random.Range(0, bloodSpawns.Length);
            return bloodSpawns[rnd];
        }

        private GameObject GetRandomBloodPrefab()
        {
            int rnd = Random.Range(0, bloodPrefabs.Length);
            return bloodPrefabs[rnd];
        }

        private void InstantiateBloodEffect()
        {
            if (!options.bloodOn) return;
            
            var bloodToSpawn = GetRandomBloodPrefab();
            var posToSpawn = GetRandomBloodPosition();
            var randomAngle = Random.Range(60f, maxBloodRotation);

            GameObject instance = Instantiate(bloodToSpawn, posToSpawn.position, Quaternion.identity) as GameObject;
            instance.transform.Rotate(0, randomAngle, 0);
            instance.GetComponent<BFX_BloodSettings>().GroundHeight = transform.position.y;
            if (options.bloodQuality == 0)
            {
                instance.GetComponent<BFX_BloodSettings>().DecalRenderinMode =
                    BFX_BloodSettings._DecalRenderinMode.Floor_XZ;
            }
            if (options.bloodQuality == 1)
            {
                instance.GetComponent<BFX_BloodSettings>().DecalRenderinMode =
                    BFX_BloodSettings._DecalRenderinMode.AverageRayBetwenForwardAndFloor;
            }

            Destroy(instance, bloodWorldTime);
        }

        public void TakeDamage(float damageAmount, bool isCrit)
        {
            if (IsDead()) return;
            
            lerpTimer = 0f;
            
            if (!playerCombat.isUnarmed)
            {
                InstantiateBloodEffect();
            }
            if (options.hitEffect)
            {
                GetComponent<HighlightEffect>().HitFX();
            }
            if (!enemyAI.canSeePlayer && !playerCombat.isUnarmed && !enemyAI.isAggro) // when player attacks without being seen. insta kill
            {
                var exceptionalDamage = maxHealth + damageAmount;
                health = 0;
                wasHit = true;
                ultimateTextManager.Add(exceptionalDamage.ToString("0.##"), damageTextTransform, "critical");
                ultimateTextManager.Add("Assassination", damageTextTransform, "critical");
                playerR.AddToExperience(xpPerHit + (damageAmount / 2));
                Die();
                return;
            }
            health -= damageAmount;
            health = Mathf.Clamp(health, 0, maxHealth);
            wasHit = true;
            UpdateEnemyTooltipInfo();
            playerR.AddToExperience(xpPerHit);
            playerCombat.isCaster = false;
            if (isCrit)
            {
                ultimateTextManager.Add(damageAmount.ToString("0.##"), damageTextTransform, "critical");
            }
            else
            {
                ultimateTextManager.Add(damageAmount.ToString("0.##"), damageTextTransform, "normal");
            }
            spellSys.nextSpellIsCrit = false;
            spellSys.ResetDamageValues();
            if (health <= 0)
            {
                Die();
            }
        }

        public void TakeDamageOverTime(float amount, float duration, bool isCrit)
        {
            if (IsDead()) return;
            
            lerpTimer = 0f;
            timeSinceTookDamage = 0f;

            StartCoroutine(TakeDamageOverTimeRoutine(amount, duration));
        }

        IEnumerator TakeDamageOverTimeRoutine(float amount, float duration)
        {
            bool isCrit = false;
            if (playerCombat.isCaster)
            {
                isCrit = spellSys.nextSpellIsCrit;
            }

            if (playerCombat.isMelee)
            {
                isCrit = playerCombat.nextIsCrit;
            }

            while (timeSinceTookDamage < duration)
            {
                TakeDamage(amount, isCrit);
                yield return new WaitForSeconds(1.5f);
            }

            yield break;
        }

        public void StartSlowEffect(float duration)
        {
            if (IsDead()) return;
            StartCoroutine(SlowEffect(duration));
        }

        IEnumerator SlowEffect(float duration)
        {
            isSlowed = true;
            yield return new WaitForSeconds(duration);
            isSlowed = false;
        }

        public void StartStunEffect(float duration)
        {
            if (IsDead()) return;
            StartCoroutine(StunEffect(duration));
        }

        IEnumerator StunEffect(float duration)
        {
            float defaultSpeed = enemyAI.chaseSpeed;
            enemyNavAgent.speed = 0f;
            isStunned = true;
            enemyNavAgent.isStopped = true;
            Debug.Log("should stun");
            yield return new WaitForSeconds(duration);
            isStunned = false;
            enemyNavAgent.speed = defaultSpeed;
            if (!isStunned && !IsDead()) enemyNavAgent.isStopped = false;
        }

        private void Die()
        {
            
            backBar.color = new Color(32f,32f,32f);
            backBar.fillAmount = 0;
            frontBar.color = Color.gray;
            frontBar.fillAmount = 0;
            
            lootBox.gameObject.SetActive(true);
            lootBox.GetComponent<LootBox>().enemyDropping = gameObject;
            
            #region QuestSystem
            if (QuestLog.GetQuestState("Hunting season") == QuestState.Active)
            {
                Destroy(enemyOnDisableDialogue);
            
                int kills = DialogueLua.GetVariable("EnemiesKilledOne").asInt;
                if (kills == 2)
                {
                    QuestLog.SetQuestState("Hunting season", QuestState.ReturnToNPC);
                    DialogueLua.SetVariable("EnemiesKilledOne", (int)3);
                }
            }
            
            #endregion
            
            
            ResetAnimTriggersAndBools();
            enemyAnimator.SetTrigger("dead");
            enemyAnimator.SetBool("isDead", true);
            
            playerR.AddToExperience(xpPerKill);
            
            StopAllCoroutines();
            
            selectionDisplayHealthText.text = "Dead";
           
            
            
            if (deathSound != null)
            {
                enemyAudioSource.PlayOneShot(deathSound);
            }

            Cancel();
        }

        public void CheatDie()
        {
            backBar.color = new Color(32f,32f,32f);
            backBar.fillAmount = 0;
            frontBar.color = Color.gray;
            frontBar.fillAmount = 0;
            
            lootBox.gameObject.SetActive(true);
            lootBox.GetComponent<LootBox>().enemyDropping = gameObject;
            
            ResetAnimTriggersAndBools();
            enemyAnimator.SetTrigger("dead");
            enemyAnimator.SetBool("isDead", true);
            
            StopAllCoroutines();
            
            selectionDisplayHealthText.text = "Dead";
            Cancel();
        }


        public void Cancel()
        {
            if (playerTarget.GetComponent<PlayerCombatController>().enemyTarget == this)
            {
                playerTarget.GetComponent<PlayerCombatController>().enemyTarget = null;
            }

            GetComponent<Target>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<EnemyAIntelligence>().targetPlayer = null;
            GetComponent<NavMeshAgent>().enabled = false;
            enabled = false;
        }
    }
}