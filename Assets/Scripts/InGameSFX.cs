using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSFX : MonoBehaviour
{
    public AudioSource music;
    private AudioSource audioSource;
    public AudioClip buttonClick;
    public AudioClip pieceCleared;
    public AudioClip gameEnd;

    void Start()
    {
        music.volume = AudioSettings.music_volume;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = AudioSettings.sfx_volume;
        if (buttonClick != null)
        {
            audioSource.clip = buttonClick;
        }
    }
    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }

}
