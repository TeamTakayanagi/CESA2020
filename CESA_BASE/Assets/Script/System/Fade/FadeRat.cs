using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeRat : FadeBase
{
    const int REAN = 3;

    private const float FUSE_POS_X = 4500;
    private const float FUSE_POS_Y = 355;
    private const float RAT_POS_X = 3000;
    private const float RAT_POS_Y = 355;
    private const float FADE_RAT_TIME = 4.0f;

    private float m_fuseWidth = 0.0f;
    private List<Image> m_fuseRect = new List<Image>();
    private List<Transform> m_ratRect = new List<Transform>();
    private Transform m_judgeTrans = null;      // フェードの終了判断となるオブジェクトの格納（正方向に移動するオブジェクト）

    new void Start()
    {
        Transform fuse = transform.GetChild(0);
        for (int i = 0; i < fuse.childCount; ++i)
        {
            Image rect = fuse.GetChild(i).GetComponent<Image>();
            rect.transform.localPosition = new Vector3(FUSE_POS_X * (i % 2 * 2 - 1), FUSE_POS_Y - i * FUSE_POS_Y, 0.0f);
            m_fuseRect.Add(rect);
        }

        Transform rat = transform.GetChild(1);
        for (int i = 0; i < rat.childCount; ++i)
        {
            Transform rect = rat.GetChild(i);
            rect.localPosition = new Vector3(RAT_POS_X * -(i % 2 * 2 - 1), RAT_POS_Y - i * RAT_POS_Y, 0.0f);
            m_ratRect.Add(rect);
        }

        m_fuseWidth = m_fuseRect[0].GetComponent<RectTransform>().rect.width;
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    override protected void FadeIn()
    {
        m_judgeTrans = m_fuseRect[0].transform;
        base.FadeIn();

        for (int i = 0; i < m_fuseRect.Count; ++i)
        {
            Transform trans = m_fuseRect[i].transform;
            trans.DOLocalMoveX(0, FADE_RAT_TIME);
        }
    }

    override protected void FadeOut()
    {
        m_judgeTrans = m_ratRect[1];
        base.FadeOut();

        for (int i = 0; i < m_ratRect.Count; ++i)
        {
            Transform trans = m_ratRect[i];
            trans.DOLocalMoveX(0, FADE_RAT_TIME);
        }
    }
    override protected bool FadeCheack()
    {
        return m_judgeTrans.localPosition.x > - FUSE_POS_X * 0.1f && m_judgeTrans.localPosition.x < FUSE_POS_X * 0.1f;
    }

    override protected void Draw(bool isDraw)
    {
        if (!isDraw)
        {
            for (int i = 0; i < m_fuseRect.Count; ++i)
            {
                Image trans = m_fuseRect[i];
                trans.enabled = isDraw;
            }
            for (int i = 0; i < m_ratRect.Count; ++i)
            {
                Transform trans = m_ratRect[i];
                if (!trans)
                    continue;

                trans.GetChild(0).GetComponent<Image>().enabled = isDraw;
                // 描画されていないなら
                if (!isDraw)
                    trans.DOPause();
            }
        }
        else
        {
            for (int i = 0; i < m_fuseRect.Count; ++i)
            {
                Image trans = m_fuseRect[i];
                trans.enabled = isDraw;
                trans.transform.localPosition = new Vector3(FUSE_POS_X * (i % 2 * 2 - 1), FUSE_POS_Y - i * FUSE_POS_Y, 0.0f);
            }
            for (int i = 0; i < m_ratRect.Count; ++i)
            {
                Transform trans = m_ratRect[i];
                trans.GetChild(0).GetComponent<Image>().enabled = isDraw;
                trans.localPosition = new Vector3(RAT_POS_X * -(i % 2 * 2 - 1), RAT_POS_Y - i * RAT_POS_Y, 0.0f);

                // 描画されていないなら、Dotween
                if (!isDraw)
                    trans.DOPause();
            }
        }
    }

    private IEnumerator DoMoveRat(float time)
    {
        float timeCounter = 0.0f;
        float[] target = new float[REAN];
        float[] value = new float[REAN];

        for(int i = 0; i < REAN; ++i)
        {
            target[i] = - m_ratRect[i].localPosition.x;
            value[i] = (target[i] - m_ratRect[i].localPosition.x) / time;
        }

        while (true)
        {
            for (int i = 0; i < REAN; ++i)
            {
                m_ratRect[i].localPosition += new Vector3(value[i] * Time.deltaTime, 0.0f, 0.0f);
                m_fuseRect[i].material.SetFloat("_Current", (target[i] - m_ratRect[i].localPosition.x) / m_fuseWidth);
            }

            if (timeCounter >= time)
                break;

            timeCounter += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
