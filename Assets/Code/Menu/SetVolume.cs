using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    public void SetLevel(float sliderValue)
    {
        // takes the '0.0001 - 1' slider value and turns it into '-80 - 0' on a logarithmic scale
        audioMixer.SetFloat("GlobalVolume", Mathf.Log10(sliderValue) * 20);
    }
}
