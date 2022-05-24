using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider mouseSensitivitySlider;
    public AudioMixer audioMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        { 
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        if (!Application.isPlaying) return;
        
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

}
