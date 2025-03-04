using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioSource musicSource1;
    [SerializeField] AudioSource musicSource2;
    [SerializeField] AudioMixer audioMixer;

    AudioSource currentSource;
    Song currentSong;

    protected override void Awake()
    {
        base.Awake();

        currentSource = musicSource2;
    }

    public void PlaySong(Song song, float fadeDuration, bool loop = true, bool restartIfAlreadyPlaying = false)
    {
        // Temporary
        if (song.Loop == null)
            return;

        // If we are trying to play a song that is already playing, check to see if we want to restart it.
        if (currentSource.isPlaying && currentSong == song)
            if (restartIfAlreadyPlaying == false)
                return;

        StartCoroutine(FadeInSong(song, fadeDuration, loop));
    }

    public void FadeOutCurrentSong(float fadeDuration)
    {
        StartCoroutine(FadeSource(currentSource, 0, fadeDuration));
    }

    IEnumerator FadeInSong(Song song, float fadeDuration, bool loop)
    {
        StartCoroutine(FadeSource(currentSource, 0, fadeDuration));

        if (currentSource == musicSource1)
            currentSource = musicSource2;
        else
            currentSource = musicSource1;

        StartCoroutine(FadeSource(currentSource, 1, fadeDuration));

        if (song.HasBeginning)
        {
            currentSource.PlayOneShot(song.Beginning);
            yield return new WaitForSeconds(song.LoopStartDelay + song.Beginning.length);
        }

        currentSource.loop = loop;

        currentSource.clip = song.Loop;
        currentSong = song;
        currentSource.Play();
    }

    IEnumerator FadeSource(AudioSource source, float targetVolumePercentage, float fadeDuration)
    {
        float percentage = 0;

        while (source.volume != targetVolumePercentage)
        {
            source.volume = Mathf.Lerp(source.volume, targetVolumePercentage, percentage);
            percentage = Mathf.Clamp01(percentage + Time.deltaTime / fadeDuration);
            yield return null;
        }

        if (targetVolumePercentage == 0)
            source.Stop();
    }
}
