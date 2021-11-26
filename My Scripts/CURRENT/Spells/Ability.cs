using System;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Sirenix.OdinInspector;
namespace StolenLands.Abilities
{
	[CreateAssetMenu(fileName = "Abilities", menuName = "New Ability/Create New Ability")]
	public class Ability : ScriptableObject, IDescriber
	{
		public enum AbilityGlobalType { projectile, shield, buff, heal, debuff, crowdControl, chainEffect, noType }
		public enum AbilityDamageType { arcane, fire, frost, electric, poison, noDamage }
		
		
		[PreviewField(90f, ObjectFieldAlignment.Left), HideLabel]
		[HorizontalGroup("Split", 90f)]
		public Sprite abilityImage;
		[Space]
		[VerticalGroup("Split/Right"), LabelWidth(120f)]
		public string abilityName;
		[VerticalGroup("Split/Right"), LabelWidth(120f)]
		public Color nameColor;
		[VerticalGroup("Split/Right"), LabelWidth(120f)]
		public AbilityGlobalType abilityGlobalType;
		[VerticalGroup("Split/Right"), LabelWidth(120f)]
		public AbilityDamageType abilityDamageType;
		
		[VerticalGroup("Left"),LabelWidth(120f)]
		[Title("Description")]
		[TextArea(6,20), HideLabel]public string abilityDescription;
		[Space]
		public GameObject spellPrefab;
		[Space]
		[HideInInspector] public float currentCooldown = 0;
		public float maxCooldown;
		public float manaCost;
		[Tooltip("AoE damage needs to be changed in AoEPrefab behaviour script!")]
		public float damage;
		public float damageOverTime;
		[Space]
		public float effectAmount;
		public float effectDuration;
		public float effectRadius;
		[Space]
		public bool followsTarget;
		public float maxRange;
		[Space]
		public float critValue;
		[Range(0,1)] 
		public float critChance;
		[Space]
		public float healAmount;
		public float healOverTimeAmount;
		public float timeBetweenHeals;
		[Space]
		public float manaRegen;
		[Space]
		public float castDuration;
		[Space]
		[TextArea(1, 2)]
		[SerializeField] string README;
		[Space]
		public int requiredLevel;
		public int requiredPointsToUnlock;
		[Space]
		
		[Space]
		[Header("Additional Effects")]
		[Space]
		public bool isStun;
		public bool isSlow;
		[Space] 
		[Header("Sound Effects")] 
		[Space]
		public AudioClip chargeSound;
		public AudioClip releaseSound;
		public AudioClip hitSound;

		string tooltipSpecificInfo;
		void OnEnable() // SUPER IMPORTANT, RESETS COOLDOWN ON ENABLE OTHERWISE IT WONT RESET!!
		{
			currentCooldown = 0f;
		}
		public GameObject InstantiateAbility(Vector3 spawnPosition)
		{
			GameObject abilityInstance = Instantiate(spellPrefab, spawnPosition, spellPrefab.transform.rotation);
			return abilityInstance;
		}
		public void StartAbilityCooldown()
		{
			CooldownManager.instance.StartCooldown(this);
		}
		public int GetRequiredLevel()
		{
			return requiredLevel;
		}
		public bool GetIfCanCast()
		{
			return currentCooldown <= 0;
		}
		public float GetManaCost()
		{
			return manaCost;
		}
		public float GetManaRegen()
		{
			return manaRegen;
		}
		public virtual string GetDescription()
		{
			string color = "#" + ColorUtility.ToHtmlStringRGB(nameColor);
			
			return string.Format("<color={0}>{1}</color>" + "\n<color={2}>{3}</color>" + "\n{4}",
				color, abilityName, color, tooltipSpecificInfo, abilityDescription);
		}
		public string GetPlaceText()
		{
			if (abilityGlobalType != AbilityGlobalType.buff && abilityGlobalType != AbilityGlobalType.debuff && 
			    abilityGlobalType != AbilityGlobalType.heal && abilityGlobalType != AbilityGlobalType.shield)
			{
				string s = abilityGlobalType.ToString();
				char[] a = s.ToCharArray();
				a[0] = char.ToUpper(a[0]);
				return new string(a) + string.Format("\n{0} meters", maxRange);
			}
			else
			{
				return String.Empty;
			}
		}
	}
}


