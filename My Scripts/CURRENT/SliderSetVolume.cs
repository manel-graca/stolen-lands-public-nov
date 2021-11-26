using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SliderSetVolume : MonoBehaviour
{
	public Slider musicSlider;
	public Slider effectsSlider;
	public Slider ambienceSlider;
	public Slider interfaceSlider;

	public AudioMixer musicMixer = null;
    public AudioMixer effectsMixer = null;
	public AudioMixer ambienceMixer = null;
	public AudioMixer interfaceMixer = null;
	void Start()
	{
		if(musicSlider != null)
			musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
		if (effectsSlider != null)
			effectsSlider.value = PlayerPrefs.GetFloat("EffectsVol", 0.75f);
		if (ambienceSlider != null)
			ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVol", 0.75f);
		if (interfaceMixer != null)
			interfaceSlider.value = PlayerPrefs.GetFloat("InterfaceVol", 0.75f);
	}
	public void SetMusicLevel(float sliderValue)
    {
		musicMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
		PlayerPrefs.SetFloat("MusicVol", sliderValue);
	}

	public void SetEffectsVolume(float sliderValue)
	{
		effectsMixer.SetFloat("EffectsVol", Mathf.Log10(sliderValue) * 20);
		PlayerPrefs.SetFloat("EffectsVol", sliderValue);
	}

	public void SetAmbienceVolume(float sliderValue)
	{
		ambienceMixer.SetFloat("AmbienceVol", Mathf.Log10(sliderValue) * 20);
		PlayerPrefs.SetFloat("AmbienceVol", sliderValue);
	}

	public void SetInterfaceVolume(float sliderValue)
	{
		interfaceMixer.SetFloat("InterfaceVol", Mathf.Log10(sliderValue) * 20);
		PlayerPrefs.SetFloat("InterfaceVol", sliderValue);
	}
}
