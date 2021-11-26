using System;
using StolenLands.Enemy;
using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.EventSystems;

public class LootBox : MonoBehaviour
{
	public Animator animator;
	[SerializeField] float totalTimeInWorld;
	[SerializeField] float timeUntilDestruction;
	public GameObject enemyDropping;
	public bool alreadyClickedOn = false;
	public bool isWorldCrate = false;
	public LootDropSystem crateLootDrop;

	GameObject pCombat;
	void Start()
	{
		if (!isWorldCrate)
		{
			pCombat = GameObject.FindGameObjectWithTag("Player");
			transform.LookAt(pCombat.transform);
			gameObject.SetActive(false);

			animator.enabled = false;
		}
	}
	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void ChangeToAlreadyLooted()
	{
		Destroy(gameObject,2f);
	}
}
