using System.Collections.Generic;
using StolenLands.Abilities;
using UnityEngine;
using StolenLands.Enemy;
using StolenLands.Projectiles;

namespace StolenLands.Player
{
    public class PlayerCombatController : MonoBehaviour
    {
        #region
        public static PlayerCombatController instance;
		private void Awake()
		{
            instance = this;
		}
        #endregion
        public bool inCombat = false;

        [Space] 
        [Header("Player Combat Stats")]
        [Space] 
        [Range(0,1)] public float baseCritChance;
        [Tooltip("For debug purposes, should not be changed")] public int baseDamage;
        public float timeBetweenAttacks = 2f;

        [Range(0, 1)] public float rotationSpeed;

        [Header("Weapons")]
        public Weap currentWeapon = null;
        [SerializeField] Weap defaultWeapon = null;
        [Header("Shields")]
        [Space]
        public GameObject shieldPrefab = null;
        public bool hasShield = false;
        public bool hasWithdrawnSword = false;
        public bool canPickupWeapon = true;
        [Space]
        
        [Header("Transforms")]

        public Transform RighthandTransform;
        public Transform LefthandTransform;
        [SerializeField] Transform arrowLaunchPosition = null;

        // CLASS VARIABLES
        public float timeSinceLastAttack = Mathf.Infinity;
        public bool isAttacking = false;
        public bool hasTarget = false;

        float timeSinceMouseOverEnemy = Mathf.Infinity;

        public GameObject enemyDisplay;
        public EnemyController enemyTarget;
        
        [Space]
        
        [Header("During Gameplay")]
        
        public bool nextIsCrit = false;
        public bool nextAttackEffect = false;
        [Space]

        public bool isUnarmed = true;
        public bool isMelee = false;
        public bool isArcher = false;
        public bool isCaster = false;

        private float timeSinceIsCriticalCalled = Mathf.Infinity;
        private float timeBetweenIsCriticalCheck = 0.2f;

        public Animator myAnimator;
        // CACHED REFERENCES

        PlayerControllerV2 playerController;
        PlayerMover playerMover;
        Projectile arrow;
        PlayerSpellSystem spellSys;
        PlayerResources playerR;
        CharacterStats stats;
        PlayerSoundManager soundM;
        GameManager manager;
        
        void Start()
        {
	        manager = GameManager.instance;
	        playerMover = GetComponent<PlayerMover>();
            spellSys = GetComponent<PlayerSpellSystem>();
            playerR = GetComponent<PlayerResources>();
            playerController = GetComponent<PlayerControllerV2>();
            stats = GetComponent<CharacterStats>();
            soundM = PlayerSoundManager.instance;
            
            EquipWeapon(defaultWeapon);
            if (defaultWeapon.name == "Unarmed") isUnarmed = true;
        }

        void Update()
        {
	        AnimationLayersHandler();

	        if (currentWeapon == defaultWeapon)
	        {
		        isUnarmed = true;
	        }
	        
	        if (playerMover.blocking)
	        {
		        Transform nearest = FindClosestEnemy(); // DEBUG ONLY!!!
		        if (Vector3.Distance(transform.position, nearest.transform.position) > 20f) return;

		        RotateToFaceEnemy(nearest.position);
	        }
	        if(enemyTarget == null)
	        {
		        myAnimator.SetTrigger("stopAttack");
		        inCombat = false;
		        isAttacking = false;
		        hasTarget = false;
		        return;
	        }
	        else
	        {
		        hasTarget = true;
	        }
			manager.inCombat = inCombat;
	        if (enemyTarget.IsDead())
	        {
		        myAnimator.SetTrigger("stopAttack");
		        hasTarget = false;
		        return;
	        }
	        if (CheckIfInRangeToAttack())
	        {
		        AttackBehaviour();
	        }
	        timeSinceLastAttack += Time.deltaTime;
	        timeSinceMouseOverEnemy += Time.deltaTime;
	        timeSinceIsCriticalCalled += Time.deltaTime;
        }

		private void AnimationLayersHandler() // ALSO CONTROLS ANIM LAYERS
		{
            if (myAnimator.GetLayerName(1) == "Unarmed") // layer order check
            {
				if (isCaster)
				{
                    myAnimator.SetLayerWeight(4, 1f);

                    myAnimator.SetLayerWeight(1, 0f);
                    myAnimator.SetLayerWeight(2, 0f); 
                    myAnimator.SetLayerWeight(3, 0f);
				}
				if (isArcher)
				{
                    myAnimator.SetLayerWeight(3, 1f);

                    myAnimator.SetLayerWeight(1, 0f);
                    myAnimator.SetLayerWeight(2, 0f);
                    myAnimator.SetLayerWeight(4, 0f);
                }
                if (isMelee)
				{
					myAnimator.SetLayerWeight(2, 1f);

					myAnimator.SetLayerWeight(1, 0f);
                    myAnimator.SetLayerWeight(3, 0f);
                    myAnimator.SetLayerWeight(4, 0f);
                }
                if (isUnarmed && spellSys.currentAbility == spellSys.defaultAbility)
                {
                    myAnimator.SetLayerWeight(1, 1f);

                    myAnimator.SetLayerWeight(2, 0f);
                    myAnimator.SetLayerWeight(3, 0f);
                    myAnimator.SetLayerWeight(4, 0f);
                }
                else return;
            }
		}

		public void SetEnemyTarget(GameObject target)
        {
            enemyTarget = target.GetComponent<EnemyController>();
        }
		public void AttackBehaviour()
		{
			if (enemyTarget == null) return;
			isAttacking = true;
			if (GetIfAlreadyCanAttack())
			{
				TriggerAttack();
				timeSinceLastAttack = 0;
			}
			else
			{
				isAttacking = false;
				return;
			}
		}
		public bool GetIfAlreadyCanAttack()
		{
			return timeSinceLastAttack > currentWeapon.attackSpeed;
		}

		public void RotateToFaceEnemy(Vector3 target)
        {
            if(enemyTarget != null)
			{
                var targetRotation = Quaternion.LookRotation(target - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
            }
        }
        private void TriggerAttack()
        {
	        if (isCaster || spellSys.isNowCasting || spellSys.preCasting) return;
	        if (isUnarmed || isMelee || isArcher)
	        {
		        
		        playerMover.ResetCurrentPath();
		        myAnimator.ResetTrigger("stopAttack");
		        myAnimator.SetTrigger("attack");
		        RotateToFaceEnemy(enemyTarget.transform.position);
	        }
        }

        public void SetNextAttackIsTrigger() // is next attack crit or not
        {
	        if (GetIfCriticalDamage())
	        {
		        nextIsCrit = true;
		        return;
	        }
	        else nextIsCrit = false;
        }

        public bool GetIfCriticalDamage() 
        {
	        if (timeSinceIsCriticalCalled > timeBetweenIsCriticalCheck)
	        {
		        timeSinceIsCriticalCalled = 0f;
		        
		        var focus = stats.focusAmount / 2 / 100f;
		        var agility = stats.agilityAmount / 2 / 100f;
		        agility = Mathf.Clamp(agility, 0,0.9f);
		        focus = Mathf.Clamp(focus, 0,0.9f);
		        
		        float rnd = Random.Range(0f, 1f);
		        
		        var critChance = currentWeapon.criticalChance + agility + focus + baseCritChance;
		        
		        critChance = Mathf.Clamp(critChance, 0, 1);
		        rnd = (rnd * 100) / 100f;
		        if (critChance > rnd)
		        {
			        Debug.Log( "Critical in combat");
			        return true;
		        }
		        Debug.Log("No critical hit!");
		        return false;
	        }
	        return false;
        }

        public float GetWeaponCriticalDamage() // Only called if nextAttackIsCrit is true. Gets called ONLY in GetWeaponFinalDamage()
        {
	        if (currentWeapon != null)
	        {
		        float critValue = currentWeapon.critValue;
		        float critMultiplier = (playerR.currentLevel + 1) / 2;
		        
		        critValue = Random.Range(critValue - critMultiplier, critValue + critMultiplier);
		        critValue = Mathf.Clamp(critValue, 0, Mathf.Infinity);
			        
		        return critValue;
	        }
	        return 0f;
        }

        public float GetWeaponFinalDamage() // sums up all the dmg calculations into one final float
        {
	        var strengthMultiplier = stats.strengthAmount / 100;
	        var dexterityMultiplier = stats.dexterityAmount / 100;
	        var damageMultiplied = strengthMultiplier + dexterityMultiplier;
	        var percentage = (currentWeapon.damage * damageMultiplied);
	        var damageToApply = (currentWeapon.damage + percentage);

	        if (nextIsCrit)
	        {
		        damageToApply = damageToApply + GetWeaponCriticalDamage() + playerR.currentLevel + baseDamage;
	        }
	        
	        damageToApply = Random.Range((damageToApply - 0.5f - playerR.currentLevel), (damageToApply + (3.5f * playerR.currentLevel)));
	        damageToApply = (damageToApply * 100f) / 100f;
	        damageToApply = Mathf.Clamp(damageToApply, 1, Mathf.Infinity);

	        return damageToApply;
        }

        public void ProcessHit() /// ANIMATION EVENT \\\
        {
            Debug.Log("ola");
            if (enemyTarget == null) return;
            Debug.Log("tem target");
            float finalDamage = GetWeaponFinalDamage();

            if (currentWeapon.shootsProjectiles)
            {
                //arrow.target = enemyTarget;
                currentWeapon.ShootArrow(arrowLaunchPosition, enemyTarget);
                nextIsCrit = false;
                isAttacking = false;
            }
            else
            {
                enemyTarget.TakeDamage(finalDamage, nextIsCrit);
                nextIsCrit = false;
                isAttacking = false;
            }
        } 

        private bool CheckIfInRangeToAttack()
        {
            return Vector3.Distance(transform.position, enemyTarget.transform.position) < currentWeapon.range;
        }
        public void CancelAttack()
        {
            myAnimator.SetTrigger("stopAttack");
            isAttacking = false;
        }

        public void SetTargetToNull()
		{
            enemyTarget = null;
        }

        public void UnEquipWeapon(Weap weapon)
		{
			isUnarmed = true;

			if(weapon.weaponType == WeaponType.Sword)
			{
                myAnimator.SetTrigger("swordSheat");
                GameObject weap = RighthandTransform.GetChild(0).gameObject;
                if (weap.CompareTag("PlayerWeapon"))
                {
	                isMelee = false;
	                isArcher = false;
	                Destroy(weap, 1f);
                    currentWeapon = defaultWeapon;
                    canPickupWeapon = true;
                    soundM.PlaySwordEquipSound(false);
                    return;
                }
            }
            if (weapon.weaponType == WeaponType.Bow)
            {
                myAnimator.SetTrigger("bowSheat");
                GameObject weap = LefthandTransform.GetChild(0).gameObject;
                if (weap.CompareTag("PlayerWeapon"))
                {
	                isMelee = false;
	                isArcher = false;
	                Destroy(weap, 1f);
                    currentWeapon = defaultWeapon;
                    canPickupWeapon = true;
                    return;
                }
            }
		}

        public void EquipWeapon(Weap weapon)
        {
	        if (weapon.weaponType == WeaponType.Sword)
	        {
		        isMelee = true;
		        isUnarmed = false;
		        isArcher = false;
		        myAnimator.SetTrigger("swordWithdraw");
		        weapon.EquipWeapon(RighthandTransform, LefthandTransform, myAnimator);
		        currentWeapon = weapon;
		        soundM.PlaySwordEquipSound(true);
	        }

	        if (weapon.weaponType == WeaponType.Bow)
	        {
		        if (hasShield)
		        {
			        PlayerUI.instance.InstantiateWarning("Can't equip " + weapon.itemName + " while holding shield");
			        return;
		        }
		        
		        isArcher = true;
		        isUnarmed = false;
		        isMelee = false;
		        myAnimator.SetTrigger("bowWithdraw");
		        weapon.EquipWeapon(RighthandTransform, LefthandTransform, myAnimator);
		        currentWeapon = weapon;
	        }

	        if (weapon.name == "Unarmed")
	        {
		        currentWeapon = defaultWeapon;
		        isUnarmed = true;
	        }
        }

        public void EquipShield()
        {
	        hasShield = true;
	        shieldPrefab.SetActive(true);
        }

        public void UnEquipShield()
        {
	        hasShield = false;
	        shieldPrefab.SetActive(false);
        }

        public void PlayBowDraw()
        {
	        soundM.BowDrawSound();
        }

        public void PlayBowRelease()
        {
	        soundM.PlayArrowReleaseSound();
        }

        private Transform FindClosestEnemy()
        {
            float distanceToClosestEnemy = Mathf.Infinity;
            EnemyAIntelligence closestEnemy = null;
            EnemyAIntelligence[] allEnemies = GameObject.FindObjectsOfType<EnemyAIntelligence>();

            foreach (EnemyAIntelligence currentEnemy in allEnemies)
            {
                float distanceToEnemy = (currentEnemy.transform.position - transform.position).sqrMagnitude;
                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                }
            }
            return closestEnemy.transform;
        }

        public EnemyController GetEnemyTarget()
		{
            return enemyTarget;
		}
    }
}

