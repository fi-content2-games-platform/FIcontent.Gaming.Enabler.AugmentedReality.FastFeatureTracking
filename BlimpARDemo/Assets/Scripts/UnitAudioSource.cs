using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the audiosources of the units
/// </summary>
public class UnitAudioSource : MonoBehaviour
{
    //private GameObject gameObject;                                            // gameobject to attach the source
    private bool audioEnabled = false;                                          // enables the source

    private static List<WeakReference> instances = new List<WeakReference>();   // instance counter
    private const int MAX_AUDIOSOURCES = 99;                                   // limits the sources
    
    [HideInInspector]
    public float volumeFactor = 1;                                              // multiplies the volume

    /// <summary>
    /// Adds the source to the object
    /// </summary>
    public void AddSource(bool force = false)
    {
        if (!force)
            if (GetInstances().Count >= MAX_AUDIOSOURCES) return;
        
        this.Add();
        instances.Add(new WeakReference(this));
    }
    
    /// <summary>
    /// Plays a clip async
    /// </summary>    
    public void PlayOneShot(AudioClip audio)
    {
        if (audioEnabled)
            gameObject.audio.PlayOneShot(audio);
    }

    /// <summary>
    /// Plays the clip
    /// </summary>
    public void Play()
    {
        if (audioEnabled)
            gameObject.audio.Play();
    }

    public void Stop()
    {
        if (audioEnabled)
            gameObject.audio.Stop();
    }

    /// <summary>
    /// Sets the volume according to the global fx volume
    /// </summary>
    public void SetVolume()
    {
        if (audioEnabled) 
            gameObject.audio.volume =  AudioControl.EffectsVolume * volumeFactor;
    }

    /// <summary>
    /// Sets the audioclip
    /// </summary>
    public AudioClip clip
    {
        set
        {
            if (audioEnabled && gameObject.audio.clip != value)
                gameObject.audio.clip = value;
        }
    }

    /// <summary>
    /// sets and gets volume
    /// </summary>
    public float volume
    {
        get { return audioEnabled ? gameObject.audio.volume : 0; }
        set { if (audioEnabled) gameObject.audio.volume = value * volumeFactor; }
    }
    /// <summary>
    /// sets and gets pitch
    /// </summary>
    public float pitch
    {
        get { return audioEnabled ? gameObject.audio.pitch : 0; }
        set { if (audioEnabled) gameObject.audio.pitch = value; }
    }

    /// <summary>
    /// gets if the audioclip is playing
    /// </summary>
    public bool isPlaying
    {
        get { return audioEnabled ? gameObject.audio.isPlaying : false; }
    }
    /// <summary>
    /// sets and gets the loop mode
    /// </summary>
    public bool loop
    {
        get { return audioEnabled ? gameObject.audio.loop : false; }
        set { if (audioEnabled) gameObject.audio.loop = value; }
    }

    /// <summary>
    /// sets and gets the priority
    /// </summary>
    public int priority
    {
        get { return audioEnabled ? gameObject.audio.priority : 0; }
        set { if (audioEnabled) gameObject.audio.priority = value; }
    }

    public float maxDistance
    {
        get { return audioEnabled ? gameObject.audio.maxDistance : 0; }
        set { if (audioEnabled) gameObject.audio.maxDistance = value; }
    }

    /// <summary>
    /// adds the audiosource to the gameobject
    /// </summary>
    void Add()
    {
        this.gameObject.AddComponent<AudioSource>();
        this.gameObject.audio.bypassEffects = true;
        this.gameObject.audio.playOnAwake = true;
        this.gameObject.audio.priority = 128;
        this.gameObject.audio.volume = AudioControl.EffectsVolume * volumeFactor;
        this.gameObject.audio.dopplerLevel = 0;
        this.gameObject.audio.rolloffMode = AudioRolloffMode.Logarithmic;
        this.gameObject.audio.maxDistance = 1000;
        this.gameObject.audio.minDistance = 80;

        audioEnabled = true;
    }

    /// <summary>
    /// counts the instances
    /// </summary>
    /// <returns></returns>
    public static IList<UnitAudioSource> GetInstances()
    {
        List<UnitAudioSource> realInstances = new List<UnitAudioSource>();
        List<WeakReference> toDelete = new List<WeakReference>();

        foreach (WeakReference reference in instances)
        {
            if (reference.IsAlive)
            {
                realInstances.Add((UnitAudioSource)reference.Target);
            }
            else
            {
                toDelete.Add(reference);
            }
        }

        foreach (WeakReference reference in toDelete) instances.Remove(reference);

        return realInstances;
    }
}

