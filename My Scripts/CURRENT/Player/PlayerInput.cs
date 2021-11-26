using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace StolenLands.Player
{
    public class PlayerInput : MonoBehaviour
    {
		#region Singleton
		public static PlayerInput instance;
		private void Awake()
		{
            instance = this;
		}
        #endregion
        [Header("Movement Related Input")]
        [FormerlySerializedAs("pressingCtrl")] public bool crouching = false;
        [FormerlySerializedAs("pressingShift")] public bool sprinting = false;
        [FormerlySerializedAs("pressingCaps")] public bool walking = false;
        [FormerlySerializedAs("pressingQ")]
        [Header("Combat Related Input")]
        public bool blockAttack;
        public bool isAnyNumberPressed;
        [Header("Camera Control Input")]
        public bool pressingALT = false;
        [Header("UI Input")]
        public bool pressingEsc = false;
        public bool openCraftWindow = false;
        [FormerlySerializedAs("pressingB")] public bool openInventory = false;
        [FormerlySerializedAs("pressingK")] public bool openCharacterWindow = false;
        [FormerlySerializedAs("pressingP")] public bool openSpellbook = false;
        [FormerlySerializedAs("pressingE")] public bool interact = false;
        [FormerlySerializedAs("pressingF")] public bool usingAbility = false;
        public bool cancelInteract = false;
        public bool isTyping = false;
        [Space]
        [Space]
        public int lastNumberPressed;
        [Space]
        public KeyCode escKey;
        public KeyCode interactKey;
        public KeyCode stopInteractionKey;
        public KeyCode mapKey;
        public KeyCode inventoryKey;
        public KeyCode craftingKey;
        public KeyCode spellBookKey;
        public KeyCode characterWindowKey;
        public KeyCode crouchKey;
        public KeyCode sprintKey;
        public KeyCode walkKey;
        public KeyCode useAbilityKey;
        public KeyCode blockAttackKey;
        
        PlayerSpellSystem spellSys;
        private PlayerControllerV2 playerController;
        private GameManager manager;

		private void Start()
		{
            manager = GameManager.instance;
            spellSys = GetComponent<PlayerSpellSystem>();
            playerController = GetComponent<PlayerControllerV2>();
        }

		private void Update()
        {
            if (playerController.isInputBlock || isTyping) return;
            if(!spellSys.isNowCasting && !spellSys.preCasting) NumbersInput();
            ToggleInput();
			HoldInput();
			GetIfAnyNumberIsPressed();
		}

        public bool GetInput(bool keyPressed) 
        {
            return keyPressed;
        }

        private void GetIfAnyNumberIsPressed()
		{
			if (lastNumberPressed != 0)
			{
				isAnyNumberPressed = true;
			}
			else
			{
				isAnyNumberPressed = false;
			}
		}

		private void ToggleInput()
        {
            if (Input.GetKeyDown(walkKey))
            {
                walking = !walking;
                manager.walking = walking;
            }
            if (Input.GetKeyDown(sprintKey))
            {
                sprinting = !sprinting;
                manager.sprinting = sprinting;
            }
            if (Input.GetKeyDown(crouchKey))
            {
                crouching = !crouching;
                manager.crouching = crouching;
            }
            if (Input.GetKeyDown(escKey))
			{
                pressingEsc = !pressingEsc;
			}
            if (Input.GetKeyDown(inventoryKey))
			{
                openInventory = !openInventory;
			}
            if (Input.GetKeyDown(characterWindowKey))
            {
                openCharacterWindow = !openCharacterWindow;
            }
            if (Input.GetKeyDown(spellBookKey) || Input.GetKeyDown(KeyCode.V))
            {
                openSpellbook = !openSpellbook;
            }
            if (Input.GetKeyDown(useAbilityKey))
            {
                usingAbility = !usingAbility;
            }

            if (Input.GetKeyDown(craftingKey))
            {
                openCraftWindow = !openCraftWindow;
            }
        }

        void NumbersInput()
		{
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 1)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 1;
                }
                else lastNumberPressed = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 2)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 2;
                }
                else lastNumberPressed = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 3)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 3;
                }
                else lastNumberPressed = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 4)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 4;
                }
                else lastNumberPressed = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 5)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 5;
                }
                else lastNumberPressed = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 6)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 6;
                }
                else lastNumberPressed = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (lastNumberPressed != 0)
                {
                    if (lastNumberPressed == 7)
                    {
                        lastNumberPressed = 0;
                    }
                    else lastNumberPressed = 7;
                }
                else lastNumberPressed = 7;
            }
        }

        private void HoldInput()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
			{
                pressingALT = true;
			}
			else
			{
                pressingALT = false;
			}

            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                blockAttack = true;
            }
            else
            {
                blockAttack = false;
            }

            if (Input.GetKey(interactKey))
            {
                interact = true;
            }
            else
            {
                interact = false;
            }
        }

    }
}


