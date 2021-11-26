using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StolenLands.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private Image mountainFadeImage;
    
    private GameObject player;
    private PlayerInput input;
    private PlayerControllerV2 pController;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        input = player.GetComponent<PlayerInput>();
        pController = player.GetComponent<PlayerControllerV2>();
    }

    public void DoTransport(Transform receiverTransform, Transform receiverSpawn)
    {
        StartCoroutine(DoTransportRoutine(receiverTransform, receiverSpawn));
    }

    IEnumerator DoTransportRoutine(Transform receiverTransform, Transform receiverSpawn)
    {
        var pNav = player.GetComponent<NavMeshAgent>();
        
        pNav.enabled = false;
        pController.isInputBlock = true;
        
        mountainFadeImage.DOFade(1f, 1.5f);
        
        yield return new WaitForSeconds(1.65f);
        
        player.transform.position = receiverSpawn.position;
        
        yield return new WaitForSeconds(1f);
        
        mountainFadeImage.DOFade(0f, 1.5f);
        pNav.enabled = true;


        yield return new WaitForSeconds(1.5f);
        
        pController.isInputBlock = false;
        
    }
}
