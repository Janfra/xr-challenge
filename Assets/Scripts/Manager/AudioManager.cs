using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    /// <summary>
    /// Non spatial audios
    /// </summary>
    [SerializeField]
    private Audio[] Audios;

    /// <summary>
    /// Audios available to play
    /// </summary>
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    /// <summary>
    /// Sets instance, and creates and stores all audio sources for Audios array
    /// </summary>
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }

        foreach(Audio audio in Audios)
        {
            AudioSource currentSource = gameObject.AddComponent<AudioSource>();
            SetAudioSource(audio, currentSource);
        }
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// Attempts to play the clip by the name given
    /// </summary>
    /// <param name="_clipName">Name of clip being played</param>
    /// <returns>True if the clip was found, otherwise false</returns>
    public bool TryPlayAudio(string _clipName)
    {
        AudioSource clipSource = GetAudioSource(_clipName);
        if (clipSource)
        {
            clipSource.Play();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Attemps to stop playing the clip by the name given
    /// </summary>
    /// <param name="_clipName">Name of clip being played</param>
    /// <returns>True if the clip was found, otherwise false</returns>
    public bool TryStopAudio(string _clipName)
    {
        AudioSource clipSource = GetAudioSource(_clipName);
        if (clipSource)
        {
            clipSource.Stop();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Attemps to find the audio source of the clip name given to return it
    /// </summary>
    /// <param name="_clipName">Name of clip being found</param>
    /// <returns>Clip source if found, otherwise null</returns>
    private AudioSource GetAudioSource(string _clipName)
    {
        if (audioSources.TryGetValue(_clipName, out AudioSource source))
        {
            return source;
        }
        else
        {
            Debug.LogError($"Audio: {_clipName} was not found!");
            return null;
        }
    }

    /// <summary>
    /// Adds a new audio to the available playable audios
    /// </summary>
    /// <param name="_audio">Audio being added</param>
    /// <param name="_source">Source to set up and store</param>
    private void SetAudioSource(Audio _audio, AudioSource _source)
    {
        audioSources.Add(_audio.Clip.name, _source);
        _source.loop = _audio.IsLoop;
        _source.volume = _audio.Volume;
        _source.clip = _audio.Clip;

        Debug.Log($"{_audio.Clip.name} was added to the sources!");
    }
}

[System.Serializable]
public struct Audio
{
    public const float DEFAULT_VOLUME = 1;

    [SerializeField]
    private AudioClip clip;
    public AudioClip Clip => clip;

    [SerializeField]
    [Range(0f, 1f)]
    private float volume;
    public float Volume => Mathf.Clamp01(volume);

    [SerializeField]
    private bool isLoop;
    public bool IsLoop => isLoop;

    public Audio(AudioClip _clip, float _volume, float _spatialBlend, bool _isLoop)
    {
        clip = _clip;
        volume = Mathf.Clamp01(_volume);
        isLoop = _isLoop;
    }

    public Audio(AudioClip _clip)
    {
        clip = _clip;
        volume = DEFAULT_VOLUME;
        isLoop = false;
    }

    public void SetVolume(float _volume)
    {
       volume = Mathf.Clamp01(_volume);
    }
}
