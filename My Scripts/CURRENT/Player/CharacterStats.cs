using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterStats : MonoBehaviour
{
	#region Singleton
	public static CharacterStats instance;
	private void Awake()
	{
        instance = this;
	}
    #endregion

    public Stat Agility;
    public float agilityAmount;
    [Space]
    public Stat Armor;
    public float armorAmount;
    [Space]
    public Stat AttackDamage;
    public float attackDmgAmount;
    [Space]
    public Stat Dexterity;
    public float dexterityAmount;
    [Space]
    public Stat Focus;
    public float focusAmount;
    [Space]
    public Stat Endurance;
    public float enduranceAmount;
    [Space]
    public Stat Intellect;
    public float intellectAmount;
    [Space]
    public Stat MagicDamage;
    public float magicDmgAmount;
    [Space]
    public Stat MagicResist;
    public float magicResistAmount;
    [Space]
    public Stat Stamina;
    public float staminaAmount;
    [Space]
    public Stat Strength;
    public float strengthAmount;
    [Space]
    public Stat Vitality;
    public float vitalityAmount;
    [Space]
    [Space]
    public float baseValue;
    public float valueToAdd;
    public float percentageModifier;
    public float floatModifier;

    private void Update()
	{
		ClampStatValues();
    }

	private void ClampStatValues()
	{
		agilityAmount = Mathf.Clamp(agilityAmount, 0f, 2500f);
		armorAmount = Mathf.Clamp(armorAmount, 0f, 2500f);
		attackDmgAmount = Mathf.Clamp(attackDmgAmount, 0f, 2500f);
		dexterityAmount = Mathf.Clamp(dexterityAmount, 0f, 2500f);
		focusAmount = Mathf.Clamp(focusAmount, 0f, 2500f);
		enduranceAmount = Mathf.Clamp(enduranceAmount, 0f, 2500f);
		intellectAmount = Mathf.Clamp(intellectAmount, 0f, 2500f);
		magicDmgAmount = Mathf.Clamp(magicDmgAmount, 0f, 2500f);
		magicResistAmount = Mathf.Clamp(magicResistAmount, 0f, 2500f);
		staminaAmount = Mathf.Clamp(staminaAmount, 0f, 2500f);
		strengthAmount = Mathf.Clamp(strengthAmount, 0f, 2500f);
		vitalityAmount = Mathf.Clamp(vitalityAmount, 0f, 2500f);
	}

    public float GetMagicDamage()
	{
        return magicDmgAmount;
	}

	public void AddToStat(Gear gear, float amount)
	{
        if(valueToAdd > 0 || valueToAdd < 0)
		{
            valueToAdd = 0;
		}
        valueToAdd += amount;
		switch (gear.statType)
		{
			case StatsType.Agility:
				agilityAmount += valueToAdd;
				valueToAdd = 0f;
				break;
			case StatsType.Armor:
                armorAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Endurance:
				enduranceAmount += valueToAdd;
				valueToAdd = 0f;
				break;
			case StatsType.Stamina:
                staminaAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.AttackDamage:
                attackDmgAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.MagicDamage:
                magicDmgAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.MagicResist:
                magicResistAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Intellect:
                intellectAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Focus:
                focusAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Strength:
                strengthAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Dexterity:
                dexterityAmount += valueToAdd;
                valueToAdd = 0f;
                break;
			case StatsType.Vitality:
				vitalityAmount += valueToAdd;
				valueToAdd = 0f;
				break;
		}
	}

    public void RemoveFromStat(Gear gear, float amount)
	{
        valueToAdd += amount;

        switch (gear.statType)
        {
	        case StatsType.Agility:
		        agilityAmount = valueToAdd;
		        valueToAdd = 0f;
		        break;
            case StatsType.Armor:
                armorAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Stamina:
                staminaAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.AttackDamage:
                attackDmgAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Endurance:
	            enduranceAmount -= valueToAdd;
	            valueToAdd = 0f;
	            break;
            case StatsType.MagicDamage:
                magicDmgAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.MagicResist:
                magicResistAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Intellect:
                intellectAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Focus:
                focusAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Strength:
                strengthAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Dexterity:
                dexterityAmount -= valueToAdd;
                valueToAdd = 0f;
                break;
            case StatsType.Vitality:
	            vitalityAmount -= valueToAdd;
	            valueToAdd = 0f;
	            break;
        }
    }
}