using System;
using System.Collections;
using System.Collections.Generic;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;
    public Transform eyesRaycasterTransform;
    
    [FormerlySerializedAs("minDistanceToSeePlayer")] 
    public float minDistanceToSeePlayerHidden;
    
    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(eyesRaycasterTransform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (PlayerMover.instance.isHiding)
                    {
                        var hidezone = PlayerMover.instance.GetHidingZone();
                        if (Vector3.Distance(transform.position, hidezone.transform.position) < minDistanceToSeePlayerHidden)
                        {
                            canSeePlayer = true;
                            return;
                        }
                        else
                        {
                            canSeePlayer = false;
                            return;
                        }
                    }
                    canSeePlayer = true;
                    
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
