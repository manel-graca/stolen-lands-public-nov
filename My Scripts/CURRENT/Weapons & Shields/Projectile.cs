using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StolenLands.Player;
using StolenLands.Enemy;

namespace StolenLands.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float arrowHitOffset = 1.4f;
        [SerializeField] private float speed = 5f;
        
        [SerializeField] Weap playerBow;

        PlayerCombatController player;
        PlayerSoundManager soundM;
        [HideInInspector] public EnemyController target;

		private void Start()
		{
            player = FindObjectOfType<PlayerCombatController>();
            target = player.enemyTarget;
            soundM = player.GetComponent<PlayerSoundManager>();
		}

		void Update()
        {
            GoToTarget();
        }

        private Vector3 GetHitPositionWithOffset()
		{
            CapsuleCollider targetCaps = target.GetComponent<CapsuleCollider>();
            Vector3 arrowLookAt = target.transform.position + Vector3.up * targetCaps.height / arrowHitOffset;
            return arrowLookAt;
		}
  
        public void GoToTarget()
		{
            if (target != null)

            transform.LookAt(GetHitPositionWithOffset());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

		private void OnTriggerEnter(Collider other)
		{
			if (playerBow != null)
			{
                if (other.gameObject.GetComponent<EnemyController>())
                {
                    other.GetComponent<EnemyController>().TakeDamage(player.GetWeaponFinalDamage(), player.GetIfCriticalDamage());
                    if (soundM != null)
                    {
                        soundM.PlayArrowHitSound();
                    }
                    Destroy(this.gameObject);
                }
            }
           
		}
	}
}

