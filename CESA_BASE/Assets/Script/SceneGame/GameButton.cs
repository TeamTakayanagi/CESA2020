using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    private const float SLIDE_TIME = 1.0f;
    private const float SLIDE_VALUE = 1.5f;     // 1つとその半分移動
    private enum ChildType
    {
        Sride,
        Retire,
        Retry,
        GameSpeed,
    }

    // スライドして画面に出てきているかどうか
    private bool m_isSride = false;
    private Vector3 m_defaultPos;
    private RectTransform rectTrans;

    public bool Slide
    {
        get
        {
            return m_isSride;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();

        m_defaultPos = rectTrans.localPosition;
    }

    public void SlideVar()
    {
        // サウンド
        Sound.Instance.PlaySE(Audio.SE.Click, GetInstanceID());

        if (!m_isSride)
        {
            rectTrans.DOLocalMove(m_defaultPos + new Vector3(rectTrans.sizeDelta.x * SLIDE_VALUE, 0.0f, 0.0f), SLIDE_TIME);
            m_isSride = true;
        }
        else
        {
            rectTrans.DOLocalMove(m_defaultPos, SLIDE_TIME);
            m_isSride = false;
        }
    }
}
