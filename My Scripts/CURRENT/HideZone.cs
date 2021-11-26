using System;
using StolenLands.Player;
using UnityEngine;
public class HideZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerMover>().IsPlayerCrouching())
            {
                PlayerMover.instance.EnterHideZone(this);
                return;
            }
            else
            {
                PlayerMover.instance.ExitHideZone();
                return;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerMover>().IsPlayerCrouching())
            {
                PlayerMover.instance.ExitHideZone();
                return;
            }
            else return;
        }
    }
}
