using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Resourcesフォルダを使わずに全てのBGM/SEを管理するクラス。
/// </summary>
public class Sound : SingletonMonoBehaviour<Sound>
{
    //[SerializeField]
    //private GameObject m_soundObj = null;
    [SerializeField]
    private List<AudioClip> m_bgmList = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> m_seList = new List<AudioClip>();
    [SerializeField]
    private int MAX_PLAY_SE = 0;
    private AudioSource m_bgmSource;
    private Dictionary<Tuple<string, int>, AudioSource> m_seSources = new Dictionary<Tuple<string, int>, AudioSource>();

    private Dictionary<string, AudioClip> m_bgmDict = null;
    private Dictionary<string, AudioClip> m_seDict = null;

    protected override void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // 各種インスタンス
        m_bgmSource = gameObject.AddComponent<AudioSource>();
        m_seSources = new Dictionary<Tuple<string, int>, AudioSource>();
        m_bgmDict = new Dictionary<string, AudioClip>();
        m_seDict = new Dictionary<string, AudioClip>();

        void AddClipDict(Dictionary<string, AudioClip> dict, AudioClip clip)
        {
            if (!dict.ContainsKey(clip.name))
                dict.Add(clip.name, clip);
        }

        m_bgmList.ForEach(bgm => AddClipDict(m_bgmDict, bgm));
        m_seList.ForEach(se => AddClipDict(m_seDict, se));
    }

    private void Update()
    {
        
    }
    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="seName">ハンドル名</param>
    public void PlaySE(string seName, int instanceID, bool isLoop = false, float volume = 1.0f)
    {
        Tuple<string, int> key = Tuple.Create(seName, instanceID);
        // その名前の音源がない
        if (!m_seDict.ContainsKey(seName) ||
            // すでにその音がそのオブジェクトkら再生されている
            (m_seSources.ContainsKey(key) && m_seSources[key].isPlaying))
            return;

        AudioSource _sourceAt = null;
        foreach (AudioSource _source in m_seSources.Values)
        {
            if (_source.isPlaying)
                continue;

            _sourceAt = _source;
            break;
        }


        if (_sourceAt == null)
        {
            if (m_seSources.Count >= MAX_PLAY_SE)
                return;

            _sourceAt = gameObject.AddComponent<AudioSource>();
        }
        if(!m_seSources.ContainsKey(key))
            m_seSources.Add(key, _sourceAt);

        _sourceAt.clip = m_seDict[seName];
        _sourceAt.loop = isLoop;
        _sourceAt.Play();
        _sourceAt.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// SEを停止
    /// </summary>
    public void StopSE(string seName, int instanceID)
    {
        if (!m_seSources.ContainsKey(Tuple.Create(seName, instanceID)))
            return;

        foreach (AudioSource _source in m_seSources.Values)
        {
            if (!_source.isPlaying)
                continue;

            _source.Stop();
        }
        m_seSources.Remove(Tuple.Create(seName, instanceID));
    }

    /// <summary>
    /// SEを全て停止
    /// </summary>
    public void StopAllSE()
    {
        foreach (AudioSource audio in m_seSources.Values)
            audio.Stop();

        m_seSources.Clear();
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="bgmName">ハンドル名</param>
    public void PlayBGM(string bgmName, float volume = 1.0f)
    {
        if (!m_bgmDict.ContainsKey(bgmName) || m_bgmSource.clip == m_bgmDict[bgmName])
            return;

        m_bgmSource.Stop();
        m_bgmSource.clip = m_bgmDict[bgmName];
        m_bgmSource.Play();
        m_bgmSource.volume = Mathf.Clamp01(volume);
        m_bgmSource.loop = true;
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM()
    {
        m_bgmSource.Stop();
        m_bgmSource.clip = null;
    }
}