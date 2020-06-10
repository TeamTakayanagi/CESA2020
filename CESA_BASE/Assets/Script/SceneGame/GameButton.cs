using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    private const float SLIDE_RATE = 3.0f / 5.0f;
    private const float SLIDE_TIME = 1.0f;
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

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (!m_isSride)
        {
            rect.localPosition -= new Vector3(rect.sizeDelta.x * SLIDE_RATE, 0.0f, 0.0f);
        }

        m_defaultPos = rect.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SlideVar()
    {
        Debug.Log("サイドバー");
        RectTransform rect = GetComponent<RectTransform>();

        if (!m_isSride)
        {
            rect.DOLocalMove(m_defaultPos + new Vector3(rect.sizeDelta.x * SLIDE_RATE, 0.0f, 0.0f), SLIDE_TIME);
            m_defaultPos += new Vector3(rect.sizeDelta.x * SLIDE_RATE, 0.0f, 0.0f);
            m_isSride = true;
        }
        else
        {
            rect.DOLocalMove(m_defaultPos - new Vector3(rect.sizeDelta.x * SLIDE_RATE, 0.0f, 0.0f), SLIDE_TIME);
            m_defaultPos -= new Vector3(rect.sizeDelta.x * SLIDE_RATE, 0.0f, 0.0f);
            m_isSride = false;
        }
    }
}
