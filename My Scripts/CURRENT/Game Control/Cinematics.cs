using System;
using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace StolenLands.Cinematics
{
    public class Cinematics : MonoBehaviour
    {
	    [SerializeField] GameObject sphereCollider;
	    [SerializeField] RockPassage rockPassage;
	    [SerializeField] GameObject dollyCam;
	    [SerializeField] PlayerControllerV2 playerController;
	    [SerializeField] PlayableDirector playableDir;
	    [SerializeField] PlayerUI ui;
        public bool hasPlayerHit = false;
        public bool isCinematicRunning = false;
        
        GameManager manager;
        private void Start()
        {
	        manager = GameManager.instance;
        }

        private void Update()
        {
	        if (isCinematicRunning)
	        {
		        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
		        {
			        StopCinematic();
		        }
	        }
        }
        public void PlayCinematic()
        {
	        if(playableDir != null)
	        {
		        sphereCollider.SetActive(false);
		        StartCoroutine(CinematicRoutine());
		        Debug.Log("cinematic started");
	        }
        }
        private void StopCinematic()
        {
	        Destroy(dollyCam);
	        isCinematicRunning = false;
	        manager.cinematicPlaying = false;
	        playerController.StartReadingInput();
	        ui.ActivateDeactivateUI(true);
	        Debug.Log("cinematic stopped");
	        
	        if (!rockPassage.hasDetonated) // just in case some shit happens with timeline
	        {
		        rockPassage.Detonate();
	        }
        }
		IEnumerator CinematicRoutine()
		{
			ui.ActivateDeactivateUI(false);
			Debug.Log("cinematic started");
			playerController.StopReadInput();
			isCinematicRunning = true;
			manager.cinematicPlaying = true;
			dollyCam.SetActive(true);
			playableDir.Play();
			yield return new WaitForSeconds((float)playableDir.duration + 4f);
			StopCinematic();
			
		}
	}

}
