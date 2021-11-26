using System;
using StolenLands.Enemy;
using StolenLands.Projectiles;
using UnityEngine;

namespace StolenLands.Player
{
    [CreateAssetMenu(fileName = "Enemy Weapon", menuName = "Enemy/Weapon/New Weapon")]
    public class EnemyWeapon : ScriptableObject
    {
        [Header("Stats")]

        public float weaponRange = 4f;
        public float weaponDamage = 5f;
        
        [Tooltip("Default value, DO NOT CHANGE")] public float weaponDamageDefault = 8;
        
        public bool shootsProjectiles = false;
        [SerializeField] bool isRightHanded = true;

        [Header("Refs and Prefabs")]

        public GameObject spawnedWeapon = null;
        [SerializeField] public Projectile arrow;
        [SerializeField] private GameObject prefabToEquip = null;

        private void OnEnable()
        {
	        weaponDamage = weaponDamageDefault;
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
		{
            if (prefabToEquip != null)
			{
                Transform hand;
                if (isRightHanded)
                {
                    hand = rightHand;
                }
                else
                {
                    hand = leftHand;
                }
                GameObject spawnedWeapon = Instantiate(prefabToEquip, hand);
            }
		}

        public void ShootArrow(Transform launchTransform, EnemyController target)
		{
            Projectile arrowSpawned = Instantiate(arrow, launchTransform.transform.position, Quaternion.identity);
		}

        public float GetWeaponDamage()
		{
            return weaponDamage;
		}
    }
}


