using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    private double bgmStartDspTime;
    private float pauseTime = 0f;
    private bool isBgmPlaying = false;

    [Header("SFX")]
    public Sound[] sounds;

    public double BgmProgressDSP => AudioSettings.dspTime - bgmStartDspTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化音效庫
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.playOnAwake = false;
        }
    }

    public AudioClip LoadClip(string file)
    {
        AudioClip clip = ResourceManager.Load<AudioClip>(Path.Combine("Songs", file));
        if (clip == null)
        {
            Debug.LogError($"Can't load audio clip: {file}");
        }

        return clip;
    }

    public void LoadBGM(AudioClip clip)
    {
        if (bgmSource == null) return;

        bgmSource.clip = clip;
        if (!clip.loadState.Equals(AudioDataLoadState.Loaded))
        {
            // 先解壓縮避免後續卡頓
            clip.LoadAudioData();
        }
    }

    public void PlayBGM(double delay = 0f)
    {
        if (bgmSource == null || bgmSource.clip == null) return;
        bgmSource.Stop();
        double startTime = AudioSettings.dspTime + delay;
        bgmStartDspTime = startTime;
        isBgmPlaying = true;
        Debug.Log($"Play at : {BgmProgressDSP}");
        bgmSource.PlayScheduled(startTime);
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            isBgmPlaying = false;
        }
    }

    public void PauseBGM()
    {
        if (bgmSource != null && isBgmPlaying)
        {
            bgmSource.Pause();
            pauseTime = bgmSource.time;
            isBgmPlaying = false;
            Debug.Log($"Pause at : {BgmProgressDSP}");
        }
    }

    public void ResetBGM()
    {
        if (bgmSource != null && isBgmPlaying)
        {
            bgmSource.time = 0f;
        }
    }

    public void ResumeBgm()
    {
        if (bgmSource != null && !isBgmPlaying)
        {
            bgmSource.time = pauseTime;
            bgmSource.Play();
            bgmStartDspTime = AudioSettings.dspTime - pauseTime;
            isBgmPlaying = true;
            Debug.Log($"Resuming at : {BgmProgressDSP})");
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning($"音效 {name} 不存在！");
        }

        s.source.PlayOneShot(s.clip, s.volume);
    }

    public void StopSFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning($"音效 {name} 不存在！");
        }

        s.source.Stop();
    }
}
