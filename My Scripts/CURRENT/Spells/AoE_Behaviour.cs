using System;
using System.Collections.Generic;
using UnityEngine;
using StolenLands.Enemy;
using StolenLands.Player;

namespace StolenLands.Abilities
{
    public class AoE_Behaviour : MonoBehaviour
    {
	    [SerializeField] private bool isHeal;
        [SerializeField] private bool hitsIndividually;
        
        float totalDamage;
		float statMultiplier;
		float timeSinceHealed = Mathf.Infinity;
		PlayerCombatController pCombat;
		private PlayerSpellSystem spellSys;
		CharacterStats pStats;
		
		public Ability ability;
		
		
		private float finalDamage;
		private bool isCrit;
		void Start()
		{
			pStats = CharacterStats.instance;
			pCombat = pStats.gameObject.GetComponent<PlayerCombatController>();
			spellSys = pStats.gameObject.GetComponent<PlayerSpellSystem>();

			finalDamage = spellSys.finalDamage;
			isCrit = spellSys.nextSpellIsCrit;
		}

		private void Update()
		{
			timeSinceHealed += Time.deltaTime;
		}
		private void OnTriggerStay(Collider other)
		{
			if (!isHeal) return;
			
			if (other.CompareTag("Player"))
			{
				var pHealth = other.GetComponent<PlayerHealthController>();
				if (timeSinceHealed > 1.2f)
				{
					timeSinceHealed = 0f;
					pHealth.GeralHeal(ability.healOverTimeAmount);
				}
			}
			else return;
		}

		void OnParticleCollision(GameObject other)
		{
			if(isHeal) return;
			if (!hitsIndividually)
			{
				Collider[] cols = Physics.OverlapSphere(transform.position, ability.effectRadius);
				for (int i = 0; i < cols.Length; i++)
				{
					if (cols[i].GetComponent<EnemyController>() && cols[i].gameObject != this.gameObject )
					{
						cols[i].GetComponent<EnemyController>().TakeDamage(finalDamage, isCrit);
					}
				}

				return;
			}
			else
			{
				if (other.GetComponent<EnemyController>())
				{
					other.GetComponent<EnemyController>().TakeDamage(finalDamage, isCrit);
					return;
				}
			}
		}
	}
}

