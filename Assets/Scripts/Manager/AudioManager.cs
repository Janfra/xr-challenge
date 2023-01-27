using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Dependencies")]
    /// <summary>
    /// Non spatial audios
    /// </summary>
    [SerializeField]
    private Audio[] audios;

    [SerializeField]
    /// <summary>
    /// Audios available to play
    /// </summary>
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("EDITOR ONLY")]
    private int audiosArrayLenght = -1;

    private void OnValidate()
    {
        if(audios.Length != audiosArrayLenght)
        {
            if (audios.Length > audiosArrayLenght && audiosArrayLenght > 0)
            {
                for(int i = audiosArrayLenght - 1; i < audios.Length; i++)
                {
                    audios[i].SetPitch(Audio.DEFAULT_VALUE);
                    audios[i].SetVolume(Audio.DEFAULT_VALUE);
                }
            }
            audiosArrayLenght = audios.Length;
        }
    }
#endif

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

        foreach(Audio audio in audios)
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
    /// Attempts to play the clip by the name given and set its volume
    /// </summary>
    /// <param name="_clipName">Name of clip being played</param>
    /// <param name="_volume">Volume of the clip</param>
    /// <returns>True if the clip was found, otherwise false</returns>
    public bool TryPlayAudio(string _clipName, float _volume)
    {
        AudioSource clipSource = GetAudioSource(_clipName);
        if (clipSource)
        {
            clipSource.volume = Mathf.Clamp01(_volume);
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
        if (!audioSources.ContainsKey(_audio.Clip.name))
        {
            audioSources.Add(_audio.Clip.name, _source);
            _source.loop = _audio.IsLoop;
            _source.volume = _audio.Volume;
            _source.pitch = _audio.Pitch;
            _source.clip = _audio.Clip;

            Debug.Log($"{_audio.Clip.name} was added to the sources!");
        }
        else
        {
            Debug.LogError($"{_audio.Clip.name} attempted to be added to the sources more than once!");
        }
    }
}

[System.Serializable]
public struct Audio
{
    public const float DEFAULT_VALUE = 1;

    [SerializeField]
    private AudioClip clip;
    public AudioClip Clip => clip;

    [SerializeField]
    [Range(0f, 1f)]
    private float volume;
    public float Volume => Mathf.Clamp01(volume);

    [SerializeField]
    [Range(-1f, 3f)]
    private float pitch;
    public float Pitch => pitch;

    [SerializeField]
    private bool isLoop;
    public bool IsLoop => isLoop;

    public Audio(AudioClip _clip, float _volume, float _pitch, bool _isLoop)
    {
        clip = _clip;
        pitch = _pitch;
        volume = Mathf.Clamp01(_volume);
        isLoop = _isLoop;
    }

    public Audio(AudioClip _clip)
    {
        clip = _clip;
        pitch = DEFAULT_VALUE;
        volume = DEFAULT_VALUE;
        isLoop = false;
    }

    public void SetVolume(float _volume)
    {
       volume = Mathf.Clamp01(_volume);
    }

    public void SetPitch(float _pitch)
    {
        pitch = Mathf.Clamp(_pitch, 1f, 3f);
    }
}
