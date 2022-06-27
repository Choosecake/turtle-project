using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider mouseVolumeSlider;
    public AudioMixer audioMixer;

    private const string SensitivityString = "Sensitivity";
    private const string VolumeString = "Volume";

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(SensitivityString))
        { 
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat(SensitivityString);
        }

        float volumeValue;
        if (audioMixer.GetFloat(VolumeString, out volumeValue))
        {
            mouseVolumeSlider.value = volumeValue;
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        if (!Application.isPlaying) return;
        
        PlayerPrefs.SetFloat(SensitivityString, sensitivity);
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(VolumeString, volume);
    }

}
