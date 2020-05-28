﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeRat : FadeBase
{
    private const float FUSE_POS_X = 4500;
    private const float FUSE_POS_Y = 330;
    private const float RAT_POS_X = 3000;
    private const float RAT_POS_Y = 330;
    private const float FADE_RAT_TIME = 5.0f;

    [SerializeField]
    private ParticleSystem m_particleRat = null;
    private List<RectTransform> m_fuseRect = new List<RectTransform>();
    private List<RectTransform> m_ratRect = new List<RectTransform>();
    private float m_target = 0.0f;
    private RectTransform m_judgeTrans = null;      // フェードの終了判断となるオブジェクトの格納（正方向に移動するオブジェクト）

    // Start is called before the first frame update
    new void Start()
    {
        Transform fuse = transform.GetChild(0);
        for (int i = 0; i < fuse.childCount; ++i)
        {
            RectTransform rect = fuse.GetChild(i).GetComponent<RectTransform>();
            rect.localPosition = new Vector3(FUSE_POS_X * (i % 2 * 2 - 1), FUSE_POS_Y - i * FUSE_POS_Y, 0.0f);
            m_fuseRect.Add(rect);
        }
        Transform rat = transform.GetChild(1);
        for (int i = 0; i < rat.childCount; ++i)
        {
            RectTransform rect = rat.GetChild(i).GetComponent<RectTransform>();
            rect.localPosition = new Vector3(RAT_POS_X * -(i % 2 * 2 - 1), RAT_POS_Y - i * RAT_POS_Y, 0.0f);
            m_ratRect.Add(rect);
        }
        m_func = null;
        m_particleRat.Stop();
        m_state = FadeState.None;
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    override protected void FadeIn()
    {
        m_target = transform.GetChild(0).position.x;
        m_judgeTrans = m_fuseRect[0];
        base.FadeIn();

        for (int i = 0; i < m_fuseRect.Count; ++i)
        {
            RectTransform trans = m_fuseRect[i];
            trans.DOMoveX(m_target, FADE_RAT_TIME);
        }
    }

    override protected void FadeOut()
    {
        m_particleRat.Play();

        m_target = transform.GetChild(0).position.x;
        m_judgeTrans = m_ratRect[1];
        base.FadeOut();

        for (int i = 0; i < m_ratRect.Count; ++i)
        {
            RectTransform trans = m_ratRect[i];
            trans.DOMoveX(m_target, FADE_RAT_TIME);
        }
    }
    override protected bool FadeCheack()
    {
        return (int)Mathf.Ceil(m_judgeTrans.position.x) == (int)m_target;
    }

    override protected void Draw(bool isDraw)
    {
        for (int i = 0; i < m_fuseRect.Count; ++i)
        {
            RectTransform trans = m_fuseRect[i];
            trans.GetComponent<SpriteRenderer>().enabled = isDraw;
        }
        for (int i = 0; i < m_ratRect.Count; ++i)
        {
            RectTransform trans = m_ratRect[i];
            trans.GetChild(0).GetComponent<SpriteRenderer>().enabled = isDraw;
        }
    }
}
