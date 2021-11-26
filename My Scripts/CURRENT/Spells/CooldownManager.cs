using StolenLands.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public static CooldownManager instance;

    public List<Ability> abilitiesOnColldown = new List<Ability>();
    void Awake()
	{
		instance = this;
	}

	void Update()
	{
		for (int i = 0; i < abilitiesOnColldown.Count; i++)
		{
			abilitiesOnColldown[i].currentCooldown -= Time.deltaTime;
			if (abilitiesOnColldown[i].currentCooldown <= 0f)
			{
				abilitiesOnColldown[i].currentCooldown = 0f;
				if (abilitiesOnColldown[i].currentCooldown <= 0f)
				{
					abilitiesOnColldown.Remove(abilitiesOnColldown[i]);
				}
			}
		}
	}

    public void StartCooldown(Ability ability)
	{
		if (!abilitiesOnColldown.Contains(ability))
		{
			ability.currentCooldown = ability.maxCooldown;
			abilitiesOnColldown.Add(ability);
		}
	}
}
