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
    private bool isBgmPlaying = false;

    [Header("SFX")]
    public Sound[] sounds;

    public double BgmProgressDSP => isBgmPlaying ? AudioSettings.dspTime - bgmStartDspTime : 0.0;

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
        AudioClip clip = ResourceManager.Instance.Load<AudioClip>(Path.Combine("Songs", file));
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
        double startTime = AudioSettings.dspTime + delay;
        bgmSource.PlayScheduled(startTime);
        bgmStartDspTime = startTime;
        isBgmPlaying = true;
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
            isBgmPlaying = false;
        }
    }

    public void ResumeBgm()
    {
        if (bgmSource != null && !isBgmPlaying)
        {
            bgmSource.UnPause();
            isBgmPlaying = true;
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
