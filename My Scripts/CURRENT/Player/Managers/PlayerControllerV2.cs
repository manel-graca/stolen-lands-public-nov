using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StolenLands.Enemy;
using StolenLands.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace StolenLands.Player
{
	public class PlayerControllerV2 : MonoBehaviour
	{
		[SerializeField] private float minLootDistance;
		[SerializeField] private GameObject helmet;
		[SerializeField] private Material beardMaterial;
		[SerializeField] private LayerMask ignoreLayers;
		[SerializeField] private ParticleSystem mouseClickPS;
		[SerializeField] private GameObject enemyTooltip;
		public bool isInputBlock = false;
		public bool blockPlayerController = false;
		public LayerMask gatherObjectLayer;
		
		public EnemyController selectedEnemy = null;
		public NavMeshAgent playerAgent;
		
		[Header("Debug and Cheats")]
		[Space]
		public bool isGodMode;
		public bool playerInvisible;

		// CACHED REFERENCES

		Vector3 oldDestination;
		PlayerMover playerMover;
		PlayerCombatController playerCombatController;
		PlayerSpellSystem spellSys;
		PlayerHealthController playerHealthController;
		PlayerUI ui;
		private PausePanel pausePanel;

		void Start()
		{
			ui = PlayerUI.instance;
			playerMover = GetComponent<PlayerMover>();
			playerCombatController = GetComponent<PlayerCombatController>();
			spellSys = GetComponent<PlayerSpellSystem>();
			playerHealthController = GetComponent<PlayerHealthController>();
			pausePanel = FindObjectOfType<PausePanel>();
			enemyTooltip.gameObject.SetActive(false);
			
			beardMaterial.SetFloat("_Cutoff", PlayerPrefs.GetFloat("beard"));
			if (PlayerPrefs.GetString("helmet") == "false")
			{
				helmet.SetActive(false);
			}
			else
			{
				helmet.SetActive(true);
			}
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			{
				TurnOffSpellBools();
			}
			
			if(EventSystem.current.IsPointerOverGameObject() ||
			   pausePanel.isMenuActive || 
			   pausePanel.optionsPressed || 
			   isInputBlock) return;
			
			CheckIfOverLootBox();
			DeSelectTarget();
			ClickToDisableTooltips();

			if (DoStopMovement()) return;
			if (ClickToLoot()) return;
			if (CombatInteraction()) return;
			if (MovementInteraction()) return; 
		}

		public void StopReadInput()
		{
			isInputBlock = true;
		}
		public void StartReadingInput()
		{
			isInputBlock = false;
		}
		private void OnDrawGizmos()
		{
			if (playerAgent == null || playerAgent.path == null)
				return;

			var line = GetComponent<LineRenderer>();
			if (line == null)
			{
				line = gameObject.AddComponent<LineRenderer>();
			}

			var path = playerAgent.path;

			line.positionCount = path.corners.Length;

			for (int i = 0; i < path.corners.Length; i++)
			{
				line.SetPosition(i, path.corners[i] + Vector3.up);
			}
			if(Vector3.Distance(transform.position, playerAgent.destination) <= playerAgent.stoppingDistance)
			{
				line.positionCount = 0;
			}
		}
		
		private void TurnOffSpellBools()
		{
			if (!spellSys.isNowCasting && !spellSys.preCasting)
			{
				spellSys.isNowCasting = false;
				spellSys.preCasting = false;
			}
		}
		private bool DoStopMovement()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				playerMover.ResetCurrentPath();
				return true;
			}
			return false;
		}
		public bool MovementInteraction()
		{
			Ray ray;
			RaycastHit hit;
			bool hasHit;

			if (Input.GetMouseButtonDown(1) && !spellSys.isNowCasting)
			{
				if (EventSystem.current.IsPointerOverGameObject()) return false;
				
				ray = GetMousePosition();
				hasHit = Physics.Raycast(ray, out hit, 200f, ~ignoreLayers);
				Vector3 hitPos = hit.point;

				if (hasHit)
				{
					if (selectedEnemy != null && hit.transform.GetComponent<EnemyController>()) return false;
					if (hit.collider.gameObject == gameObject) return false;
					
					if (hit.transform.gameObject.GetComponent<Terrain>())
					{
						if (GameObject.FindWithTag("OnClickEffect"))
						{
							GameObject existingClickPS = GameObject.FindWithTag("OnClickEffect");
							Destroy(existingClickPS);
						}
						playerMover.MoveTo(hit.point);
						oldDestination = playerAgent.destination;
						Instantiate(mouseClickPS, hitPos, Quaternion.Euler(-90f, 0, 0));
						PlayerInteract.instance.StopGatherInteraction();
						return true;
					}
					else
					{
						if (oldDestination != null) playerAgent.destination = oldDestination;
						else playerAgent.isStopped = true;
						return false;
					}
				}
			}
			return false;
		}

		private void CheckIfOverLootBox()
		{
			bool isOverLoot = false;
			Ray ray = GetMousePosition();
			RaycastHit hit;
			Physics.Raycast(ray, out hit);
			if (hit.transform != null)
			{
				if (hit.transform.CompareTag("Enemy"))
				{
					if (hit.transform.GetComponentInChildren<LootBox>() && !isOverLoot)
					{
						if(Vector3.Distance(transform.position, hit.transform.position) > minLootDistance) return;
						CursorManager.instance.SetLootCursor();
						isOverLoot = true;
					}
				}
				else
				{
					CursorManager.instance.SetDefaultCursor();
					isOverLoot = false;
				}
			}
		}

		private bool ClickToLoot()
		{
			Ray ray = GetMousePosition();
			RaycastHit hit;
			if (Input.GetMouseButtonDown(0))
			{
				Physics.Raycast(ray, out hit);
				if (hit.transform.gameObject != null)
				{
					GameObject hitObj = hit.transform.gameObject;
					LootBox box = hitObj.GetComponentInChildren<LootBox>();
					if (box != null)
					{
						if (LootWindow.instance != null) return false;
						if (Vector3.Distance(transform.position, box.transform.position) > minLootDistance)
						{
							ui.InstantiateWarning("Too far");
							return false;
						}

						if (box.GetComponentInParent<LootDropSystem>().itemsToDrop.Count <= 0)
						{
							box.ChangeToAlreadyLooted();
							ui.InstantiateWarning("Crate already looted");
							return false;
						}
						
						box.alreadyClickedOn = true;
						box.animator.enabled = true;
						box.GetComponent<LootBox>().animator.SetTrigger("open");
						ui.InstantiateLootWindow();
						return true;
					}
				}
			}

			return false;
		}

		private void ClickToDisableTooltips()
		{
			Ray ray = GetMousePosition();
			RaycastHit hit;
			if(Input.GetMouseButtonDown(0) && !PlayerInput.instance.pressingALT)
			{
				Physics.Raycast(ray, out hit);
				Vector3 hitPos = hit.point;
				if (hitPos != null)
				{
					if (hit.transform.CompareTag("GatherObject") || hit.transform.CompareTag("Enemy")) return;
					enemyTooltip.gameObject.SetActive(false);
					PlayerUI.instance.HideTooltip();
					PlayerUI.instance.resourceTooltip.gameObject.SetActive(false);
				}
			}
		}
		private void DeSelectTarget()
		{
			if (PlayerInput.instance.pressingALT) return;
			if (spellSys.preCasting || spellSys.isNowCasting) return;
			Ray ray = GetMousePosition();
			RaycastHit hit;
			if (Input.GetMouseButtonDown(0) && !playerCombatController.isAttacking && !PlayerInput.instance.pressingALT)
			{
				Physics.Raycast(ray, out hit);

				if (hit.transform.GetComponent<Terrain>() || hit.transform.CompareTag("IgnoreMouseClick"))
				{
					if (playerCombatController.enemyTarget != null)
					{
						MakeTargetNull();
					}
					return;
				}
				if (hit.transform.CompareTag("Enemy")) return;
			}
		}

		public void MakeTargetNull()
		{
			if (spellSys.preCasting || spellSys.isNowCasting) return;
			
			PlayerInteract.instance.objToGather = null;
			selectedEnemy = null;
			enemyTooltip.gameObject.SetActive(false);
			playerCombatController.enemyTarget = null;
			playerCombatController.hasTarget = false;
		}

		private bool CombatInteraction()
		{
			RaycastHit[] hits = Physics.RaycastAll(GetMousePosition());
			foreach (RaycastHit hit in hits)
			{
				selectedEnemy = hit.transform.GetComponent<EnemyController>();

				if (selectedEnemy == null) continue;
				if (selectedEnemy.IsDead()) continue;

				
				if (Input.GetMouseButtonDown(0))
				{
					if (hit.transform.GetComponent<Terrain>())
					{
						return false;
					}
					selectedEnemy = selectedEnemy.GetComponent<EnemyController>();
					playerCombatController.SetEnemyTarget(selectedEnemy.gameObject);
				}
				if (Input.GetMouseButtonDown(1) && !isInputBlock && selectedEnemy != null)
				{
					if (playerMover.IsPlayerCrouching())
					{
						PlayerUI.instance.InstantiateWarning("Can't attack while crouching");
						return false;
					}
					
					if (Vector3.Distance(transform.position, selectedEnemy.transform.position) >
					    playerCombatController.currentWeapon.range)
					{
						playerMover.MoveTo(selectedEnemy.transform.position);
					}
					playerCombatController.SetEnemyTarget(selectedEnemy.gameObject);
					return true;
				}
			}
			return false;
		}

		public void CleanAgentPath()
		{
			playerAgent.isStopped = true;
			playerAgent.ResetPath();
		}

		public Ray GetMousePosition() 
		{
			return Camera.main.ScreenPointToRay(Input.mousePosition);
		}
	}
}

