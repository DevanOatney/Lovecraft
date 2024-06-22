using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct BGMusic {
    public float delayOffset;
    public AudioClip music;
    [Range(0, 1)]
    public float volume;

    public BGMusic(float _delay, AudioClip _music, float _volume)
    {
        delayOffset = _delay;
        music = _music;
        volume = _volume;
    }
}

public class MainMenuAudioController : MonoBehaviour
{
    [SerializeField] private float DelayToStartBackgroundMusic = 0;
    [SerializeField] private AudioClip OpeningSoundEffect;
    [Range(0,1)]
    [SerializeField] private float OpeningSFXVolume = 0;
    [SerializeField] private List<BGMusic> BackgroundMusicList;

    // Start is called before the first frame update
    void Start()
    {
        GameObject openingSFXSource = new GameObject("OpeningSFXSource");
        openingSFXSource.AddComponent<AudioSource>();
        openingSFXSource.GetComponent<AudioSource>().clip = OpeningSoundEffect;
        openingSFXSource.GetComponent<AudioSource>().volume = OpeningSFXVolume;
        openingSFXSource.GetComponent<AudioSource>().Play();

        foreach(BGMusic _music in BackgroundMusicList)
        {
            GameObject bgmusic = new GameObject("BGMusic_" + _music.music.name);
            bgmusic.AddComponent<AudioSource>();
            bgmusic.GetComponent<AudioSource>().playOnAwake = false;
            bgmusic.GetComponent<AudioSource>().clip = _music.music;
            bgmusic.GetComponent<AudioSource>().volume = _music.volume;
            bgmusic.GetComponent<AudioSource>().PlayDelayed(DelayToStartBackgroundMusic + _music.delayOffset);
        }
    }
}
