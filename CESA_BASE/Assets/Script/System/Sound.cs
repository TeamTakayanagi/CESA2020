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
    public struct Sound_ID
    {
        string name;
        int instanceID;
    }

    public bool sound = false;
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
            Destroy(this);
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

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="seName">ハンドル名</param>
    public void PlaySE(string seName, int instanceID, float volume = 1.0f)
    {
        if (!sound || !m_seDict.ContainsKey(seName)) 
            return;

        AudioSource _source = m_seSources.FirstOrDefault(s => s.Value.isPlaying).Value;
        if (_source == null)
        {
            if (m_seSources.Count >= MAX_PLAY_SE) 
                return;

            _source = gameObject.AddComponent<AudioSource>();
            m_seSources.Add(Tuple.Create(seName, instanceID), _source);
        }

        _source.clip = m_seDict[seName];
        _source.Play();
        _source.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// SEを全て停止
    /// </summary>
    public void StopSE(string seName, int instanceID)
    {
        if (!sound || !m_seSources.ContainsKey(Tuple.Create(seName, instanceID))) 
            return;

        AudioSource _source = m_seSources[Tuple.Create(seName, instanceID)];
        if (_source)
        {
            _source.Stop();
            m_seSources.Remove(Tuple.Create(seName, instanceID));
        }
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
        if (!sound || !m_bgmDict.ContainsKey(bgmName) || m_bgmSource.clip == m_bgmDict[bgmName])
            return;

        m_bgmSource.Stop();
        m_bgmSource.clip = m_bgmDict[bgmName];
        m_bgmSource.Play();
        m_bgmSource.volume = Mathf.Clamp01(volume);
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
