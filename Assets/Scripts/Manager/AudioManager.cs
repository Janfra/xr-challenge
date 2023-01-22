using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private Audio[] Audios;

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
    private float volume;
    public float Volume => Mathf.Clamp01(volume);

    [SerializeField]
    private float spatialBlend;
    public float SpatialBlend => Mathf.Clamp01(spatialBlend);

    [SerializeField]
    private bool isLoop;

    public Audio(AudioClip _clip, float _volume, float _spatialBlend, bool _isLoop)
    {
        clip = _clip;
        volume = Mathf.Clamp01(_volume);
        spatialBlend = Mathf.Clamp01(_spatialBlend);
        isLoop = _isLoop;
    }

    public Audio(AudioClip _clip)
    {
        clip = _clip;
        volume = DEFAULT_VOLUME;
        spatialBlend = 0;
        isLoop = false;
    }

    public void SetVolume(float _volume)
    {
       volume = Mathf.Clamp01(_volume);
    }
}
