using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework.Constraints;
using UnityEngine;
using StolenLands.Abilities;
using UnityEngine.UI;
using StolenLands.Enemy;
using TMPro;
using UnityEngine.AI;

namespace StolenLands.Player
{
    public class PlayerSpellSystem : MonoBehaviour
    {
        #region vars

        public float finalDamage = 0f;
        private float critDamage = 0f;
        
        [SerializeField] private ActionBar actionBar;
        private AbilitySlot[] abilitySlots;
        public bool preCasting;
        public bool isNowCasting = false;
        [Space] 
        public Ability currentAbility = null;
        public Ability lastAbilityUsed = null;
        public bool hasSpellSelected = false;
        [Space] 
        [Header("Runtime Combat Data")]
        public bool nextSpellIsCrit = false;
        [Space]
        [Header("Ability Scripts")]
        public Ability defaultAbility;
        [Space]
        [Header("Ability type")]
        
        public bool isBuff;
        public bool isHeal;
        public bool isDebuff;
        public bool isShield;
        public bool isProjectile;
        public bool isCrowdControl;
        public bool shieldActivated = false;
        
        [Header("Transforms")]
        
        public Transform projectileTransform;
        public Transform AoETransform;
        
        [Header("Reference Prefabs")]
        
        [SerializeField] private GameObject floorMarkerPrefab = null;
        [SerializeField] private FloorSpellMarker floorMarkerManager;
        
        [Space]
        [Header("Cast Bar")]
        
        [SerializeField] private Text currentCastTimeText;
        [SerializeField] private Text castTimeLeftText;
        [SerializeField] private Text currentAbilityText;
        [SerializeField] private Image castingBar;
        [SerializeField] private Image castBarBackground;
        [SerializeField] private Image castBarFrame;
        [Space] 
        [Header("Top Panel")] 
        [SerializeField] private GameObject statusWarningText;
        [SerializeField] private Image[] abilitySkullFrames;

        private bool isAoEPosNull = false;
        private float currentMana;
        private float timeSinceStartCasting;
        
        private float totalCastDuration;
        public float testTime;
	
        [SerializeField] private PlayerUI ui;
        private PlayerCombatController player;
        private PlayerHealthController pHealth;
        private PlayerResources playerR;
        private PlayerInput pInput;
        private PlayerSoundManager soundM;
        private PlayerMover pMover;
        private PlayerControllerV2 pController;
        private CharacterStats stats;
        private Animator animator;

        #endregion

        private void Start()
        {
	        CacheComponents();
            DisableAbilityActivatedFrames();
            HideCastBar();
            
			currentAbility = defaultAbility;
            hasSpellSelected = false;
            abilitySlots = actionBar.GetComponentsInChildren<AbilitySlot>();
        }
        private void Update()
        {
	        currentMana = playerR.GetCurrentMana();
	        
	        if (currentAbility != defaultAbility)
	        {
		        hasSpellSelected = true;		        
	        }
	        else
	        {
		        hasSpellSelected = false;
	        }

	        DisplayCrowdControlFloorMarker();
	        AbilityBehavioursHandler();	
	        BlockMovementWhenCasting();
			GetAbilityTypeBools();
	        if (timeSinceStartCasting >= currentAbility.castDuration) preCasting = false; 

	        timeSinceStartCasting += Time.deltaTime;
	        testTime += Time.deltaTime;
        }

        private void GetAbilityTypeBools()
        {
	        isHeal = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.heal;
	        isBuff = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.buff;
	        isDebuff = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.debuff;
	        isShield = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.shield;
	        isProjectile = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.projectile;
	        isCrowdControl = currentAbility.abilityGlobalType == Ability.AbilityGlobalType.crowdControl;
        }
        private void CacheComponents()
        {
	        pController = GetComponent<PlayerControllerV2>();
	        player = GetComponent<PlayerCombatController>();
	        stats = GetComponent<CharacterStats>();
	        pHealth = GetComponent<PlayerHealthController>();
	        pInput = GetComponent<PlayerInput>();
	        soundM = GetComponent<PlayerSoundManager>();
	        playerR = GetComponent<PlayerResources>();
	        pMover = GetComponent<PlayerMover>(); 
	        animator = GetComponent<Animator>();
        }
        private void HideCastBar()
		{
            castingBar.fillAmount = 0;
            castBarBackground.color = Color.clear;
            castBarFrame.color = Color.clear;
            currentAbilityText.text = null;
            currentCastTimeText.text = null;
            castTimeLeftText.text = null;
        }
		private void DisableAbilityActivatedFrames()
		{
			foreach (var squareFrame in abilitySkullFrames)
			{
				squareFrame.enabled = false;
			}
		}
		private void BlockMovementWhenCasting()
		{
			if (preCasting)
			{
				GetComponent<PlayerCombatController>().CancelAttack();
				pMover.ResetCurrentPath();
				if (Input.GetMouseButtonDown(1))
				{
					CancelCast();
				}
			}

			if (isNowCasting && currentAbility != defaultAbility)
			{
				GetComponent<PlayerCombatController>().CancelAttack();
				pMover.ResetCurrentPath();
			}
		}
		private void DisplayCrowdControlFloorMarker()
		{
            if (floorMarkerManager.GetAoEPosition() == Vector3.zero)
            {
	            isAoEPosNull = true;
	            
            }
            else isAoEPosNull = false;

            if (currentAbility.abilityGlobalType == Ability.AbilityGlobalType.crowdControl)
			{
				floorMarkerPrefab.SetActive(true);
			}
			else
			{
				floorMarkerPrefab.SetActive(false);
			}
		}
		
		#region DamageCalculations

		public void ResetDamageValues()
		{
			finalDamage = 0f;
			critDamage = 0f;
			pInput.lastNumberPressed = 0;
		}

		public void PrepareNextSpell() // ANIM EVENT
		{
			if (GetIfCritical())
			{
				nextSpellIsCrit = true;
				GetAbilityCriticalDamage();
			}
			if (!GetIfCritical())
			{
				nextSpellIsCrit = false;
			}
			
			GetAbilityFinalDamage();
		}
		public void GetAbilityFinalDamage()
		{
			if (currentAbility != null)
			{
				var damage = currentAbility.damage;
				var baseDamage = PlayerCombatController.instance.baseDamage;
				var intellectMultiplier = stats.intellectAmount / 100f;
				var magicDamageMultiplier = stats.magicDmgAmount / 100f;
				var focusMultiplier = stats.focusAmount / 100f;
				
				var damageMultiplied = intellectMultiplier + magicDamageMultiplier + focusMultiplier;
				
				var percentage = (damage * damageMultiplied);
				
				var damageToApply = (damage + percentage);
				
				if (nextSpellIsCrit)
				{
					damageToApply += critDamage;
				}

				var minDamageToApply = (damageToApply - (1.25f * playerR.currentLevel));
				var maxDamageToApply = (damageToApply + (2.25f * playerR.currentLevel));

				minDamageToApply = Mathf.Clamp(minDamageToApply, 1f, Mathf.Infinity);
				maxDamageToApply = Mathf.Clamp(maxDamageToApply, 1f, Mathf.Infinity);
				
				damageToApply = Random.Range(minDamageToApply, maxDamageToApply);
				damageToApply = Mathf.Clamp(damageToApply, 1, Mathf.Infinity);
				damageToApply = (damageToApply * 100f) / 100f;
				
				finalDamage = damageToApply + baseDamage;
			}
		}
		
		public void GetAbilityCriticalDamage() // Only called if nextSpellIsCrit is true
		{                                      // Gets called only in GetAbilityFinalDamage()
			if (currentAbility != null)
			{
				float damageMultiplier;
		        
				float critValue = currentAbility.critValue;
				critValue = Mathf.Clamp(critValue, 0f, critValue);

				float maxCritValue = critValue + playerR.currentLevel;
				critValue = Mathf.Clamp(critValue, 0, maxCritValue);

				damageMultiplier = finalDamage + critValue;
				
				critDamage = damageMultiplier;
			}
		}
		
		public bool GetIfCritical()
		{
			if (currentAbility != null)
			{
				var intellect = stats.intellectAmount / 2 / 100f;
				var agility = stats.agilityAmount / 2 / 100f;
				var dexterity = stats.dexterityAmount / 2 / 100f;
				var baseCritChance = PlayerCombatController.instance.baseCritChance;
				
				agility = Mathf.Clamp(agility, 0,0.9f);
				intellect = Mathf.Clamp(intellect, 0,0.9f);
				dexterity = Mathf.Clamp(dexterity, 0,0.9f);
				
				float critChance = currentAbility.critChance + baseCritChance + intellect + agility + dexterity;
				float rnd = Random.Range(0f, 1f);
				if (critChance > rnd)
				{
					return true;
				}
			}
			return false;
		}
		
        #endregion

       private void AbilityBehavioursHandler()
        {
	        if (!preCasting && !isNowCasting)
	        {
		        switch (pInput.lastNumberPressed)
			        {
				        case 1:
					        if (currentAbility == null || currentAbility != abilitySlots[0].ability)
					        {
						        if (abilitySlots[0].ability == null)
						        {
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[0].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasManaToCast(abilitySlots[0].ability))
						        {
									ui.InstantiateWarning("Not enough mana");
									pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[0].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[0].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[0].ability) && HasManaToCast(abilitySlots[0].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				        
				        case 2:
					        if (currentAbility == null || currentAbility != abilitySlots[1].ability)
					        {
						        if (abilitySlots[1].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[1].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[1].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[1].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[1].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[1].ability) && HasManaToCast(abilitySlots[1].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				        
				        case 3:
					        
					        if (currentAbility == null || currentAbility != abilitySlots[2].ability)
					        {
						        if (abilitySlots[2].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[2].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[2].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[2].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[2].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[2].ability) && HasManaToCast(abilitySlots[2].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				        
				        case 4:
					        
					        
					        if (currentAbility == null || currentAbility != abilitySlots[3].ability)
					        {
						        if (abilitySlots[3].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[3].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[3].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[3].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[3].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[3].ability) && HasManaToCast(abilitySlots[3].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				       
				        case 5:
					        
					        if (currentAbility == null || currentAbility != abilitySlots[4].ability)
					        {
						        if (abilitySlots[4].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[4].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[4].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[4].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[4].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[4].ability) && HasManaToCast(abilitySlots[4].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				        
				        case 6:
					        
					        if (currentAbility == null || currentAbility != abilitySlots[5].ability)
					        {
						        if (abilitySlots[5].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[5].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[5].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[5].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[5].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[5].ability) && HasManaToCast(abilitySlots[5].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
				       
				        case 7:
					       
					        if (currentAbility == null || currentAbility != abilitySlots[6].ability)
					        {
						        if (abilitySlots[6].ability == null)
						        {
							        // warning
							        ui.InstantiateWarning("No ability to select");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (!HasCooldownToCast(abilitySlots[6].ability))
						        {
							        ui.InstantiateWarning("Ability not ready yet");
							        pInput.lastNumberPressed = 0;
							        return;
						        }

						        if (!HasManaToCast(abilitySlots[6].ability))
						        {
							        ui.InstantiateWarning("Not enough mana");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
						        if (abilitySlots[6].ability.abilityGlobalType == Ability.AbilityGlobalType.projectile)
						        {
							        if (!GetIfHasTarget())
							        {
								        ui.InstantiateWarning("No target selected");
								        pInput.lastNumberPressed = 0;
								        return;
							        }
						        }
						        if (animator.GetLayerWeight(3) >= 0.1f)
						        {
							        ui.InstantiateWarning("Can' cast while holding bow");
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        
					        currentAbility = abilitySlots[6].ability;
					        
					        if (currentAbility != defaultAbility)
					        {
						        if(HasCooldownToCast(abilitySlots[6].ability) && HasManaToCast(abilitySlots[6].ability))
						        {
							        Invoke("StartSpellCastingWithDelay", 0.01f);
							        pInput.lastNumberPressed = 0;
							        return;
						        }
					        }
					        break;
			        }
	        }
        }
        private void StartSpellCastingWithDelay() // METHOD GETS CALLED IN ABOVE METHOD AS INVOKE 
        {
	        player.isMelee = false;
	        StartCoroutine(StartSpellCasting());
        }	
        private IEnumerator StartSpellCasting()
        {
	        if (currentAbility == defaultAbility || currentAbility == null) yield break;
	        if(isProjectile)
			{
				if (!GetIfHasTarget() || player.enemyTarget.IsDead())
				{
					ui.InstantiateWarning("No target");
					yield break;
				}
				if (Vector3.Distance(transform.position, player.enemyTarget.transform.position) < 2.5f && GetIfHasTarget())
				{
					ui.InstantiateWarning("Target is too close");
					yield break;
				}
				
				if (Vector3.Distance(transform.position, player.enemyTarget.transform.position) >
				    currentAbility.maxRange && GetIfHasTarget())
				{
					ui.InstantiateWarning("Not enough range");
					yield break;
				}
			}
	        player.isCaster = true;
	        preCasting = true;
	        isNowCasting = true;
	        timeSinceStartCasting = 0f;
	        if (isProjectile)
	        {
		        player.RotateToFaceEnemy(player.enemyTarget.transform.position);
	        }
	        if (currentAbility.castDuration > 0)
            {
	            StartCoroutine(PlayerCastingUIUpdater());
	            yield return new WaitForSeconds(currentAbility.castDuration);
            }
            PlayerCombatController.instance.isCaster = true;
            AbilityAttackBehaviour();
        }

        private IEnumerator PlayerCastingUIUpdater()
        {
            
	        const float projectileTimeOffset = 0.8920855f; // I got this values by counting the time since the start of casting animation until the instantiating
            const float crowdControlTimeOffset = 0.364642f;
            const float healthShieldTimeOffset = 1.353276f;

            if (isProjectile) totalCastDuration = currentAbility.castDuration + projectileTimeOffset;
            if (isCrowdControl) totalCastDuration = currentAbility.castDuration + crowdControlTimeOffset;
            if (isShield || isHeal) totalCastDuration = currentAbility.castDuration + healthShieldTimeOffset;
            if (isBuff || isDebuff) totalCastDuration = currentAbility.castDuration + crowdControlTimeOffset;
            
			GameObject tempStatus = PlayerUI.instance.InstantiateStatusMessage("Casting " + currentAbility.abilityName);
            Destroy(tempStatus, currentAbility.castDuration + 1);
			castBarBackground.color = Color.black;
            castBarFrame.color = Color.white;
            castTimeLeftText.text = totalCastDuration.ToString("#.00");
            currentAbilityText.text = currentAbility.abilityName;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / totalCastDuration;
                castingBar.fillAmount = Mathf.Lerp(0, 1, t);
                currentCastTimeText.text = timeSinceStartCasting.ToString("#.00");
                yield return null;
            }
            if (castingBar.fillAmount == 1f || currentAbility)
            {
                castingBar.fillAmount = 0f;
                castBarBackground.color = Color.clear;
                castBarFrame.color = Color.clear;
                currentAbilityText.text = null;
                currentCastTimeText.text = null;
                castTimeLeftText.text = null;
                yield return new WaitForSeconds(0.5f);
                preCasting = false;
                isNowCasting = false;
                Destroy(tempStatus); 
            }
        }

        public void InstantiateProjectile() // ANIM EVENT
        {
            playerR.ApplyManaCost(currentAbility.GetManaCost());
            currentAbility.InstantiateAbility(projectileTransform.position);
            currentAbility.StartAbilityCooldown();
            AssignLastUsedAbility(currentAbility);
        }

        private void InstantiateAreaAbility() // ANIM EVENT
        {
            Vector3 floorOffset = new Vector3(0f, 0.1f, 0f);

            playerR.ApplyManaCost(currentAbility.GetManaCost());
            currentAbility.InstantiateAbility(transform.position + floorOffset);
            currentAbility.StartAbilityCooldown();
            AssignLastUsedAbility(currentAbility);

            if (isHeal) pHealth.HealPotion(currentAbility.healAmount);
            if (isShield) Debug.Log("is shield");
            if (isBuff) Debug.Log("is buff");
            if (isDebuff) Debug.Log("is debuff");
        }

        private void InstantiateCrowdControl() // ANIM EVENT
		{
            if (!isAoEPosNull)
			{
                floorMarkerPrefab.GetComponentInChildren<SpriteRenderer>().enabled = false;
                playerR.ApplyManaCost(currentAbility.GetManaCost());
                GameObject newSpell = currentAbility.InstantiateAbility(floorMarkerManager.GetAoEPosition());
                if (newSpell.GetComponent<AoE_Behaviour>())
                {
	                newSpell.GetComponent<AoE_Behaviour>().ability = currentAbility;
                }
                currentAbility.StartAbilityCooldown();
                AssignLastUsedAbility(currentAbility);

			}
        }

        public void TurnOffIsCasting() // ANIM EVENT
        {
            isNowCasting = false;
            preCasting = false;
            currentAbility = defaultAbility;
            floorMarkerPrefab.GetComponentInChildren<SpriteRenderer>().enabled = true;
            player.isCaster = false;

        } 

        private void AbilityAttackBehaviour() // ANIM TRIGGERS AND OTHER BEHAVIOURS
        {
			if(currentAbility != defaultAbility)
			{
				GetComponent<PlayerCombatController>().CancelAttack();
				if (isProjectile && GetIfHasTarget())
                {
	                player.RotateToFaceEnemy(player.enemyTarget.transform.position);
                    player.myAnimator.ResetTrigger("stopCast");
                    player.myAnimator.SetTrigger("castProjectile");
                }
				if (isCrowdControl)
                {
                    if (!isAoEPosNull)
                    {
                        player.RotateToFaceEnemy(AoETransform.position);
                        player.myAnimator.ResetTrigger("stopCast");
                        player.myAnimator.SetTrigger("castCrowdControl");
                    }
					else
					{
                        PlayerUI.instance.InstantiateWarning("You can't cast there!");
                        
					}
                }
                if (isHeal || isShield)
                {
	                player.myAnimator.ResetTrigger("stopCast");
                    player.myAnimator.SetTrigger("castArea");
                }

                if (isBuff || isDebuff)
                {
	                player.myAnimator.ResetTrigger("stopCast");
	                player.myAnimator.SetTrigger("castBuff");
                }
            }
        }

        public void CancelCast()
        {
	        player.isCaster = false;
			castBarFrame.color = Color.clear;
			castingBar.fillAmount = 0f;
			castBarBackground.color = Color.clear;
			currentAbilityText.text = null;
			currentCastTimeText.text = null;
			castTimeLeftText.text = null;
			player.myAnimator.SetTrigger("stopCast");
			player.myAnimator.ResetTrigger("stopCast");
			player.myAnimator.ResetTrigger("castProjectile");
			player.myAnimator.ResetTrigger("castArea");
			player.myAnimator.ResetTrigger("castCrowdControl");
			StopAllCoroutines();
			preCasting = false;
			isNowCasting = false;
			nextSpellIsCrit = false;
        }
        private bool HasCooldownToCast(Ability ability)
		{
            return ability.GetIfCanCast();
		}
        private bool HasRangeToCast(Ability ability)
        {
	        if (player.enemyTarget == null) return false;
	        return Vector3.Distance(transform.position,player.enemyTarget.transform.position) < ability.maxRange;
        }
        private bool HasManaToCast(Ability ability)
		{
            return currentMana > ability.manaCost;
        }
		private bool GetIfHasTarget()
        {
            return player.hasTarget;
        }
		private void OnTriggerStay(Collider other)
		{
            if(other.GetComponent<MagicShield>())
			{
				float radius;
				radius = other.GetComponent<MagicShield>().GetComponent<SphereCollider>().radius;
                if (other.GetComponent<MagicShield>().GetComponent<SphereCollider>().isTrigger) shieldActivated = true;
                if (!other.GetComponent<MagicShield>().GetComponent<SphereCollider>().isTrigger ||
                   Vector3.Distance(transform.position, other.transform.position) > radius) shieldActivated = false;
			}
		}

		public void AssignLastUsedAbility(Ability ability)
		{
			lastAbilityUsed = ability;
		}
		public void CleanLastUsedAbility()
		{
			lastAbilityUsed = null;
		}
	}
}

