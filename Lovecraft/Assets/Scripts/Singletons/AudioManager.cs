using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum SFXType
{
    UIPop,
    Impact,
    // Add other SFX types here
}

public enum BGMType
{
    MainMenu,
    // Add other BGM types here
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<SFXType, AudioClip> sfxClips = new Dictionary<SFXType, AudioClip>();
    private Dictionary<BGMType, AudioClip> bgmClips = new Dictionary<BGMType, AudioClip>();

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    public string sfxPath = "Audio/SFX/";
    public string bgmPath = "Audio/BGM/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;

            LoadAudioClips<SFXType>(sfxPath, sfxClips);
            LoadAudioClips<BGMType>(bgmPath, bgmClips);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAudioClips<T>(string path, Dictionary<T, AudioClip> clipDict) where T : System.Enum
    {
        foreach (T type in System.Enum.GetValues(typeof(T)))
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, path, type.ToString());
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.GetFiles(folderPath, "*.wav"))
                {
                    AudioClip clip = LoadClip(file);
                    if (clip != null)
                    {
                        clipDict[type] = clip;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Folder not found: {folderPath}");
            }
        }
    }

    private AudioClip LoadClip(string path)
    {
        AudioClip clip = null;
        WWW www = new WWW("file://" + path);
        clip = www.GetAudioClip(false, false);
        return clip;
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxClips.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX Clip not found for type: " + type);
        }
    }

    public void PlayBGM(BGMType type)
    {
        if (bgmClips.TryGetValue(type, out AudioClip clip) && bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM Clip not found for type: " + type);
        }
    }
}