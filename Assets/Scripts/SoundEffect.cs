using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip soundClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (soundClip != null)
        {
            audioSource.clip = soundClip;
        }
    }
    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
