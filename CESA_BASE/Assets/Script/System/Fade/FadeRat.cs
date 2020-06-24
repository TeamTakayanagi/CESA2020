using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeRat : FadeBase
{
    const int REAN = 3;

    private const float FUSE_POS_X = 3000;
    private const float FUSE_POS_Y = 370;
    private const float RAT_POS_X = 2300;
    private const float FADE_FUSE_TIME = 2.0f;
    private const float RAT_POS_Y = 330;
    private const float FADE_RAT_TIME = 2.0f;

    private float m_target = 0.0f;
    private float m_fuseWidth = 0.0f;
    private List<Image> m_fuseImage = new List<Image>();
    private List<Transform> m_ratRect = new List<Transform>();
    private Transform m_judgeTrans = null;      // フェードの終了判断となるオブジェクトの格納（正方向に移動するオブジェクト）

    new void Start()
    {
        Transform fuse = transform.GetChild(0);
        for (int i = 0; i < fuse.childCount; ++i)
        {
            Image fuseImage = fuse.GetChild(i).GetComponent<Image>();
            fuseImage.transform.localPosition = new Vector3(FUSE_POS_X * (i % 2 * 2 - 1), FUSE_POS_Y - i * FUSE_POS_Y, 0.0f);
            fuseImage.material.SetFloat("_Current", 1.0f);
            int pp = i % 2;
            fuseImage.material.SetFloat("_Direct", pp);
            m_fuseImage.Add(fuseImage);
        }

        Transform rat = transform.GetChild(1);
        for (int i = 0; i < rat.childCount; ++i)
        {
            Transform rect = rat.GetChild(i);
            rect.localPosition = new Vector3(RAT_POS_X * -(i % 2 * 2 - 1), RAT_POS_Y - i * RAT_POS_Y, 0.0f);
            m_ratRect.Add(rect);
        }

        m_fuseWidth = m_fuseImage[0].GetComponent<RectTransform>().rect.width;
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 導火線の移動
    /// </summary>
    override protected void FadeIn()
    {
        m_judgeTrans = m_fuseImage[0].transform;
        m_target = 0.0f;
        base.FadeIn();

        for (int i = 0; i < m_fuseImage.Count; ++i)
        {
            Transform trans = m_fuseImage[i].transform;
            trans.DOLocalMoveX(m_target, FADE_FUSE_TIME);
        }
    }

    /// <summary>
    /// ネズミの移動
    /// </summary>
    override protected void FadeOut()
    {
        m_judgeTrans = m_ratRect[1];
        base.FadeOut();
        StartCoroutine(DoMoveRat(FADE_RAT_TIME));
    }

    override protected bool FadeCheack()
    {
        return m_judgeTrans.localPosition.x == m_target;
    }

    override protected void Draw(bool isDraw)
    {
        if (!isDraw)
        {
            for (int i = 0; i < m_fuseImage.Count; ++i)
            {
                Image trans = m_fuseImage[i];
                trans.enabled = isDraw;
            }
            for (int i = 0; i < m_ratRect.Count; ++i)
            {
                Transform trans = m_ratRect[i];
                if (!trans)
                    continue;

                trans.GetComponent<Image>().enabled = isDraw;
                // 描画されていないなら
                if (!isDraw)
                    trans.DOPause();
            }
        }
        else
        {
            for (int i = 0; i < m_fuseImage.Count; ++i)
            {
                Image trans = m_fuseImage[i];
                trans.enabled = isDraw;
                trans.transform.localPosition = new Vector3(FUSE_POS_X * (i % 2 * 2 - 1), FUSE_POS_Y - i * FUSE_POS_Y, 0.0f);
            }
            for (int i = 0; i < m_ratRect.Count; ++i)
            {
                Transform trans = m_ratRect[i];
                trans.GetComponent<Image>().enabled = isDraw;
                trans.localPosition = new Vector3(RAT_POS_X * -(i % 2 * 2 - 1), RAT_POS_Y - i * RAT_POS_Y, 0.0f);

                // 描画されていないなら、Dotween
                if (!isDraw)
                    trans.DOPause();
            }
        }
    }

    private IEnumerator DoMoveRat(float time)
    {
        int loop = 0;
        float timeCounter = 0.0f;
        float[] target = new float[REAN];
        float[] value = new float[REAN];

        for(int i = 0; i < REAN; ++i)
        {
            target[i] = - m_ratRect[i].localPosition.x;
            value[i] = (target[i] - m_ratRect[i].localPosition.x) / time;
        }
        m_target = target[1];
        while (true)
        {
            loop = 0;
            for (loop = 0; loop < REAN; ++loop)
            {
                m_ratRect[loop].localPosition += new Vector3(value[loop] * Time.deltaTime, 0.0f, 0.0f);
                float curret = (target[loop] - m_ratRect[loop].localPosition.x) / target[loop] / 2;
                m_fuseImage[loop].material.SetFloat("_Current", Mathf.Clamp01(curret));
            }

            if (timeCounter >= time)
                break;

            timeCounter += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
