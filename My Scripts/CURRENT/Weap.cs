using StolenLands.Enemy;
using StolenLands.Projectiles;
using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;
using Sirenix.OdinInspector;
public enum WeaponType { Sword, Bow, Unarmed }
[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Gear/Weapons/New Weapon")]
public class Weap : Gear // WEAPON
{
    [Title("WEAPON PREFAB", Bold = true)]
    [SerializeField] private GameObject prefabToEquip = null;
    
    [Title("WEAPON CORE STATS", Bold = true)]
    public float damage;
    public float range;
    public float attackSpeed;
    [Space]
    [Title("WEAPON CRITICAL")]
    [Range(0.00f,1.00f)][Tooltip("0-1 chance")] public float criticalChance;
    public float critValue;
    
    
    [Title("WEAPON SPECIFIC", Bold = true)]
    [EnumToggleButtons]
    public WeaponType weaponType;
    
    
    public bool shootsProjectiles = false;
    
    [SerializeField] bool isRightHanded = true;
    
    [Title("RANGED WEAPONS", Bold = true)]
    [SerializeField] public Projectile arrow;
    
    
	public override void EquipWeapon(Transform rightHand, Transform leftHand, Animator animator)
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
}
