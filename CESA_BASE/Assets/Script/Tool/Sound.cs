using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utility
{
    /// <summary>
    /// Resourcesフォルダを使わずに全てのBGM/SEを管理するクラス。
    /// </summary>
    public class Sound : SingletonMonoBehaviour<Sound>
    {
        [SerializeField]
        private string          PATH            = "Asset/Audio/";   // Soundまでのパス
        [SerializeField]
        private string          BGM_FOLDER_NAME = "BGM/";           // BGM保存フォルダ名
        [SerializeField]
        private string          SE_FOLDER_NAME  = "SE/";            // SE保存フォルダ名
        [SerializeField]
        private int             MAX_PLAY_SE     = 10;               // SEの最大再生数
        [SerializeField]
        private List<AudioClip> m_bgmList       = null;             // BGMをインスペクターから取得
        [SerializeField]
        private List<AudioClip> m_seList        = null;             // SEをインスペクターから取得

        private AudioSource                   m_bgmSource = null;
        private List<AudioSource>             m_seSources = null;
        private Dictionary<string, AudioClip> m_bgmDict   = null;
        private Dictionary<string, AudioClip> m_seDict    = null;

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
            m_seSources = new List<AudioSource>();
            m_bgmDict   = new Dictionary<string, AudioClip>();
            m_seDict    = new Dictionary<string, AudioClip>();

            void AddClipDict(Dictionary<string, AudioClip> dict, AudioClip clip)
            {
                if (!dict.ContainsKey(clip.name)) dict.Add(clip.name, clip);
            }
            m_bgmList.ForEach(bgm => AddClipDict(m_bgmDict, bgm));
            m_seList. ForEach(se  => AddClipDict(m_seDict,  se ));
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="seName">ハンドル名</param>
        public void PlaySE(string seName)
        {
            if (!m_seDict.ContainsKey(PATH + SE_FOLDER_NAME + seName)) return;

            AudioSource _source = m_seSources.FirstOrDefault(s => !s.isPlaying);
            if (_source == null)
            {
                if (m_seSources.Count >= MAX_PLAY_SE) return;

                _source = gameObject.AddComponent<AudioSource>();
                m_seSources.Add(_source);
            }

            _source.clip = m_seDict[seName];
            _source.Play();
        }

        /// <summary>
        /// SEを全て停止
        /// </summary>
        public void StopSE()
        {
            m_seSources.ForEach(s => s.Stop());
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="bgmName">ハンドル名</param>
        public void PlayBGM(string bgmName)
        {
            if (!m_bgmDict.ContainsKey(PATH + BGM_FOLDER_NAME + bgmName)) return;
            if (m_bgmSource.clip == m_bgmDict[bgmName]) return;
            m_bgmSource.Stop();
            m_bgmSource.clip = m_bgmDict[bgmName];
            m_bgmSource.Play();
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
}
