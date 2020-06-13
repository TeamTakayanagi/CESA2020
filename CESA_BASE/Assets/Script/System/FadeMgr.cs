using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    public enum FadeType
    {
        Rat,
        Zoom,
        Alpha,
    }

    private Canvas m_canvas = null;
    private FadeBase m_fade = null;
    private List<FadeBase> m_fadeList = new List<FadeBase>();

    public FadeBase.FadeState State
    {
        get
        {
            if (!m_fade)
                return FadeBase.FadeState.None;

            return m_fade.State;
        }
    }

    override protected void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_canvas = GetComponent<Canvas>();

        for(int i = 0; i < transform.childCount; ++i)
        {
            m_fadeList.Add(transform.GetChild(i).GetComponent<FadeBase>());
        }

        m_fadeList.Sort((a, b) => a.FadeType - b.FadeType);
    }
    void Update()
    {
        // パワー
        if(!m_canvas.worldCamera)
            m_canvas.worldCamera = Camera.main;
    }

    public void StartFade(FadeType type, string nextScene)
    {
        m_fade = m_fadeList[(int)type];
        // フェード中にフェードが始まれば
        if(m_fade.State != FadeBase.FadeState.None)
        {
            // 現在のフェードを中断

        }
        m_fade.FadeStart(nextScene);
        EffectManager.Instance.DestoryEffects();
        EffectManager.Instance.Create = false;
    }
}
