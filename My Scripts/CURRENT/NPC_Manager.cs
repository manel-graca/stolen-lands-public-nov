using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PixelCrushers.DialogueSystem;
using StolenLands.Player;
using UnityEngine;
public enum NPCName{VelkjirOne, VelkjirTwo, Floki, Shopkeeper}
public class NPC_Manager : MonoBehaviour
{
    [SerializeField] [Range(0,1)] private float rotationSpeed;
    [SerializeField] private GameObject npcCam;
    [SerializeField] private GameObject playerCam;
    public bool isInteracting = false;
    public NPCName npcName;
    
    private Quaternion initialRotation;
    private GameObject player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        
        initialRotation = transform.rotation;

        StartCoroutine(SetCameraOnRoutine());
    }

    private void Update()
    {
        if (isInteracting)
        {
            FacePlayer();
        }
    }

    private void OnMouseDown()
    {
        StartInteractionWithPlayer();
    }

    private void OnMouseEnter()
    {
        CursorManager.instance.SetInteractCursor();
    }

    private void OnMouseExit()
    {
        CursorManager.instance.SetDefaultCursor();
        isInteracting = false;
    }

    public void StartInteractionWithPlayer()
    {
        isInteracting = true;
        PlayerInteract.instance.lastNPCClicked = this;
        CursorManager.instance.SetDefaultCursor();
    }
    IEnumerator SetCameraOnRoutine()
    {
        npcCam.SetActive(false);
        yield return (.05f);
        npcCam.SetActive(true);
    }

    public void FacePlayer()
    {
        var targetRotation = Quaternion.LookRotation(PlayerInteract.instance.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
    }
}
