using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StolenLands.Enemy
{
    public class EnemyDoctor : MonoBehaviour
    {
        [SerializeField] EnemyWeapon weaponToUse;
        EnemyAIntelligence enemyAI;

        void Start()
        {
            enemyAI = GetComponent<EnemyAIntelligence>();
            enemyAI.defaultWeapon = weaponToUse;
            EquipStartingWeapon(weaponToUse);
        }

        public void EquipStartingWeapon(EnemyWeapon weapon)
        {
            weapon = weaponToUse;
        }


    }
}

