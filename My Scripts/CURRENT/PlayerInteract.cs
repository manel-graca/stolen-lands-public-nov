using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Cinemachine;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using Random = UnityEngine.Random;

namespace StolenLands.Player
{
    public class PlayerInteract : MonoBehaviour
    {
		#region Singleton
		public static PlayerInteract instance;
		private void Awake()
		{
			instance = this;
		}
		#endregion
		[SerializeField] [Range(0,1)] private float rotationSpeed;
		[SerializeField] private GameObject playerCmCam;

		public NPC_Manager lastNPCClicked;
		public bool isGathering = false;
		
		[SerializeField] private NPC_Manager[] duplicatedNpcs;
		
        private Inventory inv;
        private NPC_Manager[] npcs;
        
	    public GathereableObject objToGather;
		
		Animator myAnimator;
        PlayerInput pInput;
        PlayerCombatController pCombat;
        GameManager manager;
        
		void Start()
		{
			manager = GameManager.instance;
			inv = Inventory.instance;
	        pInput = GetComponent<PlayerInput>();
			pCombat = GetComponent<PlayerCombatController>();
			myAnimator = GetComponent<Animator>();
			npcs = FindObjectsOfType<NPC_Manager>();
        }

		void Update()
		{
			if (lastNPCClicked != null)
			{
				if (lastNPCClicked.isInteracting)
				{
					FaceTarget();
				}
			}
			
		}

		public void StopNPCInteraction()
		{
			lastNPCClicked = null;
			foreach (var npc in npcs)
			{
				npc.isInteracting = false;
			}
		}

		public void SwitchVelkjirs(int toDestroy, int toActivate)
		{
			Destroy(duplicatedNpcs[toDestroy].gameObject);
			duplicatedNpcs[toActivate].gameObject.SetActive(true);
		}

		public void FaceTarget()
		{
			if (lastNPCClicked == null) return;
			
			var targetRotation = Quaternion.LookRotation(lastNPCClicked.transform.position - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
		}
		public void ChangeToNPCCam()
		{
			if (lastNPCClicked == null) return;
			
			playerCmCam.GetComponent<CinemachineFreeLook>().enabled = false;
			lastNPCClicked.GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
		}
		public void ChangeToPlayerCam()
		{
			if (lastNPCClicked == null) return;
			
			lastNPCClicked.isInteracting = false;
			lastNPCClicked.GetComponentInChildren<CinemachineVirtualCamera>().enabled = false;
			playerCmCam.GetComponent<CinemachineFreeLook>().enabled = true;
			lastNPCClicked = null;
		}
		public void StartGatherInteraction(GathereableObject objectToGather)
		{
			if (Vector3.Distance(transform.position, objectToGather.transform.position) <= objectToGather.minGatherDistance)
			{
				manager.isGathering = true;
				myAnimator.SetBool("isGathering", true);
			}
			else
			{
				PlayerUI.instance.InstantiateWarning("You are too far away!");
			}
		}
		public void StopGatherInteraction()
		{
			manager.isGathering = false;
			isGathering = false;
			myAnimator.SetBool("isGathering", false);
		}
		
		public void GatherResource() // ANIM EVENT
		{
			if (objToGather == null || !inv.GetIfCanAddItem())
			{
				Debug.Log("shit happenikng");
				return;
			}
				
			var itemToGather = objToGather.itemToGather;
			var amountToGather = objToGather.totalAmount;
				
			if(Inventory.instance.Add(itemToGather, amountToGather))
			{
				objToGather.ApplyGather();
			}
		}

		private bool GetIfHasObject()
		{
			if (!inv.HasItem(objToGather.requiredItem_1))
			{
				if(!inv.HasItem(objToGather.requiredItem_2))
				{
					return false;

				}
				else
				{
					return true;
				}
			}
			if (inv.HasItem(objToGather.requiredItem_1) || inv.HasItem(objToGather.requiredItem_2))
			{
				return true;
			}
			return false;
		}
		private void OnTriggerStay(Collider other)
		{
			if(other.GetComponentInParent<GathereableObject>())
			{
				objToGather = other.GetComponentInParent<GathereableObject>();
				objToGather.playerCloseBy = true;
				
				if (Vector3.Distance(transform.position, objToGather.transform.position) <= objToGather.minGatherDistance)
				{
					PlayerUI.instance.ActivateHintText(String.Format("Press (E) to mine {0}", objToGather.tooltipName));
				}
				if(pInput.interact) 
				{
					if (pCombat.isMelee || pCombat.isArcher)
					{
						PlayerUI.instance.InstantiateWarning("You need to be unarmed to gather resources");
						StopGatherInteraction();
						return;
					}

					if (objToGather.requiredItem_1 != null)
					{
						if (!GetIfHasObject())
						{
							PlayerUI.instance.InstantiateWarning("You need " + objToGather.requiredItem_1.itemName + " to gather this");
							StopGatherInteraction();
							return;
						}
					}
					if (!inv.GetIfCanAddItem() || isGathering)
					{
						StopGatherInteraction();
						return;
					}
					StartGatherInteraction(objToGather);
					return; 
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.GetComponentInParent<GathereableObject>())
			{
				objToGather = null;
				PlayerUI.instance.DeactivateHintText();
				other.GetComponentInParent<GathereableObject>().playerCloseBy = false;
				StopGatherInteraction();
			}
		}
		
		
		

    }
}

