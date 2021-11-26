using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace StolenLands.Player
{
    public class PlayerSoundManager : MonoBehaviour
    {
        #region Singleton

        public static PlayerSoundManager instance;

        private void Awake()
        {
            instance = this;
        }

        #endregion

        #region Vars

        [Header("Sword")] 
        [Space] 
        public AudioClip swordWithdraw;
        public AudioClip[] swordSwing;


        [Header("Bow")] 
        [Space] 
        public AudioClip[] bowDrawSound;
        public AudioClip[] arrowRelease;
        public AudioClip[] arrowHitOnEnemy;

        [Header("Player Sounds")] 
        [Space]
        public AudioClip[] playerHit;
        public AudioClip[] attackGrunt;

        [Space] 
        public AudioClip lowHealth;
        public AudioClip[] deathSound;
        [Space] 
        [Header("UI")]
        [Space] 
        public AudioClip errorClickSound;
        public AudioClip[] eatSounds;
        public AudioClip[] potionSounds;
        [Space] 
        public AudioClip lootItemSound;
        public AudioClip lootGearSound;
        public AudioClip levelUpSound;
        public AudioClip abilityUnlockSound;
        public AudioClip mouseOverAbilityTree;
        public AudioClip mousePickupAbilityTree;
        public AudioClip mousePlaceAbilityTree;
        [Space]
        [Header("Inventory / Shops / Currency")]
        [Space]
        public AudioClip conversionSuccessSound;
        public AudioClip itemBoughtSound;
        public AudioClip itemSoldSound;
        [Space]
        public AudioClip itemCraftedSound;
        [Space] 
        public AudioClip specialClickSound;
        public AudioClip mouseOverButton;
        public AudioClip mouseClickButton;
        public AudioClip mouseClickSlotSound;
        [Header("Windows Sounds")] 
        public AudioClip openPlayerWindowsSound;
        public AudioClip closePlayerWindowsSound;
        [Header("Equipping Gear Sounds")] 
        public AudioClip equipBagSound;
        public AudioClip unEquipBagSound;
        public AudioClip gearEquipSound;
        public AudioClip gearUnEquipSound;
        public AudioClip swordEquipSound;
        public AudioClip swordUnEquipSound;

        [Space] 
        [Header("Audio Sources")] 
        [Tooltip("Assign THIS prefab's AudioSource")]
        public AudioSource playerAudioSource;

        public AudioSource interfaceAudioSource;

        private GameObject player;

        //                                              //
        // VARS, WILL GET RANDOM VALUE ASSIGNED         //
        //                                              //
        AudioClip randomDeathSound;
        AudioClip randomArrowHit;
        AudioClip randomArrowRelease;
        AudioClip randomAttackGrunt;
        AudioClip randomSwordSwing;
        AudioClip randomPlayerHit;
        AudioClip randomBowDrawSound;
        AudioClip randomEatSound;
        AudioClip randomPotionSound;

        #endregion

        public AudioMixer effectsMixer;
        public AudioMixer musicMixer;
        public AudioMixer ambienceMixer;
        public AudioMixer interfaceMixer;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }
        

        public void PlaySimpleSound(AudioClip clip)
        {
            if (clip == null) return;
            playerAudioSource.PlayOneShot(clip);
        }

        public void PlayInterfaceSound(AudioClip clip)
        {
           if(clip == null) return;
           interfaceAudioSource.PlayOneShot(clip);
        }

        #region Animation Events

        public void PlaySwordPickupSound(AudioClip clip)
        {
            playerAudioSource.PlayOneShot(clip);
        }

        public void PlaySwordSwingSound()
        {
            if (swordSwing != null)
                randomSwordSwing = swordSwing[Random.Range(0, swordSwing.Length)];
            playerAudioSource.PlayOneShot(randomSwordSwing);
        }
        public void BowDrawSound()
        {
            if (bowDrawSound != null)
                randomBowDrawSound = bowDrawSound[Random.Range(0, bowDrawSound.Length)];
            playerAudioSource.PlayOneShot(randomBowDrawSound);
        }

        public void PlayArrowReleaseSound()
        {
            if (arrowRelease != null)
                randomArrowRelease = arrowRelease[Random.Range(0, arrowRelease.Length)];
            playerAudioSource.PlayOneShot(randomArrowRelease);
        }

        public void PlaySpellChargingSound(AudioClip clip)
        {
            if (clip != null)
            {
                playerAudioSource.PlayOneShot(clip);
            }
        }

        public void PlaySpellReleaseSound(AudioClip clip)
        {
            if (clip != null)
            {
                playerAudioSource.PlayOneShot(clip);
            }
        }

        public void PlaySpellHitSound(AudioClip clip)
        {
            if (clip != null)
            {
                playerAudioSource.PlayOneShot(clip);
            }
        }

        #endregion
        
        #region Arrow

        public void PlayArrowHitSound()
        {
            if (arrowHitOnEnemy != null)
                randomArrowHit = arrowHitOnEnemy[Random.Range(0, arrowHitOnEnemy.Length)];
            playerAudioSource.PlayOneShot(randomArrowHit);
        }

        public void PlayAttackGruntSound()
        {
            if (swordSwing != null)
                randomAttackGrunt = attackGrunt[Random.Range(0, attackGrunt.Length)];
            playerAudioSource.PlayOneShot(randomAttackGrunt);
        }
        
        #endregion

        public void PlayLowHealthSound()
        {
            if (lowHealth != null)
            {
                if (!playerAudioSource.isPlaying)
                {
                    playerAudioSource.PlayOneShot(lowHealth);
                }
            }
        }

        public void PlayHurtSound()
        {
            if (playerHit != null)
            {
                randomPlayerHit = playerHit[Random.Range(0, playerHit.Length)];
                if (!playerAudioSource.isPlaying)
                {
                    playerAudioSource.PlayOneShot(randomPlayerHit);
                }
            }
        }

        public void PlayDeathSound()
        {
            if (deathSound != null)
                randomDeathSound = deathSound[Random.Range(0, deathSound.Length)];
            playerAudioSource.PlayOneShot(randomDeathSound);
        }


        #region UI

        public void PlayOnAbilityUnlockSound()
        {
            if (abilityUnlockSound == null) return;
            interfaceAudioSource.PlayOneShot(abilityUnlockSound);
        }

        public void PlayFoodEatSound()
        {
            randomEatSound = eatSounds[Random.Range(0, eatSounds.Length)];
            interfaceAudioSource.PlayOneShot(randomEatSound);
        }

        public void PlayPotionUseSound()
        {
            randomPotionSound = potionSounds[Random.Range(0, potionSounds.Length)];
            interfaceAudioSource.PlayOneShot(randomPotionSound);
        }
        
        public void PlaySpecialClickSound()
        {
            if(specialClickSound == null) return;
            interfaceAudioSource.PlayOneShot(specialClickSound);
        }

        public void PlayButtonMouseOverSound()
        {
            if (mouseOverButton == null) return;
            interfaceAudioSource.PlayOneShot(mouseOverButton);
        }

        public void PlayButtonMouseClickSound()
        {
            if (mouseClickButton == null) return;
            interfaceAudioSource.PlayOneShot(mouseClickButton);
        }

        public void PlayMouseClickSlot()
        {
            if (mouseClickSlotSound == null) return;
            interfaceAudioSource.PlayOneShot(mouseClickSlotSound);
        }

        public void PlayPlayerWindowsOpenSound()
        {
            if (openPlayerWindowsSound == null) return;
            interfaceAudioSource.PlayOneShot(openPlayerWindowsSound);
        }

        public void PlaySwordEquipSound(bool equipping)
        {
            if (equipping)
            {
                interfaceAudioSource.PlayOneShot(swordEquipSound);
            }
            else
            {
                interfaceAudioSource.PlayOneShot(swordUnEquipSound);
            }
        }

        #endregion
    }
}