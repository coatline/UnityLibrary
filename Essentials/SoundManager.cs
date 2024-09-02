using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource audioSourcePrefab;

    List<AudioSource> usableSoundSources;
    GameObject parent;

    private void Start()
    {
        usableSoundSources = new List<AudioSource>();
    }

    public void PlaySound(SoundType sound, Vector3 soundPosition, float volume = 1, float spatialBlend = 1)
    {
        if (parent == null)
            parent = new GameObject("Sounds");

        PlayAudioClip(sound.RandomSound, TryGetAudioSource(), soundPosition, volume, spatialBlend);
    }

    public void PlaySound(string soundName, Vector3 soundPosition, float volume = 1, float spatialBlend = 1)
    {
        PlaySound(DataLibrary.I.Sounds[soundName], soundPosition, volume, spatialBlend);
    }

    void PlayAudioClip(AudioClip clip, AudioSource audioSource, Vector3 soundPosition, float volume, float spatialBlend)
    {
        audioSource.transform.position = soundPosition;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);

        StartCoroutine(DelayUseAgain(audioSource, clip.length));
    }

    AudioSource TryGetAudioSource()
    {
        if (usableSoundSources.Count == 0)
            return NewAudioSource();
        else
        {
            // Removing from the end of the list is faster
            int index = usableSoundSources.Count - 1;
            AudioSource source = usableSoundSources[index];
            usableSoundSources.RemoveAt(index);
            return source;
        }
    }

    AudioSource NewAudioSource()
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, parent.transform);
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0.0f;

        return audioSource;
    }

    IEnumerator DelayUseAgain(AudioSource source, float soundLength)
    {
        yield return new WaitForSeconds(soundLength);

        if (source.gameObject != null)
            usableSoundSources.Add(source);
    }
}