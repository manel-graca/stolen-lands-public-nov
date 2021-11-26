using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StolenLands.Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField][Range(1f,20f)] public float speed = 5f;
        [SerializeField] private float arrowHitOffset = 1.4f;
        [SerializeField] EnemyWeapon simpleBow;
        public AudioClip arrowHitPlayerSound;
        public bool followsTarget = false;
        public float soundFXVolume = 2f;

        Vector3 targetLastPosition;

        EnemyArcher enemyArcher;
        EnemyController enemyController;
        EnemyAIntelligence enemyAI;
        AudioSource enemyAS;
        [HideInInspector] public PlayerHealthController target;
        
        void Awake()
        {
            enemyAS = GetComponent<AudioSource>();
            enemyController = GetComponent<EnemyController>();
            enemyAI = FindObjectOfType<EnemyAIntelligence>();
            enemyArcher = FindObjectOfType<EnemyArcher>();
            target = FindObjectOfType<PlayerHealthController>();
        }

		private void Start()
		{
            targetLastPosition = target.transform.position;
            if(!followsTarget)
			{
                transform.LookAt(GetHitPositionWithOffset(targetLastPosition));
            }
            Destroy(gameObject, 5f);
		}

		void Update()
        {
            if (target == null) return;
            GoToTarget();
        }

        private Vector3 GetHitPositionWithOffset(Vector3 targetPos)
        {
            CapsuleCollider targetCaps = target.GetComponent<CapsuleCollider>();
            Vector3 arrowLookAt = targetPos + Vector3.up * targetCaps.height / arrowHitOffset;
            return arrowLookAt;
        }

        public void GoToTarget()
        {
            if(followsTarget)
			{
                transform.LookAt(GetHitPositionWithOffset(target.transform.position));
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
			if (!followsTarget)
			{
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        private void PlayHitPlayerSound()
        {
            if (arrowHitPlayerSound != null)
            {
                enemyAS.PlayOneShot(arrowHitPlayerSound, soundFXVolume);
            }
        }

        private void OnTriggerEnter(Collider other) // when hits player
        {
            if (other.gameObject.GetComponent<PlayerHealthController>())
            {
                other.GetComponent<PlayerHealthController>().TakeDamage(simpleBow.weaponDamage, enemyAI.baseDamage);
                PlayHitPlayerSound();
                Destroy(this.gameObject);
            }
        }
	}
}

