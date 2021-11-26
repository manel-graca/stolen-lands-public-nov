using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : MonoBehaviour
{
	SphereCollider col;
	private void Start()
	{
		col = GetComponent<SphereCollider>();
		col.isTrigger = true;
		Invoke("TurnOffTrigger", 5.5f);
	}
	private void TurnOffTrigger()
	{
		col.isTrigger = false;
	}

}
