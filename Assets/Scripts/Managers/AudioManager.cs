using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局音频管理器，负责音乐和音效播放
/// 特性：单例模式、BGM 管理、SFX 池、音量控制、淡入淡出
/// </summary>
public class AudioManager : MonoSingleton<AudioManager>
{
    /// <summary>
    /// 背景音乐 AudioSource
    /// </summary>
    private AudioSource _musicSource;

    /// <summary>
    /// 环境音 AudioSource
    /// </summary>
    private AudioSource _ambientSource;

    /// <summary>
    /// 音效 AudioSource 对象池
    /// </summary>
    private List<AudioSource> _sfxSourcePool = new List<AudioSource>();

    /// <summary>
    /// 音效池大小
    /// </summary>
    [SerializeField]
    private int _sfxPoolSize = 10;

    /// <summary>
    /// 背景音乐音量
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    private float _musicVolume = 0.7f;

    /// <summary>
    /// 音效音量
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    private float _sfxVolume = 1f;

    /// <summary>
    /// 是否静音
    /// </summary>
    private bool _isMuted = false;

    /// <summary>
    /// 当前淡入淡出协程
    /// </summary>
    private Coroutine _fadeCoroutine;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 创建背景音乐 AudioSource
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = _musicVolume;

        // 创建环境音 AudioSource
        _ambientSource = gameObject.AddComponent<AudioSource>();
        _ambientSource.loop = true;
        _ambientSource.playOnAwake = false;
        _ambientSource.volume = _musicVolume;

        // 初始化音效池
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = _sfxVolume;
            _sfxSourcePool.Add(sfxSource);
        }

        Debug.Log($"[AudioManager] Initialized with {_sfxPoolSize} SFX sources.");
    }

    private void Update()
    {
        // 全局鼠标左键点击音效
        if (Input.GetMouseButtonDown(0))
        {
            PlaySFXByName("Click");
        }
    }

    #region 背景音乐控制

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip">音乐片段</param>
    /// <param name="fadeTime">淡入时间（秒）</param>
    public void PlayMusic(AudioClip clip, float fadeTime = 0f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayMusic: clip is null.");
            return;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (fadeTime > 0f)
        {
            _fadeCoroutine = StartCoroutine(FadeMusicCoroutine(clip, fadeTime));
        }
        else
        {
            _musicSource.clip = clip;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }

        Debug.Log($"[AudioManager] Playing music: {clip.name}");
    }

    /// <summary>
    /// 通过名称播放背景音乐（从 Resources/Audio/BGM 加载）
    /// </summary>
    /// <param name="musicName">音乐名称</param>
    /// <param name="fadeTime">淡入时间</param>
    public void PlayMusicByName(string musicName, float fadeTime = 1f)
    {
        if (string.IsNullOrEmpty(musicName)) return;

        // 如果已经在播放该音乐，则跳过
        if (_musicSource.clip != null && _musicSource.clip.name == musicName) return;

        // 1. 优先在 Music 根目录查找
        AudioClip clip = Resources.Load<AudioClip>("Music/" + musicName);
        
        // 2. 尝试在 Music/Backgroundmusic 子目录查找
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Music/Backgroundmusic/" + musicName);
        }

        // 3. 兼容旧路径
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Audio/BGM/" + musicName);
        }

        if (clip != null)
        {
            PlayMusic(clip, fadeTime);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Music clip not found at Resources/Music/{musicName}, Resources/Music/Backgroundmusic/{musicName} or Resources/Audio/BGM/{musicName}");
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    /// <param name="fadeTime">淡出时间（秒）</param>
    public void StopMusic(float fadeTime = 0f)
    {
        if (fadeTime > 0f)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeOutMusicCoroutine(fadeTime));
        }
        else
        {
            _musicSource.Stop();
        }

        Debug.Log("[AudioManager] Stopped music.");
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
        _musicSource.Pause();
        Debug.Log("[AudioManager] Paused music.");
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeMusic()
    {
        _musicSource.UnPause();
        Debug.Log("[AudioManager] Resumed music.");
    }

    /// <summary>
    /// 淡入音乐协程
    /// </summary>
    private System.Collections.IEnumerator FadeMusicCoroutine(AudioClip newClip, float fadeTime)
    {
        // 淡出当前音乐
        float startVolume = _musicSource.volume;
        while (_musicSource.volume > 0f)
        {
            _musicSource.volume -= startVolume * Time.deltaTime / (fadeTime * 0.5f);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.Play();

        // 淡入新音乐
        while (_musicSource.volume < _musicVolume)
        {
            _musicSource.volume += _musicVolume * Time.deltaTime / (fadeTime * 0.5f);
            yield return null;
        }

        _musicSource.volume = _musicVolume;
        _fadeCoroutine = null;
    }

    /// <summary>
    /// 淡出音乐协程
    /// </summary>
    private System.Collections.IEnumerator FadeOutMusicCoroutine(float fadeTime)
    {
        float startVolume = _musicSource.volume;
        while (_musicSource.volume > 0f)
        {
            _musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.volume = _musicVolume;
        _fadeCoroutine = null;
    }

    #endregion

    #region 音效控制

    /// <summary>
    /// 播放音效（一次性）
    /// </summary>
    /// <param name="clip">音效片段</param>
    /// <param name="volume">音量（0-1）</param>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlaySFX: clip is null.");
            return;
        }

        // 从池中获取可用的 AudioSource
        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.volume = _sfxVolume * volume;
            availableSource.PlayOneShot(clip);
            Debug.Log($"[AudioManager] Playing SFX: {clip.name}");
        }
        else
        {
            Debug.LogWarning("[AudioManager] All SFX sources are in use. Consider increasing pool size.");
        }
    }

    /// <summary>
    /// 通过名称播放音效（从 Resources 加载）
    /// </summary>
    /// <param name="sfxName">音效名称</param>
    /// <param name="volume">音量（0-1）</param>
    public void PlaySFXByName(string sfxName, float volume = 1f)
    {
        if (string.IsNullOrEmpty(sfxName)) return;

        // 1. 优先在 Soundeffect 目录查找
        AudioClip clip = Resources.Load<AudioClip>("Music/Soundeffect/" + sfxName);
        
        // 2. 兼容旧路径或通用路径
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Audio/SFX/" + sfxName);
        }

        if (clip != null)
        {
            PlaySFX(clip, volume);
        }
        else
        {
            // 只有第一次找不到时报错，避免每帧点击都刷屏（针对调试）
            // Debug.LogWarning($"[AudioManager] SFX clip not found: {sfxName}");
        }
    }

    /// <summary>
    /// 播放循环音效
    /// </summary>
    /// <param name="clip">音效片段</param>
    /// <param name="volume">音量（0-1）</param>
    /// <returns>AudioSource 实例，用于后续控制</returns>
    public AudioSource PlayLoopingSFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayLoopingSFX: clip is null.");
            return null;
        }

        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.clip = clip;
            availableSource.volume = _sfxVolume * volume;
            availableSource.loop = true;
            availableSource.Play();
            Debug.Log($"[AudioManager] Playing looping SFX: {clip.name}");
            return availableSource;
        }

        Debug.LogWarning("[AudioManager] All SFX sources are in use.");
        return null;
    }

    /// <summary>
    /// 停止指定的音效源
    /// </summary>
    /// <param name="source">AudioSource 实例</param>
    public void StopSFX(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.loop = false;
            Debug.Log("[AudioManager] Stopped SFX.");
        }
    }

    /// <summary>
    /// 从池中获取可用的音效源
    /// </summary>
    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in _sfxSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return null;
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="volume">音量（0-1）</param>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _musicVolume;
        _ambientSource.volume = _musicVolume;
        Debug.Log($"[AudioManager] Music volume set to: {_musicVolume}");
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量（0-1）</param>
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        Debug.Log($"[AudioManager] SFX volume set to: {_sfxVolume}");
    }

    /// <summary>
    /// 设置全局静音
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        _isMuted = mute;
        AudioListener.volume = mute ? 0f : 1f;
        Debug.Log($"[AudioManager] Mute set to: {mute}");
    }

    /// <summary>
    /// 切换静音状态
    /// </summary>
    public void ToggleMute()
    {
        SetMute(!_isMuted);
    }

    /// <summary>
    /// 获取背景音乐音量
    /// </summary>
    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    /// <summary>
    /// 获取音效音量
    /// </summary>
    public float GetSFXVolume()
    {
        return _sfxVolume;
    }

    /// <summary>
    /// 获取静音状态
    /// </summary>
    public bool IsMuted()
    {
        return _isMuted;
    }

    #endregion
}
