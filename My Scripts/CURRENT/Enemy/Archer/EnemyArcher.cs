using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StolenLands.Player;
using StolenLands.Projectiles;

namespace StolenLands.Enemy
{
    public class EnemyArcher : MonoBehaviour
    {
        [Header("Rig Transforms")]
        [SerializeField] Transform RighthandTransform;
        [SerializeField] Transform LefthandTransform;
        [SerializeField] Transform arrowLaunchTransform;
        
        [Header("Weapons")]
        [SerializeField] EnemyWeapon weaponToUse;
        [SerializeField] EnemyProjectile arrowPrefab;

        public AudioClip arrowHitPlayerSound;
        public AudioClip arrowReleaseSound;

        // CACHED REFERENCES
        EnemyController enemyController;
        AudioSource enemyAudioSource;
        EnemyController enemy;
        EnemyAIntelligence enemyAI;
        void Start()
        {
            enemyAI = GetComponent<EnemyAIntelligence>();
            enemyController = GetComponent<EnemyController>();
            enemyAudioSource = GetComponent<AudioSource>();
            enemy = GetComponent<EnemyController>();
            enemyAI.defaultWeapon = weaponToUse;
            EquipStartingWeapon(weaponToUse);
        }
		public void EquipStartingWeapon(EnemyWeapon weapon)
		{
            weapon = weaponToUse;
            weapon.Spawn(RighthandTransform, LefthandTransform, enemy.enemyAnimator);
        }

        public void InstantiateArrow()
		{
            EnemyProjectile arrowShot = Instantiate(arrowPrefab, arrowLaunchTransform.position, Quaternion.identity) as EnemyProjectile;
            enemyAudioSource.PlayOneShot(arrowReleaseSound, enemyController.soundFXVolume);
		}
    }
}

