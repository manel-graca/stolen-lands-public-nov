using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsType { Agility, Armor, Stamina, AttackDamage, MagicDamage, MagicResist, Intellect, Focus, Strength, Dexterity, Endurance, Vitality, NoStat }
[CreateAssetMenu(fileName = "New Stat", menuName = "Stats/New Stat")]
public class Stat : ScriptableObject
{
	public StatsType statType;
}
