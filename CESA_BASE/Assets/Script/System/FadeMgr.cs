﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    public enum FadeType
    {
        Rat,
        Scale,
        Alpha,
    }

    private int m_stageNum = 0;
    private List<FadeBase> m_fadeList = new List<FadeBase>();
    FadeBase m_fade = null;
    private Canvas m_canvas = null;
    
    private int m_startStage = 0;
    public int StartStage
    {
        get
        {
            return m_startStage;
        }
        set
        {
            m_startStage = m_stageNum - 1;
        }
    }
    private int m_clearStage = 0;
    public int ClearStage
    {
        get
        {
            return m_clearStage;
        }
        set
        {
            m_clearStage = value;
        }
    }

    private int m_maxStage = 1;
    public int MaxStage
    {
        get
        {
            return m_maxStage;
        }
    }

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
    }
    public FadeBase.FadeState State
    {
        get
        {
            if (!m_fade)
                return FadeBase.FadeState.None;

            return m_fade.State;
        }
    }

    public void NextStage()
    {
        m_stageNum++;
        //m_clearStage = m_stageNum - 1;
    }
    override protected void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
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

        m_maxStage = FindObjectsOfType<Stage>().Length;
    }
    void Update()
    {
        // パワー
        m_canvas.worldCamera = Camera.main;
    }

    public void StartFade(FadeType type, string nextScene, int nextStage)
    {
        m_fade = m_fadeList[(int)type].GetComponent<FadeBase>();
        m_fade.FadeStart(nextScene);
        m_stageNum = nextStage;
        EffectManager.Instance.DestoryEffects();
        EffectManager.Instance.Create = false;
    }
}
