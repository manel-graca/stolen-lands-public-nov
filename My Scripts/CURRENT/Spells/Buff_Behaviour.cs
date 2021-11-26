using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Abilities;
using StolenLands.Player;
using UnityEngine;
public enum BuffType {moveSpeed, health, mana, stats, critChance, meleeDamage, spellDamage, invincible}
public class Buff_Behaviour : MonoBehaviour
{
    public BuffType buffType;
    public Ability ability;
    GameObject player;
    
    PlayerMover pMover;
    PlayerControllerV2 pController;
    PlayerCombatController pCombat;
    PlayerResources pResources;
    CharacterStats pStats;
    PlayerHealthController pHealth;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        pMover = player.GetComponent<PlayerMover>();
        pController = player.GetComponent<PlayerControllerV2>();
        pCombat = player.GetComponent<PlayerCombatController>();
        pResources = player.GetComponent<PlayerResources>();
        pStats = player.GetComponent<CharacterStats>();
        pHealth = player.GetComponent<PlayerHealthController>();
    }

    private void Start()
    {
        switch (buffType)
        {
            case BuffType.moveSpeed:
                AddMovementSpeed();
                break;
            case BuffType.invincible:
                MakeInvincible();
                break;
            case BuffType.health:
                AddHealth();
                break;
        }
    }

    private void AddMovementSpeed()
    {
        StartCoroutine(MoveSpeedBuffRoutine());
    }
    IEnumerator MoveSpeedBuffRoutine()
    {
        pMover.moveSpeed += ability.effectAmount;
        yield return new WaitForSeconds(ability.effectDuration);
        pMover.moveSpeed -= ability.effectAmount;
        Destroy(gameObject, 0.1f);
    }

    private void MakeInvincible()
    {
        StartCoroutine(MakeInvincibleRoutine());
    }

    IEnumerator MakeInvincibleRoutine()
    {
        pController.isGodMode = true;
        yield return new WaitForSeconds(ability.effectDuration);
        pController.isGodMode = false;
        Destroy(gameObject, 0.1f);
    }

    private void AddHealth()
    {
        StartCoroutine(AddHealthRoutine());
    }

    IEnumerator AddHealthRoutine()
    {
        pHealth.health += ability.effectAmount;
        pHealth.maxHealth += ability.effectAmount;
        yield return new WaitForSeconds(ability.effectDuration);
        pHealth.health -= ability.effectAmount;
        pHealth.maxHealth -= ability.effectAmount;
        
    }
}
