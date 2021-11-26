using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using StolenLands.UI;
using UnityEngine.Rendering.PostProcessing;

public class OptionsManager : MonoBehaviour
{
	[Header("Character Setup Scene")] 
	[Space]
	[SerializeField] private Toggle expensiveObjToggle;
	[SerializeField] private GameObject[] expensiveObjects;
	[SerializeField] private bool performanceMode;
	[Space]
	
	[Header("Display")]
	[Space]
    [SerializeField] private GameObject fpsLimiterObject;
    public Dropdown fpsLimiterDropdown;
    public Toggle vSyncToggle;
    public Dropdown resolutionDropdown;
    public GameObject debuggerObject;
    private Resolution[] resolutions;
    
    [Space]
    [Header("Quality")]
    [Space]
    
    public Dropdown qualityDropdown;
    [SerializeField] PostProcessVolume PostFxVolume;

    [Space] 
    [Header("Game Settings")] 

    [Tooltip("0 -> performance ; 1 -> quality")]
    public int bloodQuality;
    public bool bloodOn = true;
    public Text bloodDropdownText;
    public Toggle bloodToggle;
    public Dropdown bloodQualityDropdown;
    public bool hitEffect;
    public bool cheatsAllowed;
    
    [Space]
    [Header("Navigation")]
    [Space]
    
    public GameObject minimapCanvas;
    public GameObject compassCanvas;
    
    [Space]
    [Header("Categories")]
    [Space]
    
    [SerializeField] GameObject audioCategory = null;
    [SerializeField] GameObject qualityCategory = null;
    [SerializeField] GameObject displayCategory = null;
    [SerializeField] GameObject gameCategory = null;
    

    private Terrain[] terrains;
    private Tree[] treesInScene;

    private void Start()
    {
	    treesInScene = FindObjectsOfType<Tree>();
	    terrains = FindObjectsOfType<Terrain>();
	    
	    UpdateOptionsUIElements();
	    
	    GetScreenResolutions();

	    if (QualitySettings.vSyncCount == 0)
	    {
		    Application.targetFrameRate = 60;
	    }
	}

    void UpdateOptionsUIElements()
    {
	    if (expensiveObjToggle != null)
	    {
		    expensiveObjToggle.isOn = false;
	    }
	    if (bloodQualityDropdown != null)
	    {
		    if (!bloodOn)
		    {
			    bloodQualityDropdown.interactable = false;
		    }
		    else
		    {
			    bloodQualityDropdown.interactable = true;
		    }
	    }
	    if (qualityDropdown != null)
	    {
		    qualityDropdown.value = QualitySettings.GetQualityLevel();
	    }
	    if (vSyncToggle != null)
	    {
		    if (QualitySettings.vSyncCount > 0)
		    {
			    fpsLimiterObject.SetActive(false);
		    }
		    else
		    {
			    vSyncToggle.isOn = false;
		    }
	    }
    }

    public void ToggleAllowCheats(bool boolean)
    {
	    cheatsAllowed = boolean;
    }

    public void OnOffExpensiveObjects(bool boolean)
    {
	    performanceMode = boolean;
	    foreach (var var in expensiveObjects)
	    {
		    if (performanceMode)
		    {
			    var.SetActive(false);
		    }
		    else
		    {
			    var.SetActive(true);
		    }
	    }
    }
    private void GetScreenResolutions()
	{
        resolutions = Screen.resolutions.Select(resolution =>
        new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
			{
                currentResIndex = i;
			}
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetFrameRate(int index)
    {
	    if (QualitySettings.vSyncCount > 0) return;
	    
	    int targetFps = 60;
	    switch (index)
	    {
		    case 0:
			    targetFps = 60;
			    PlayerPrefs.SetInt("fps", targetFps);
			    break;
		    case 1:
			    targetFps = 90;
			    PlayerPrefs.SetInt("fps", targetFps);
			    break;
		    case 2:
			    targetFps = 120;
			    PlayerPrefs.SetInt("fps", targetFps);
			    break;
		    case 3:
			    targetFps = 144;
			    PlayerPrefs.SetInt("fps", targetFps);
			    break;
		    case 4:
			    targetFps = 240;
			    PlayerPrefs.SetInt("fps", targetFps);
			    break;
	    }
	    Application.targetFrameRate = targetFps;
	    fpsLimiterDropdown.RefreshShownValue();
    }
    public void EnableDebugger(bool boolean)
    {
	    if (boolean)
	    {
		    debuggerObject.SetActive(true);
	    }
	    else debuggerObject.SetActive(false);
    }

	public void SetFullscreenMode(int index)
	{
		switch (index)
		{
			case 0:
				Screen.fullScreenMode = FullScreenMode.Windowed;
				break;
			case 1:
				Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
				break;
			case 2:
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				break;
			case 3:
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				break;
		}
	}
    public void SetResolution(int resolutionIndex)
	{
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionDropdown.RefreshShownValue();
	}
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
        qualityDropdown.RefreshShownValue();
    }

    public void TerrainTreesAndVegetation(bool boolean)
    {
	    foreach (var terrain in terrains)
	    {
		    if (boolean)
		    {
			    terrain.drawTreesAndFoliage = true;
		    }
		    else terrain.drawTreesAndFoliage = false;
	    }
    }

    public void WorldTreesToggle(bool boolean)
    {
	    foreach (var tree in treesInScene)
	    {
		    if (boolean)
		    {
			    tree.gameObject.SetActive(true);
		    }
		    else tree.gameObject.SetActive(false);
	    }
    }
    public void SetVsync(bool vSync)
    {
	    if (vSync)
	    {
		    QualitySettings.vSyncCount = 1;
		    fpsLimiterObject.SetActive(false);
		    PlayerPrefs.SetInt("vsync", 1);
	    }
	    else
	    {
		    QualitySettings.vSyncCount = 0;
		    fpsLimiterObject.SetActive(true);
		    PlayerPrefs.SetInt("vsync", 0);
	    }
    }

    public void LockUnlockUI()
    {
	    PlayerUI.instance.canMoveUI = !PlayerUI.instance.canMoveUI;
    }

    public void HideShowMinimap(bool boolean)
    {
	    minimapCanvas.SetActive(boolean);
    }

    public void HideShowCompass(bool boolean)
    {
	    compassCanvas.SetActive(boolean);
    }


    public void SetPostFX(bool isOn)
    {
	    PostFxVolume.isGlobal = isOn;
	    if (!isOn)
	    {
		    PostFxVolume.weight = 0;
	    }
	    if (isOn)
	    {
		    PostFxVolume.weight = 1;
	    }
    }
    
    public void SetBloodQuality(int index)
    {
	    bloodQuality = index;
    }

    public void TurnBloodOnOff(bool boolean)
    {
	    bloodOn = boolean;
	    if (!bloodOn)
	    {
		    bloodQualityDropdown.interactable = false;
		    return;
	    }
	    else
	    {
		    bloodQualityDropdown.interactable = true;
	    }
    }

    public void OnOffHitEffect(bool boolean)
    {
	    hitEffect = boolean;
    }

    private int GetQualityLevel()
	{
        return PlayerPrefs.GetInt("quality");
	}
    public void ShowAudioCategory()
	{
        qualityCategory.SetActive(false);
        displayCategory.SetActive(false);
        gameCategory.SetActive(false);
        
        audioCategory.SetActive(true);
	}
    public void ShowQualityCategory()
    {
        displayCategory.SetActive(false);
        audioCategory.SetActive(false);
        gameCategory.SetActive(false);
        
        qualityCategory.SetActive(true);
    }
    public void ShowDisplayCategory()
    {
        qualityCategory.SetActive(false);
        audioCategory.SetActive(false);
        gameCategory.SetActive(false);
        
        displayCategory.SetActive(true);
    }

    public void ShowGameDisplay()
    {
	    qualityCategory.SetActive(false);
	    displayCategory.SetActive(false);
	    audioCategory.SetActive(false);
	    
	    gameCategory.SetActive(true);
    }
}

