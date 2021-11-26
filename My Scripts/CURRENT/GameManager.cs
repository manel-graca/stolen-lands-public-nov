using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    [InfoBox("Name is set on char setup. Ignore bool if NOT in char setup scene")]
    
    [BoxGroup("Character Setup", CenterLabel = true)] 
    public string playerName;
    
    [BoxGroup("Character Setup")] 
    public bool characterSetup = false;
    
    [Space]
    
    [BoxGroup("Input", CenterLabel = true)]
    public bool receiveInput = true;
    
    [Space]
    
    [BoxGroup("Player Core", CenterLabel = true)]
    public bool walking = false;
    [BoxGroup("Player Core")]
    public bool sprinting = false;
    [BoxGroup("Player Core")]
    public bool crouching = false;
    [BoxGroup("Player Core")]
    public bool inCombat = false;
    
    [Space]
    
    [BoxGroup("Actions", CenterLabel = true)]
    public bool isGathering = false;
    [BoxGroup("Actions")]
    public bool isInteracting = false;
    [BoxGroup("Actions")]
    public bool dialogueOn = false;
    
    [Space]
    
    [BoxGroup("UI Control", CenterLabel = true)]
    public bool anyWindowOpen = false;
    
    [Space]
    
    [BoxGroup("Cinematics", CenterLabel = true)]
    public bool cinematicPlaying = false;
    
}
