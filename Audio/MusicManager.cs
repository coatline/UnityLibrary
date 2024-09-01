using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip[] musics;

    public void ModifyMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void ModifySFXVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public float Volume => musicSource.volume;
}
