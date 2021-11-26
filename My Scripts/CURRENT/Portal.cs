using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform receiver;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<PortalManager>().DoTransport(receiver ,receiver.Find("spawn"));
        }
    }
}
