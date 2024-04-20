using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioSettings : MonoBehaviour
{
    public Slider musicSlider; 
    public Slider sfxSlider;

    public static float music_volume = 0.5f;
    public static float sfx_volume = 1.0f;


    void Start()
    {
        music_volume = PlayerPrefs.GetFloat("music_volume");
        sfx_volume = PlayerPrefs.GetFloat("sfx_volume");
        // Додайте зворотні виклики до слайдерів
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void OnMusicVolumeChanged(float value)
    {
        music_volume = value;
    }

    void OnSFXVolumeChanged(float value)
    {
        sfx_volume = value;
    }
    
}


