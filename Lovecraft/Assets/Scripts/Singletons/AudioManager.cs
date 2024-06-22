using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public enum SFXType
{
    UIPop,
    Impact,
    RandomBark,
    CreatureSpawned,
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

    private Dictionary<SFXType, List<AudioClip>> sfxClips = new Dictionary<SFXType, List<AudioClip>>();
    private Dictionary<BGMType, AudioClip> bgmClips = new Dictionary<BGMType, AudioClip>();

    private List<AudioSource> sfxSources = new List<AudioSource>();
    private AudioSource bgmSource;
    public int sfxSourcePoolSize = 10; // Number of AudioSources in the pool

    public string sfxPath = "Audio/SFX/";
    public string bgmPath = "Audio/BGM/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < sfxSourcePoolSize; i++)
            {
                AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSources.Add(sfxSource);
            }

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;

            LoadAudioClips<SFXType>(sfxPath, sfxClips);
            LoadAudioClips<BGMType>(bgmPath, bgmClips);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAudioClips<T>(string path, Dictionary<T, List<AudioClip>> clipDict) where T : System.Enum
    {
        foreach (T type in System.Enum.GetValues(typeof(T)))
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, path, type.ToString());
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.GetFiles(folderPath, "*.wav"))
                {
                    StartCoroutine(LoadClipCoroutine(file, clip =>
                    {
                        if (!clipDict.ContainsKey(type))
                        {
                            clipDict[type] = new List<AudioClip>();
                        }
                        clipDict[type].Add(clip);
                    }));
                }
            }
            else
            {
                Debug.LogWarning($"Folder not found: {folderPath}");
            }
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
                    StartCoroutine(LoadClipCoroutine(file, clip =>
                    {
                        clipDict[type] = clip;
                    }));
                }
            }
            else
            {
                Debug.LogWarning($"Folder not found: {folderPath}");
            }
        }
    }

    private IEnumerator LoadClipCoroutine(string path, System.Action<AudioClip> callback)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                callback(clip);
            }
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxClips.TryGetValue(type, out List<AudioClip> clips) && clips.Count > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            AudioSource sfxSource = GetAvailableSFXSource();
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX Clips not found for type: " + type);
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

    public void PlayAudioClip(AudioClip clip)
    {
        AudioSource sfxSource = GetAvailableSFXSource();
        sfxSource.PlayOneShot(clip);
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If all sources are playing, return the first one (could create a new one if needed)
        return sfxSources[0];
    }
}
