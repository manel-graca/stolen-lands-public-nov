using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StolenLands.Enemy;

namespace StolenLands.Player
{
    public class PlayerMover : MonoBehaviour
    {
        public static PlayerMover instance;
		private void Awake()
		{
            instance = this;
		}

		[Header("Player Core Stats")]
        [Range(1, 20)]
        public float moveSpeed;
        [Range(1, 6)]
        public float walkSpeed;
        [Range(1, 6)]
        public float crouchSpeed;
        [Range(1, 30)]
        public float sprintSpeed;


        public bool sprinting = false;
        public bool walking = false;
        public bool crouching = false;
        public bool blocking = false;
        public float minTimeBetweenBlockAttack = 3f;

        [Space] 
        public bool isHiding;
        public HideZone hideZone;
        public GameObject hiddenTextPrefab;

        [SerializeField] float timeBetweenPotions;
        
        float timeSincePotionUsed = Mathf.Infinity;
        [HideInInspector] public float timeSinceBlockedAttack = Mathf.Infinity;
        
        public bool potionActive = false;

        static float lastSpeed = 3.75f;

        // CACHED REFERENCES
        public NavMeshAgent navAgent;
        Animator myAnimator;
        PlayerInput pInput;
        PlayerCombatController combatController;
        EnemyAIntelligence enemyAI;
        EnemyController target;
        PlayerSpellSystem spellSys;
        
        void Start()
        {
	        hiddenTextPrefab.SetActive(false);
            pInput = GetComponent<PlayerInput>();
            combatController = GetComponent<PlayerCombatController>();
            spellSys = GetComponent<PlayerSpellSystem>();
            navAgent = GetComponent<NavMeshAgent>();
            myAnimator = GetComponent<Animator>();
            navAgent.speed = moveSpeed;
        }

        void Update()
		{
            if(!walking || !crouching || !potionActive)
			{
                navAgent.speed = moveSpeed;
			}

            if (isHiding)
            {
	            hiddenTextPrefab.SetActive(true);
            }
            else
            {
	            hiddenTextPrefab.SetActive(false);
            }

            HandleCrouch();
            HandleBlock();
            HandleMoveVariance();
            HoldSprint();
            UpdateAnimator();
            HandleSpeedChange();
            target = combatController.enemyTarget;

            timeSincePotionUsed += Time.deltaTime;
            timeSinceBlockedAttack += Time.deltaTime;
		}
        
        public float GetSpeed()
		{
            return navAgent.speed;
		}

        public bool IsPlayerCrouching()
        {
	        if (crouching)
	        {
		        return true;
	        }
	        return false;
        }

        public HideZone GetHidingZone()
        {
	        if (hideZone == null)
	        {
		        return null;
	        }
	        return hideZone;
        }

        public void EnterHideZone(HideZone zone)
        {
	        isHiding = true;
	        hideZone = zone;
        }

        public void ExitHideZone()
        {
	        isHiding = false;
	        hideZone = null;
        }

        private void HandleSpeedChange() // direct change of agent speed
		{
            if(walking)
			{
                navAgent.speed = walkSpeed;
			}
            if(crouching)
			{
                navAgent.speed = crouchSpeed;
			}
            if(sprinting && !crouching)
			{
                navAgent.speed = sprintSpeed;
			}
            if(blocking)
			{
                navAgent.speed = 0;
			}
		}
        private void HandleMoveVariance() // handles generic movement booleans, feeds into HandleSpeedChanger()
		{
            if (pInput.walking)
            {
                walking = true;
            }
            if (!pInput.walking)
			{
                walking = false;
            }
        }
        private void HandleBlock()
		{
			if (!combatController.hasShield || timeSinceBlockedAttack < minTimeBetweenBlockAttack) return;

			if (pInput.blockAttack)
			{
				blocking = true;
                navAgent.isStopped = true;
                myAnimator.SetBool("IsBlocking", true);
            }
            if (!pInput.blockAttack || PlayerHealthController.instance.wasHit)
            {
                StopBlocking();
            }
        }

        public void StopBlocking()
        {
	        blocking = false;
	        navAgent.isStopped = false;
	        myAnimator.SetBool("IsBlocking", false);
        }
        public void HandleCrouch()
		{
            if(pInput.crouching)
			{
                crouching = true;
                myAnimator.SetBool("IsCrouched", true);
			}
            if(!pInput.crouching)
			{
                crouching = false;
                myAnimator.SetBool("IsCrouched", false);
			}
		}
        private void HoldSprint()
		{
            if(pInput.sprinting)
			{
                sprinting = true;
			}
            if(!pInput.sprinting)
			{
                sprinting = false;
			}
		}
        public void MoveTo(Vector3 wantedDestination)
        {
	        combatController.CancelAttack();
	        spellSys.CancelCast();
	        
            navAgent.isStopped = false;
            navAgent.destination = wantedDestination;
            PlayerCombatController.instance.isAttacking = false;
            PlayerInteract.instance.isGathering = false;
            if (transform.position == wantedDestination)
            {
                transform.Rotate(0, 0, 0);
            }
        }

        public void ResetCurrentPath()
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            myAnimator.SetFloat("speed", speed);
        }

        public bool AddSpeedPotion(float amount, float duration)
		{
            if(!potionActive && timeSincePotionUsed > timeBetweenPotions)
			{
                timeSincePotionUsed = 0f;
                StartCoroutine(AddMoveSpeedPotion(amount, duration));
                return true;
            }
            return false;
		}
        IEnumerator AddMoveSpeedPotion(float amount, float duration)
		{
            potionActive = true;
            moveSpeed += amount;
            yield return new WaitForSeconds(duration);
            moveSpeed = lastSpeed;
            potionActive = false;
		}
    }
}


