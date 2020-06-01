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

    // Start is called before the first frame update
    void Start()
    {
        m_canvas = GetComponent<Canvas>();

        DontDestroyOnLoad(gameObject);
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_fadeList.Add(transform.GetChild(i).GetComponent<FadeBase>());
        }

        m_fadeList.Sort((a, b) => a.FadeType - b.FadeType);

        m_maxStage = GameObject.FindGameObjectWithTag(NameDefine.TagName.StageParent).transform.childCount;
    }
    void Update()
    {
        m_canvas.worldCamera = Camera.main;
    }


    public void StartFade(FadeType type, string nextScene, int nextStage)
    {
        m_fade = m_fadeList[(int)type].GetComponent<FadeBase>();
        m_fade.FadeStart(nextScene);
        m_stageNum = nextStage;
    }

    public void SetCamera()
    {
        m_canvas.worldCamera = Camera.main;
    }
}
